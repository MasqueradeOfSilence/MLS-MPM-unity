using Unity.Mathematics;

public class AirParticle_3D : Particle_3D
{
    private double mass = 0.5;
    public void Init(double3 position, double3 velocity, double3x3 affineMomentumMatrix)
    {
        Init(position, velocity, mass, affineMomentumMatrix);
    }
    public new double GetMass()
    {
        return mass;
    }
}
