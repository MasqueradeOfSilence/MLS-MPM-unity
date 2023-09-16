using Unity.Mathematics;
using UnityEngine.UIElements;
using UnityEngine;

public class FluidParticle : Particle
{
    private double mass = 3;
    public void InitParticle(double2 position, double2 velocity, double2x2 affineMomentumMatrix)
    {
        InitParticle(position, velocity, mass, affineMomentumMatrix);
    }

    public new double GetMass()
    {
        return mass;
    }

}
