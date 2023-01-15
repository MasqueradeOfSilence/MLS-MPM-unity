using UnityEngine;
using Unity.Mathematics;

public class GridCell : ScriptableObject
{
    private double2 velocity;
    private double mass;

    public void InitGridCell(double2 velocity, double mass)
    {
        this.velocity = velocity;
        this.mass = mass;
    }

    public double2 GetVelocity()
    {
        return velocity;
    }

    public double GetMass()
    {
        return mass;
    }

    public void SetVelocity(double2 newVelocity)
    {
        velocity = newVelocity;
    }

    public void SetMass(double newMass)
    {
        mass = newMass;
    }
}
