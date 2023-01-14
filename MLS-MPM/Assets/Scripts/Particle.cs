using Unity.Mathematics;
using UnityEngine;

public class Particle : ScriptableObject
{
    private Vector2 position;
    private Vector2 velocity;
    private double mass;
    // The affine momentum matrix C
    private double2x2 affineMomentumMatrix;

    public void InitParticle(Vector2 position, Vector2 velocity, double mass, double2x2 affineMomentumMatrix)
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

    public Vector2 GetPosition()
    {
        return position;
    }

    public Vector2 GetVelocity()
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
