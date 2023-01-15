using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class FluidSimulatorTests
{
    [Test]
    public void InitializeParticleArrayShouldFillAnArrayOfParticles()
    {
        FluidSimulator fluidSimulator = GameObject.Find("ExampleGeo").AddComponent<FluidSimulator>();
        fluidSimulator.InitializeParticleArray();
        // 16 to 48 with a spacing of 0.5, two dimensions = 128 particles.
        int expectedParticleCount = 128;
        Assert.AreEqual(fluidSimulator.GetParticleCount(), expectedParticleCount);
        Particle particle = fluidSimulator.GetParticles()[0, 0];
        // Assert.IsNotNull(particle);
    }

    [Test]
    public void InitializePositionsShouldCreateAGridOfEvenlySpacedInitialPositionsForCells()
    {
        // once this is written, write the test to fill up the particle array w/default values, then uncomment line 16.
    }
}
