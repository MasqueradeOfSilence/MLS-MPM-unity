using Unity.Mathematics;

public class AirParticle : Particle
{
    private double mass = 0.5; // 0.5 might be too high, but anything lower, and it destabilizes the sim. I'm not sure where the particles end up going, but they aren't in the viewport. 
    public void InitParticle(double2 position, double2 velocity, double2x2 affineMomentumMatrix)
    {
        InitParticle(position, velocity, mass, affineMomentumMatrix);
    }

    public new double GetMass()
    {
        return mass;
    }

}
