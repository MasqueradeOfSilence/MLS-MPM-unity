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
    private Bubble_3D bubble = null;
    private bool initialized = false;
    private bool bubbleSet = false;

    public void CreateBlank()
    {
        position = 0;
        velocity = 0;
        mass = 0;
        affineMomentumMatrix = 0;
    }

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
        velocity = new(0);
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

    public Bubble_3D GetBubble()
    {
        return bubble;
    }

    public void SetBubble(double volumeFraction, bool skipped = false, float initialSizingFactor = 1f)
    {
        bubble = CreateInstance<Bubble_3D>();
        bubble.InstantiateBubble(volumeFraction, skipped, initialSizingFactor);
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
