using NUnit.Framework;
using UnityEngine;
using Unity.Mathematics;

[TestFixture]
public class FluidSimulatorTests
{
    [Test]
    public void InitializeGridShouldInstantiateTheGrid()
    {
        FluidSimulator fluidSimulator = GameObject.Find("ExampleGeo").AddComponent<FluidSimulator>();
        fluidSimulator.InitializeGrid();
        Assert.IsNotNull(fluidSimulator.GetGrid());
        int2 expectedSize = new(64, 64);
        Assert.AreEqual(fluidSimulator.GetGrid().GetSize(), expectedSize);
    }

    public void InitializeParticlesWrapperShouldInitialize4096Particles()
    {
        FluidSimulator fluidSimulator = GameObject.Find("ExampleGeo").AddComponent<FluidSimulator>();
        fluidSimulator.InitializeGrid();
        fluidSimulator.InitializeParticles();
        // 16 to 48 with a spacing of 0.5, two dimensions = 4096 particles.
        int expectedParticleCount = 4096;
        Assert.AreEqual(fluidSimulator.GetParticleCount(), expectedParticleCount);
        Particle particle = fluidSimulator.GetParticles()[0, 0];
        Assert.IsNotNull(particle);
    }

    [Test]
    public void BuildGridOfTemporaryParticlePositionsShouldCreateEvenlySpacedParticlesFrom16_16To48_48()
    {
        FluidSimulator fluidSimulator = GameObject.Find("ExampleGeo").AddComponent<FluidSimulator>();
        double2[,] temporaryParticlePositionGrid = fluidSimulator.BuildGridOfTemporaryParticlePositions();
        double2 expectedFirstPosition = new(16, 16);
        double2 actualFirstPosition = temporaryParticlePositionGrid[0, 0];
        Assert.AreEqual(actualFirstPosition, expectedFirstPosition);
        double2 expectedLastPosition = new(47.5, 47.5);
        double2 actualLastPosition = temporaryParticlePositionGrid[temporaryParticlePositionGrid.GetLength(0) - 1, temporaryParticlePositionGrid.GetLength(1) - 1];
        Assert.AreEqual(actualLastPosition, expectedLastPosition);
        double expectedParticlePositionGridSize = 4096;
        double actualParticlePositionGridSize = temporaryParticlePositionGrid.Length;
        Assert.AreEqual(actualParticlePositionGridSize, expectedParticlePositionGridSize);
    }

    [Test]
    public void ClearGridShouldResetTheGridBySettingCellMassesAndVelocitiesToZero()
    {
        double2 expectedFirstCellVelocity = new(0, 0);
        double2 expectedLastCellVelocity = new(0, 0);
        double expectedFirstCellMass = 0;
        double expectedLastCellMass = 0;
        int firstCellXPosition = 0;
        int firstCellYPosition = 0;
        int lastCellXPosition = 63;
        int lastCellYPosition = 63;
        FluidSimulator fluidSimulator = GameObject.Find("ExampleGeo").AddComponent<FluidSimulator>();
        // clear grid just re-initializes, so no need for first init call
        fluidSimulator.ClearGrid();
        GridCell firstCell = fluidSimulator.GetGrid().At(firstCellXPosition, firstCellYPosition);
        GridCell lastCell = fluidSimulator.GetGrid().At(lastCellXPosition, lastCellYPosition);
        Assert.AreEqual(firstCell.GetVelocity(), expectedFirstCellVelocity);
        Assert.AreEqual(firstCell.GetMass(), expectedFirstCellMass);
        Assert.AreEqual(lastCell.GetVelocity(), expectedLastCellVelocity);
        Assert.AreEqual(lastCell.GetMass(), expectedLastCellMass);
    }

    public void InitializeFluidSimulatorShouldSetUpGridAndParticles()
    {

    }

}
