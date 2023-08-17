using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

public class NineNeighborhoodTests
{
    // oops code duplication is an anti-pattern TODO abstract me out to UnitTestUtils
    private Particle CreateParticleInUnity()
    {
        return ScriptableObject.CreateInstance("Particle") as Particle;
    }

    private Particle CreateParticleWithGivenPosition(double2 particlePosition)
    {
        double2 initialVelocity = new(0, 0);
        double initialMass = 1;
        double2x2 initialC = new double2x2(0, 0, 0, 0);
        Particle particle = CreateParticleInUnity();
        particle.InitParticle(particlePosition, initialVelocity, initialMass, initialC);
        return particle;
    }

    [Test]
    public void TestNineNeighborhoodShouldCalculateThe3x3Neighborhood()
    {
        double2 testParticlePosition = new(2.5, 12.7);
        Particle particle = CreateParticleWithGivenPosition(testParticlePosition);
        NineNeighborhood nineNeighborhood = new NineNeighborhood(particle);
        int2 expectedUpperLeft = new(1, 11);
        int2 expectedUpper = new(1, 12);
        int2 expectedUpperRight = new(1, 13);
        int2 expectedLeft = new(2, 11);
        int2 expectedCenter = new(2, 12);
        int2 expectedRight = new(2, 13);
        int2 expectedLowerLeft = new(3, 11);
        int2 expectedLower = new(3, 12);
        int2 expectedLowerRight = new(3, 13);
        Assert.AreEqual(nineNeighborhood.GetUpperLeft(), expectedUpperLeft);
        Assert.AreEqual(nineNeighborhood.GetUpper(), expectedUpper);
        Assert.AreEqual(nineNeighborhood.GetUpperRight(), expectedUpperRight);
        Assert.AreEqual(nineNeighborhood.GetLeft(), expectedLeft);
        Assert.AreEqual(nineNeighborhood.GetCenter(), expectedCenter);
        Assert.AreEqual(nineNeighborhood.GetRight(), expectedRight);
        Assert.AreEqual(nineNeighborhood.GetLowerLeft(), expectedLowerLeft);
        Assert.AreEqual(nineNeighborhood.GetLower(), expectedLower);
        Assert.AreEqual(nineNeighborhood.GetLowerRight(), expectedLowerRight);
    }
}
