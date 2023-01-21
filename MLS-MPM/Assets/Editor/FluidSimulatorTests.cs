using NUnit.Framework;
using UnityEngine;
using Unity.Mathematics;

[TestFixture]
public class FluidSimulatorTests
{
    [Test]
    public void InitializeGridAndParticleArraysShouldFillAnArrayOfParticlesAndInstantiateTheGrid()
    {
        FluidSimulator fluidSimulator = GameObject.Find("ExampleGeo").AddComponent<FluidSimulator>();
        fluidSimulator.InitializeGridAndParticleArrays();
        // 16 to 48 with a spacing of 0.5, two dimensions = 128 particles.
        int expectedParticleCount = 192;
        Assert.AreEqual(fluidSimulator.GetParticleCount(), expectedParticleCount);
        Assert.IsNotNull(fluidSimulator.GetGrid());
        Particle particle = fluidSimulator.GetParticles()[0, 0];
        Assert.IsNotNull(particle);
    }

    [Test]
    public void BuildGridOfTemporaryParticlePositionsShouldCreateEvenlySpacedParticlesFrom16_16To64_64()
    {
        // might be a bad grid size
        FluidSimulator fluidSimulator = GameObject.Find("ExampleGeo").AddComponent<FluidSimulator>();
        double2[,] temporaryParticlePositionGrid = fluidSimulator.BuildGridOfTemporaryParticlePositions();
        double2 expectedFirstPosition = new(16, 16);
        double2 actualFirstPosition = temporaryParticlePositionGrid[0, 0];
        Assert.AreEqual(actualFirstPosition, expectedFirstPosition);
        double2 expectedLastPosition = new(63.5, 63.5);
        double2 actualLastPosition = temporaryParticlePositionGrid[temporaryParticlePositionGrid.GetLength(0) - 1, temporaryParticlePositionGrid.GetLength(1) - 1];
        Assert.AreEqual(actualLastPosition, expectedLastPosition);
        double expectedParticlePositionGridSize = 9216; // 96 x 96
        double actualParticlePositionGridSize = temporaryParticlePositionGrid.Length;
        Assert.AreEqual(actualParticlePositionGridSize, expectedParticlePositionGridSize);
    }

    // TODO: Finish this test

    //[Test]
    //public void SimulateShouldCorrectlyRunClearGrid_P2G1_P2G2_UpdateGridAndG2PToUpdateParticlePositions()
    //{
    //    FluidSimulator fluidSimulator = GameObject.Find("ExampleGeo").AddComponent<FluidSimulator>();
    //    fluidSimulator.InitializeGridAndParticleArrays();
    //    // we are only testing one simulation step
    //    fluidSimulator.Simulate();
    //    // Instead of setting mocks for the 3 steps, we can check the final particle position (first and last).
    //    // include boundary checking
    //}

    [Test]
    public void ClearGridShouldResetTheGridBySettingCellMassesAndVelocitiesToZero()
    {
        double2 expectedFirstCellVelocity = new(0, 0);
        double2 expectedLastCellVelocity = new(0, 0);
        double expectedFirstCellMass = 0;
        double expectedLastCellMass = 0;
        int firstCellXPosition = 0;
        int firstCellYPosition = 0;
        int lastCellXPosition = 95;
        int lastCellYPosition = 95;
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

    // Commenting out for now because these tests don't do much yet and really slow down the suite

    //[Test]
    //public void ParticleToGridStep1ShouldUpdateTheMassAndVelocityOfEachCell()
    //{
    //    FluidSimulator fluidSimulator = GameObject.Find("ExampleGeo").AddComponent<FluidSimulator>();
    //    fluidSimulator.InitializeGridAndParticleArrays();
    //    fluidSimulator.ParticleToGridStep1();
    //    // masses will update, likely to 1. not sure if velocities will update until a later frame.
    //    //Debug.Log(fluidSimulator.GetGrid().At(0, 0).GetVelocity());
    //    //Debug.Log(fluidSimulator.GetGrid().At(0, 0).GetMass());
    //    //Debug.Log(fluidSimulator.GetGrid().At(95, 95).GetVelocity());
    //    //Debug.Log(fluidSimulator.GetGrid().At(95, 95).GetMass());
    //    //Debug.Log(fluidSimulator.GetGrid().At(48, 48).GetVelocity());
    //    //Debug.Log(fluidSimulator.GetGrid().At(48, 48).GetMass());
    //    fluidSimulator.ClearGrid();
    //}

    //[Test]
    //public void ParticleToGridStep2ShouldUpdateTheVelocityOfEachCellUsingMomentum()
    //{
    //    FluidSimulator fluidSimulator = GameObject.Find("ExampleGeo").AddComponent<FluidSimulator>();
    //    fluidSimulator.InitializeGridAndParticleArrays();
    //    fluidSimulator.ParticleToGridStep1();
    //    fluidSimulator.ParticleToGridStep2();
    //    //Debug.Log(fluidSimulator.GetGrid().At(0, 0).GetVelocity());
    //    //Debug.Log(fluidSimulator.GetGrid().At(95, 95).GetVelocity());
    //    //Debug.Log(fluidSimulator.GetGrid().At(48, 48).GetVelocity());
    //    //Debug.Log(fluidSimulator.GetGrid().At(21, 87).GetVelocity());
    //    fluidSimulator.ClearGrid();
    //}

    //[Test]
    //public void UpdateGridShouldConvertMomentumToVelocity_ApplyGravity_AndEnforceBoundaryConditions()
    //{
    //    FluidSimulator fluidSimulator = GameObject.Find("ExampleGeo").AddComponent<FluidSimulator>();
    //    fluidSimulator.InitializeGridAndParticleArrays();
    //    fluidSimulator.ParticleToGridStep1();
    //    fluidSimulator.ParticleToGridStep2();
    //    fluidSimulator.UpdateGrid();
    //    //Debug.Log(fluidSimulator.GetGrid().At(0, 0).GetVelocity());
    //    //Debug.Log(fluidSimulator.GetGrid().At(95, 95).GetVelocity());
    //    //Debug.Log(fluidSimulator.GetGrid().At(48, 48).GetVelocity());
    //    //Debug.Log(fluidSimulator.GetGrid().At(21, 87).GetVelocity());
    //    fluidSimulator.ClearGrid();
    //}

    //[Test]
    //public void GridToParticleStepShouldAdvectParticlesToNewPositionsAndEnforceBoundaryConditions()
    //{
    //    FluidSimulator fluidSimulator = GameObject.Find("ExampleGeo").AddComponent<FluidSimulator>();
    //    fluidSimulator.InitializeGridAndParticleArrays();
    //    fluidSimulator.ParticleToGridStep1();
    //    fluidSimulator.ParticleToGridStep2();
    //    fluidSimulator.UpdateGrid();
    //    fluidSimulator.GridToParticleStep();
    //    //Debug.Log(fluidSimulator.GetParticles()[0, 0].GetPosition());
    //    //Debug.Log(fluidSimulator.GetParticles()[95, 95].GetPosition());
    //    //Debug.Log(fluidSimulator.GetParticles()[48, 48].GetPosition());
    //    //Debug.Log(fluidSimulator.GetParticles()[21, 87].GetPosition());
    //    fluidSimulator.ClearGrid();
    //}

}
