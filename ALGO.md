# Foam Fraction Flow Algorithm

The purpose of the Foam Fraction Flow (FFF) algorithm is to simulate foam. 

Specifically, it simulates low-density, drier foams, with areas of both macroscopic and microscopic bubbles. Examples of this type of foam include foaming soaps (such as Method), bubble bath foam, and hot tub foam. 

We use MLS-MPM with a multiphase Herschel-Bulkley constitutive model to drive our fluid simulation. We spawn, advect, and remove foam bubbles of 4 different sizes (microscopic, small, medium, and large) using the volume fraction algorithm. 

The volume fraction algorithm is designed to advect bubbles based on the parent simulation (in this case, MLS-MPM). Bubble-to-bubble collision behavior is modeled by Plateau's Laws, as implemented in a weighted Voronoi diagram. 

Bubble volume is calculated using a secondary simulation. A control method is used to minimize volume loss at each timestep. 

## Rationale

We expect to improve upon the volume fractions method by using MLS-MPM adapted for foam instead of the cloth simulator that they use. We hope to improve performance and maintain high artistic quality. 


## Important papers and links
- [MPM Guide](https://nialltl.neocities.org/articles/mpm_guide): This guide from NiallTL explains the MLS-MPM algorithm, exploring a non-Newtonian fluid simulation using a Neo-Hookean constitutive model as well as a Newtonian fluid simulation using a constitutive model designed for isotropic fluids. The linked code is very useful. I based my Newtonian fluid solver (found in `FluidSimulator.cs`) off of it. 
- [Continuum Foam: A Material-Point Method for Shear-Dependent Flows](http://www.cs.columbia.edu/cg/foam/foam_files/continuumfoam.pdf): This paper from Columbia University asserts that the Herschel-Bulkley constitutive model is an effective model for high-density dry foams without visible bubbles, such as shaving cream, oobleck, and melted marshmallows. It suggests that it may be effective for dry foams if combined with a bubble simulation model. 
- [MLS-MPM Paper](https://yuanming.taichi.graphics/publication/2018-mlsmpm/mls-mpm-cpic.pdf): The original paper as referenced heavily by NiallTL, which asserts that MLS-MPM is twice as fast as the original MPM algorithm, due to eliminating a computationally expensive gradient calculation. 
- [Animating Bubble Interactions in a Liquid Foam](https://web.cse.ohio-state.edu/~dey.8/paper/foam/bubble.pdf): This paper explains how bubble collision and deformation can be modeled using Plateau's Laws as implemented by a weighted Voronoi diagram. 
- [A Simple Approach to Bubble Modeling from Multiphase Fluid Simulation](https://cg.cs.tsinghua.edu.cn/papers/CVMJ-2015-bubble.pdf): This paper uses volume fractions to adapt an existing multiphase fluid simulation to include bubbles. Bubble size is determined based on the percentage of gas volume at each fluid particle. The more gas that is present, the bigger the bubble. A secondary simulation is used to determine the volume and movement of smaller bubbles, based on the physics data from the primary fluid simulator. Bubble collision and deformation is modeled using weighted Voronoi diagrams. 
- [The Wikipedia entry for Herschel-Bulkley](https://en.wikipedia.org/wiki/Herschel%E2%80%93Bulkley_fluid) has the equations we need for our constitutive model. 

## The algorithm
### Step 1: MLS-MPM solver with isotropic model
We implement the base fluid solver using MLS-MPM. To start, we can implement a Newtonian fluid solver in `FluidSimulator.cs` using a constitutive equation for isotropic fluids. This will describe our stress-strain relationship. 

The Herschel-Bulkley constitutive equation actually reduces to a Newtonian model when the *flow index* is set to 1 and the *yield stress* is set to 0 (more on that later). So when we implement that in `FoamSimulator.cs`, we can run a test with those variables set accordingly. If it behaves similarly to our Newtonian fluid solver, then we have correctly implemented the equation. 

### Step 2: MLS-MPM solver with Herschel-Bulkley constitutive model

In practice, switching out our isotropic constitutive model for a Herschel-Bulkley model means that our *stress* variable in the **P2G2** (Particle to Grid, Step 2) step is what's modified, as part of our stress-strain evaluation. The rest of the MLS-MPM solver should not need any modification. 

Herschel-Bulkley gives us the shear stress as follows:

`shear_stress = yield_stress + (consistency_index * ((shear_rate)^flow_index))`

We need to define these variables, so we know how to actually implement this. 

But we need the total stress, which means that we include pressure as well. The total stress can be approximated as follows: 

`total_stress = -pressure + shear stress`

Thus, we need an equation for pressure. 

