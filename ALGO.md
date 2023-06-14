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

Here are the values we can use for each variable:

- `yield_stress`: From the Columbia paper, we can use a value of `31.9 Pa` for shaving cream (since we want our foam to follow a similar flow pattern), though we may need to modify that so it fits with the units used in NiallTL's simulator. We can also try `79.7 Pa` from the paper below.
- `consistency_index`: This is a constant of proportionality. We can use a value of `19.6 [Pa s^n]`, which works for general foams as well as shaving cream, based on [this paper](https://citeseerx.ist.psu.edu/document?repid=rep1&type=pdf&doi=a9e7f3a9e7f5c1382c99feddbcf1656141bbb360). 
- `shear_rate`: In NiallTL's paper, this is saved as the `strain` variable. It is technically strain *rate*, [AKA shear rate](https://cdn.technologynetworks.com/TN/Resources/PDF/WP160620BasicIntroRheology.pdf). 
- `flow_index`: This variable describes to what extent our material is shear-thinning, or shear-thickening. A decimal value between 0 and 1 (exclusive) models a shear-thinning material; a value over 1 models a shear-thickening material. A value of 1 combined with a yield stress of 0 means it's Newtonian, and thus neither shear-thinning nor shear-thickening. Continuing with our shaving cream model, we will be using a value of `n = 0.22`. We can also try `0.36` based on the PSU paper. 

But we need the total stress, which means that we include pressure as well. The total stress can be approximated as follows: 

`total_stress = -pressure + shear stress`

Thus, we need an equation for pressure. In NiallTL's isotropic model, he uses the [Cole Equation of State](https://en.wikipedia.org/wiki/Cole_equation_of_state) to calculate pressure (and incorrectly refers to it as the Tait Equation of State, apparently a [common mistake](http://www.sklogwiki.org/SklogWiki/index.php/Cole_equation_of_state) that causes much confusion for us graphics programmers). We should be able to use this equation for our foam as well. 

The equation of state is as follows: 

`eosStiffness * (Math.Pow((density / restDensity), eosPower) - 1)`

But if the result of this is less than -0.1, we clamp it to -0.1. 

Density is computed in the solver, so we don't need to do any additional work there. And we can define the constant variables as follows: 

- `eosStiffness`: AKA a "pressure parameter". I cannot find any good information on this "pressure parameter" -- it is all vague. It seems to be a value that magnifies the degree of compressibility, and decreasing density the higher it gets. I will start with the existing value of 10 and then experiment from there. 
- `restDensity`: Rest density is just a reference density, compared to the one computed dynamically. A bubble bath foam should be less dense than water. Niall uses a value of 4 for water. I will experiment with a much smaller value for my foam, possibly anywhere between 0.04 and 1.2. 
- `eosPower`: The *adiabatic index*. Niall uses a value of 4, which seems consistent with what we see for water [here](http://www.mem50212.com/MDME/iTester/get-info/thermodynamics.html). For my foam, I may use a value of 1.4 for air. 

Parentheses are very important. I was stuck on a weird behavioral bug in my Newtonian water solver for a really long time until I realized that my parentheses were incorrect. 

### Step 3: Adapting the solver to be multiphase

Up to now, the solver has only included fluid particles. We need to add gas particles in order to make it multiphase (as volume fractions will not work otherwise). 

Once this is implemented, we will have the necessary groundwork complete for building volume fractions on top of MLS-MPM. 

In order to make the solver multiphase, we need to create two different types of particles: Air and Fluid. Air particles will (indirectly) be used to create bubbles. An air particle may not automatically be a bubble. Instead, the ratio of air to fluid particles in the neighborhood of *any given particle* tells us whether or not a bubble should be in that vicinity. So, there isn't a 1:1 ratio of particles to bubbles, not by any means. 

The only real difference between the two types of particles is the mass value. We can make them both inherit from the Particle class. 
