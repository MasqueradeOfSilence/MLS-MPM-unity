using Unity.Mathematics;
using UnityEngine;

public class Particle : ScriptableObject
{
    private double2 position;
    private double2 velocity;
    private double mass;
    // The affine momentum matrix C
    private double2x2 affineMomentumMatrix;
    private Bubble bubble = null;


    public void InitParticle(double2 position, double2 velocity, double mass, double2x2 affineMomentumMatrix)
    {
        this.position = position;
        this.velocity = velocity;
        this.mass = mass;
        this.affineMomentumMatrix = affineMomentumMatrix;
    }

    public GameObject ConstructSphereFromParticle(string materialName = "ClearBubbleTest")
    {
        if (bubble != null)
        {
            float unitySphereRadius = bubble.ComputeUnitySphereRadius();
            return GeometryCreator.SpawnParticleSphere_2DVersion(position, mass, unitySphereRadius, materialName);
        }
        return GeometryCreator.SpawnParticleSphere_2DVersion(position, mass, materialName: materialName);
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

    public Bubble GetBubble()
    {
        if (bubble != null)
        {
            return bubble;
        }
        return null;
    }

    public double2x2 GetAffineMomentumMatrix()
    {
        return affineMomentumMatrix;
    }

    public void SetPosition(double[] position)
    {
        this.position = GeneralMathUtils.Format2DVectorForMath(position);
    }

    public void SetPosition(double2 position)
    {
        this.position = position;
    }

    public void SetVelocity(double[] velocity)
    {
        this.velocity = GeneralMathUtils.Format2DVectorForMath(velocity);
    }

    public void SetVelocity(double2 velocity)
    {
        this.velocity = velocity;
    }

    public void SetAffineMomentumMatrix(double2x2 C)
    {
        affineMomentumMatrix = C;
    }

    public void SetBubbleWithSize(double volumeFraction)
    {
        bubble = CreateInstance<Bubble>();
        bubble.InstantiateBubble(volumeFraction);
        // bug: I think bubble is instantiated after the unity object is spawned?
    }

    public void UpdateVelocityX(double velocityX)
    {
        velocity.x = velocityX;
    }

    public void UpdateVelocityY(double velocityY)
    {
        velocity.y = velocityY;
    }
}
