using Unity.Mathematics;

public class FluidParticle : Particle
{
    private readonly double mass = 3;
    public void InitParticle(double2 position, double2 velocity, double2x2 affineMomentumMatrix)
    {
        InitParticle(position, velocity, mass, affineMomentumMatrix);
    }

    public new double GetMass()
    {
        return mass;
    }

}
