using Unity.Mathematics;
using UnityEngine;

/**
    Particle: Describes a particle in the simulation.
        It may be a bubble or simply part of a liquid mesh.
*/

public class Particle_3D : ScriptableObject
{
    /**
     * Data members
     */
    private double3 position;
    private double3 velocity;
    private double mass;
    private double3x3 affineMomentumMatrix;
    // Bubble class is already fine for 3D, so no need to make a 3D version.
    private Bubble bubble = null;
    private bool initialized = false;
    private bool bubbleSet = false;

    /**
     * Effectively the constructor to initialize the scriptable object
     */
    public void Init(double3 position, double3 velocity, double mass, double3x3 affineMomentumMatrix)
    {
        this.position = position;
        this.velocity = velocity;
        this.mass = mass;
        this.affineMomentumMatrix = affineMomentumMatrix;
        initialized = true;
    }

    public void ResetVelocity()
    {
        velocity = 0;
    }

    /**
     * Getters and Setters
     * 
     * Position -----------------------------
     */
    public double3 GetPosition()
    {
        return position;
    }

    public void SetPosition(double3 position)
    {
        this.position = position;
    }

    // Velocity ------------------------------

    public double3 GetVelocity()
    {
        return velocity;
    }

    public void SetVelocity(double3 velocity)
    {
        this.velocity = velocity;
    }

    // Mass ------------------------------

    public double GetMass()
    {
        return mass;
    }

    // Bubble ------------------------------

    public Bubble GetBubble()
    {
        return bubble;
    }

    public void SetBubble(double volumeFraction, bool skipped = false)
    {
        bubble = CreateInstance<Bubble>();
        bubble.InstantiateBubble(volumeFraction, skipped);
        bubbleSet = true;
    }

    public bool HasBubble()
    {
        return bubble != null;
    }

    // Affine momentum matrix for APIC ------------------------------
    public double3x3 GetC()
    {
        return affineMomentumMatrix;
    }

    public void SetC(double3x3 C)
    {
        affineMomentumMatrix = C;
    }

    // Flags ------------------------------
    public bool IsInitialized()
    {
        return initialized;
    }

    public bool IsBubbleSet()
    {
        return bubbleSet;
    }
}
