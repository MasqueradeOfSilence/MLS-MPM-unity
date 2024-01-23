using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

[TestFixture]
public class AirParticle_3D_Test 
{
    [Test]
    public void AirParticleShouldBeInitializedWithAMassOfBetween0And1()
    {
        double3 testPosition = new(0);
        double3 testVelocity = new(0, 1, 0);
        double3x3 testC = new(0);
        AirParticle_3D particle = ScriptableObject.CreateInstance("AirParticle_3D") as AirParticle_3D;
        particle.Init(testPosition, testVelocity, testC);
        double expectedMass = 0.5;
        Assert.AreEqual(expectedMass, particle.GetMass());
    }
}
