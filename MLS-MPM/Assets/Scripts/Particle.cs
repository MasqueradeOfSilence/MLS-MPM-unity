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

    public void SetPosition(double2 position)
    {
        this.position = position;
    }

    public void SetVelocity(double2 velocity)
    {
        this.velocity = velocity;
    }

    public void SetAffineMomentumMatrix(double2x2 C)
    {
        affineMomentumMatrix = C;
    }

    public void UpdateVelocityX(double velocityX)
    {
        this.velocity.x = velocityX;
    }

    public void UpdateVelocityY(double velocityY)
    {
        this.velocity.y = velocityY;
    }
}
