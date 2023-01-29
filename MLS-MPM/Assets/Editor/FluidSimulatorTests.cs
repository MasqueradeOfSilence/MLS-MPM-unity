using NUnit.Framework;
using UnityEngine;
using Unity.Mathematics;

[TestFixture]
public class FluidSimulatorTests
{
    [Test]
    public void InitializeFluidSimulatorShouldSetUpGridAndParticles()
    {
        // create mocks and test that the functions are called.
    }

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

    [Test]
    public void P2G1ShouldModifyEachGridCellUsingParticleAttributes()
    {
        // Test expected particle masses and velocities
        FluidSimulator fluidSimulator = GameObject.Find("ExampleGeo").AddComponent<FluidSimulator>();
        // Let's save ourselves a few loops.
        int testNeighborDimension = 1;
        fluidSimulator.SetNeighborDimension(testNeighborDimension);
        Particle[,] particles = new Particle[1, 1];
        Particle particle = ScriptableObject.CreateInstance("Particle") as Particle;
        double2 initialVelocity = new(0, 0);
        double initialMass = 1;
        double2x2 initialC = new(0, 0, 0, 0);
        double2 initialPosition = new(16, 16);
        particle.InitParticle(initialPosition, initialVelocity, initialMass, initialC);
        particles[0, 0] = particle;
        fluidSimulator.SetParticles(particles);
        double expectedNewMass = 0.25;
        double2 expectedNewVelocity = new(0, 0);
        fluidSimulator.ParticleToGridStep1();
        GridCell firstCell = fluidSimulator.GetGrid().At(0, 0);
        Assert.AreEqual(expectedNewMass, firstCell.GetMass());
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedNewVelocity, firstCell.GetVelocity()));
    }

    [Test]
    public void P2G2ShouldUpdateGridCellVelocityWithMomentum()
    {
        FluidSimulator fluidSimulator = GameObject.Find("ExampleGeo").AddComponent<FluidSimulator>();
        int testNeighborDimension = 1;
        fluidSimulator.SetNeighborDimension(testNeighborDimension);
        Particle[,] particles = new Particle[1, 1];
        Particle particle = ScriptableObject.CreateInstance("Particle") as Particle;
        double2 initialVelocity = new(0, 0);
        double initialMass = 1;
        double2x2 initialC = new double2x2(1, 1, 1, 1);
        double2 initialPosition = new(16, 16);
        particle.InitParticle(initialPosition, initialVelocity, initialMass, initialC);
        particles[0, 0] = particle;
        fluidSimulator.SetParticles(particles);
        MlsMpmGrid grid = ScriptableObject.CreateInstance("MlsMpmGrid") as MlsMpmGrid;
        grid.InitMlsMpmGrid(20);
        GridCell cell = grid.At(16, 16);
        double initialCellMass = 0.25;
        cell.SetMass(initialCellMass);
        grid.UpdateCellAt(16, 16, cell);
        fluidSimulator.SetGrid(grid);
        double2 expectedVelocity = new(0.64, 0.64);
        fluidSimulator.ParticleToGridStep2();
        double2 actualVelocity = fluidSimulator.GetGrid().At(0, 0).GetVelocity();
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedVelocity, actualVelocity));
    }

    [Test]
    public void UpdateGridShouldUpdateCellVelocitiesAndEnforceBoundaryConditions()
    {
        FluidSimulator fluidSimulator = GameObject.Find("ExampleGeo").AddComponent<FluidSimulator>();
        int testNeighborDimension = 1;
        fluidSimulator.SetNeighborDimension(testNeighborDimension);
        Particle[,] particles = new Particle[4, 4];
        Particle particle = ScriptableObject.CreateInstance("Particle") as Particle;
        double2 initialVelocity = new(0, 0);
        double initialMass = 1;
        double2x2 initialC = new double2x2(1, 1, 1, 1);
        double2 initialPosition = new(16, 16);
        particle.InitParticle(initialPosition, initialVelocity, initialMass, initialC);
        particles[0, 0] = particle;
        particles[0, 2] = particle;
        fluidSimulator.SetParticles(particles);
        MlsMpmGrid grid = ScriptableObject.CreateInstance("MlsMpmGrid") as MlsMpmGrid;
        grid.InitMlsMpmGrid(64);
        GridCell cell1 = grid.At(0, 0);
        double2 initialVelocityCell = new(0.64, 0.64);
        double initialMassCell = 0.25;
        cell1.SetMass(initialMassCell);
        cell1.SetVelocity(initialVelocityCell);
        GridCell cell2 = grid.At(3, 3);
        cell2.SetMass(initialMassCell);
        cell2.SetVelocity(initialVelocityCell);
        grid.UpdateCellAt(0, 0, cell1);
        grid.UpdateCellAt(3, 3, cell2);
        // Boundary condition is enforced for first
        double2 expectedVelocity1 = new(0, 0);
        // And not for second
        double2 expectedVelocity2 = new(2.56, 2.5);
        fluidSimulator.SetGrid(grid);
        fluidSimulator.UpdateGrid();
        double2 actualVelocity1 = fluidSimulator.GetGrid().At(0, 0).GetVelocity();
        double2 actualVelocity2 = fluidSimulator.GetGrid().At(3, 3).GetVelocity();
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedVelocity1, actualVelocity1));
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedVelocity2, actualVelocity2));

    }

}
