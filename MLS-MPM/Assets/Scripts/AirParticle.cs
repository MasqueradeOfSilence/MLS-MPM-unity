using Unity.Mathematics;

public class AirParticle : Particle
{
    // TODO: Changing this mass alone doesn't quite do what we want. I need to also take a look at the density, etc. 
    // NOTE: I switched air and fluid as a test, keep experimenting
    private double mass = 10; // 0.5 might be too high, but anything lower, and it destabilizes the sim. I'm not sure where the particles end up going, but they aren't in the viewport. It doesn't seem to change their next position computation by more than like 0.01 though.
    public void InitParticle(double2 position, double2 velocity, double2x2 affineMomentumMatrix)
    {
        InitParticle(position, velocity, mass, affineMomentumMatrix);
    }

    public new double GetMass()
    {
        return mass;
    }

}
