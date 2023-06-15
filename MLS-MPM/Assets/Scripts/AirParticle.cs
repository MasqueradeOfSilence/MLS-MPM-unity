using Unity.Mathematics;

public class AirParticle : Particle
{
    private double2 position;
    private double2 velocity;
    private double mass = 0.5;
    // The affine momentum matrix C
    private double2x2 affineMomentumMatrix;

    public void InitParticle(double2 position, double2 velocity, double2x2 affineMomentumMatrix)
    {
        this.position = position;
        this.velocity = velocity;
        this.affineMomentumMatrix = affineMomentumMatrix;
    }

    public new double GetMass()
    {
        return mass;
    }

}
