using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

[TestFixture]
public class Particle_3D_Test
{
    [Test]
    public void ParticleClassShouldExist()
    {
        double3 testPosition = new(0, 0, 0);
        double3 testVelocity = new(0, 0, 0);
        double testMass = 0;
        double3x3 testC = new();
        Particle_3D particle = ScriptableObject.CreateInstance("Particle_3D") as Particle_3D;
        particle.Init(testPosition, testVelocity, testMass, testC);
        Assert.IsNotNull(particle);
    }
}
