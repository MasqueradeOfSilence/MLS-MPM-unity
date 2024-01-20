using Unity.Mathematics;
using UnityEngine;

/**
 * Cell: Represents a cell within an MLS-MPM grid.
 *  It's just a data storage class.
 */
public class Cell_3D : ScriptableObject
{
    /**
     * Data members
     */
    private double3 velocity;
    private double mass;

    /**
     * Constructor for ScriptableObject
     */
    public void Init(double3 velocity, double mass)
    {
        this.velocity = velocity;
        this.mass = mass;
    }

    /**
     * Getters and setters
     * 
     * Velocity -----------------
     */
    public double3 GetVelocity()
    {
        return velocity;
    }

    public void SetVelocity(double3 velocity)
    {
        this.velocity = velocity;
    }

    // Mass -----------------

    public double GetMass() 
    {
        return mass;
    }

    public void SetMass(double mass)
    {
        this.mass = mass;
    }
}
