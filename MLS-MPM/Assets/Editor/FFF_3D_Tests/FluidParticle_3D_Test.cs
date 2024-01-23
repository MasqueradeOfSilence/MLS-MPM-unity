using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

[TestFixture]
public class FluidParticle_3D_Test 
{
    [Test]
    public void FluidParticleShouldBeInitializedWithAMassOf3()
    {
        double3 testPosition = new(0);
        double3 testVelocity = new(0, 1, 0);
        double3x3 testC = new(0);
        FluidParticle_3D particle = ScriptableObject.CreateInstance("FluidParticle_3D") as FluidParticle_3D;
        particle.Init(testPosition, testVelocity, testC);
        int expectedMass = 3;
        Assert.AreEqual(expectedMass, particle.GetMass());
    }
}
