using Unity.Mathematics;

public class AirParticle : Particle
{
    private double mass = 0.5;
    public void InitParticle(double2 position, double2 velocity, double2x2 affineMomentumMatrix)
    {
        InitParticle(position, velocity, mass, affineMomentumMatrix);
    }

    public new double GetMass()
    {
        return mass;
    }

}
