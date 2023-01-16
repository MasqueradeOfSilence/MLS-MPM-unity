using NUnit.Framework;
using UnityEngine;
using Unity.Mathematics;

[TestFixture]
public class ParticleTests
{
    [Test]
    public void ParticleClassShouldExist()
    {
        double2 test = new(0, 0);
        double testMass = 0;
        double2x2 testC = new double2x2();
        Particle particle = ScriptableObject.CreateInstance("Particle") as Particle;
        particle.InitParticle(test, test, testMass, testC);
        Assert.IsNotNull(particle);
    }

    [Test]
    public void ConstructSphereFromParticleShouldBuildASphereWithTheSamePositionAsTheParticle()
    {
        double2 testPosition = new(0, 0);
        double2 testVelocity = new(0, 1);
        double testMass = 1;
        double2x2 testC = new double2x2();
        Particle particle = ScriptableObject.CreateInstance("Particle") as Particle; 
        particle.InitParticle(testPosition, testVelocity, testMass, testC);
        GameObject returnedSphere = particle.ConstructSphereFromParticle();
        Vector3 expectedPosition = new Vector3(0, 0, 0);
        Assert.AreEqual(expectedPosition, returnedSphere.transform.position);
    }
}
