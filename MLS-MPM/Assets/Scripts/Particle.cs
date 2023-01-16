using Unity.Mathematics;
using UnityEngine;

public class Particle : ScriptableObject
{
    private double2 position;
    private double2 velocity;
    private double mass;
    // The affine momentum matrix C
    private double2x2 affineMomentumMatrix;

    public void InitParticle(double2 position, double2 velocity, double mass, double2x2 affineMomentumMatrix)
    {
        this.position = position;
        this.velocity = velocity;
        this.mass = mass;
        this.affineMomentumMatrix = affineMomentumMatrix;
    }

    public GameObject ConstructSphereFromParticle()
    {
        return GeometryCreator.SpawnParticleSphere_2DVersion(position);
    }

    public double2 GetPosition()
    {
        return position;
    }

    public double2 GetVelocity()
    {
        return velocity;
    }

    public double GetMass()
    {
        return mass;
    }

    public double2x2 GetAffineMomentumMatrix()
    {
        return affineMomentumMatrix;
    }
}
