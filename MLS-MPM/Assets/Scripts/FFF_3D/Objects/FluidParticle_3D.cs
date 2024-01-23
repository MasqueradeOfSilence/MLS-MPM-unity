using Unity.Mathematics;

/**
 * Fluid Particle: Subclass of a particle which represents part of a fluid. 
 *  A mesh is created from these particles.
 */
public class FluidParticle_3D : Particle_3D
{
    private readonly double mass = 3;

    public void Init(double3 position, double3 velocity, double3x3 affineMomentumMatrix)
    {
        Init(position, velocity, mass, affineMomentumMatrix);
    }

    public new double GetMass()
    {
        return mass;
    }
}
