using NUnit.Framework;
using UnityEngine;
using Unity.Mathematics;

[TestFixture]
public class FluidParticleTests
{

    [Test]
    public void FluidParticleShouldBeInitializedWithAMassOf1()
    {
        double2 testPosition = new(0, 0);
        double2 testVelocity = new(0, 1);
        double2x2 testC = new double2x2();
        FluidParticle particle = ScriptableObject.CreateInstance("FluidParticle") as FluidParticle;
        particle.InitParticle(testPosition, testVelocity, testC);
        Assert.AreEqual(1, particle.GetMass());
    }

    [Test]
    public void OtherParticleValuesShouldBeObtainedCorrectlyFromGetters()
    {
        double2 testPosition = new(1, 0);
        double2 testVelocity = new(0, 1);
        double2x2 testC = new double2x2();
        FluidParticle particle = ScriptableObject.CreateInstance("FluidParticle") as FluidParticle;
        particle.InitParticle(testPosition, testVelocity, testC);

        double2 computedPosition = particle.GetPosition();
        Assert.IsTrue(GeneralMathUtils.DeepEquals(testPosition, computedPosition));
        double2 computedVelocity = particle.GetVelocity();
        Assert.IsTrue(GeneralMathUtils.DeepEquals(testVelocity, computedVelocity));
    }
}
