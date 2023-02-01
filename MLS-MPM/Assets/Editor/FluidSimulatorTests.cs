using NUnit.Framework;
using UnityEngine;
using Unity.Mathematics;

[TestFixture]
public class FluidSimulatorTests
{
    [Test]
    public void InitializeFluidSimulatorShouldSetUpGridAndParticles()
    {
        FluidSimulator fluidSimulator = GameObject.Find("ExampleGeo").AddComponent<FluidSimulator>();
        fluidSimulator.InitializeFluidSimulator();
        int2 expectedGridSize = new(64, 64);
        int expectedNumberOfParticles = 4096;
        int2 actualGridSize = fluidSimulator.GetGrid().GetSize();
        int actualNumberOfParticles = fluidSimulator.GetParticleCount();
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedGridSize, actualGridSize));
        Assert.AreEqual(expectedNumberOfParticles, actualNumberOfParticles);
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
        double2x2 initialC = new(1, 1, 1, 1);
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
        double2x2 initialC = new(1, 1, 1, 1);
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

    [Test]
    public void UpdateCellVelocityWithEnforcedBoundaryConditionsG2PShouldEnforceBoundaries()
    {
        FluidSimulator fluidSimulator = GameObject.Find("ExampleGeo").AddComponent<FluidSimulator>();
        double mass = 1.0;
        double2x2 C = new(1, 1, 1, 1);
        double2 particlePosition = new(1, 1);

        // First: Test a particle that won't hit any boundaries
        Particle p1 = ScriptableObject.CreateInstance("Particle") as Particle;
        double2 particleVelocity1 = new(50, 50);
        p1.InitParticle(particlePosition, particleVelocity1, mass, C);
        double2 expectedReturnedVelocity1 = new(50, 50);
        double2 actualReturnedVelocity1 = fluidSimulator.UpdateCellVelocityWithEnforcedBoundaryConditionsG2P(p1);
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedReturnedVelocity1, actualReturnedVelocity1));

        // xN.x < wallMin
        Particle p2 = ScriptableObject.CreateInstance("Particle") as Particle;
        double2 particleVelocity2 = new(1, 50);
        p2.InitParticle(particlePosition, particleVelocity2, mass, C);
        double2 expectedReturnedVelocity2 = new(2, 50);
        double2 actualReturnedVelocity2 = fluidSimulator.UpdateCellVelocityWithEnforcedBoundaryConditionsG2P(p2);
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedReturnedVelocity2, actualReturnedVelocity2));

        // xN.x > wallMax
        Particle p3 = ScriptableObject.CreateInstance("Particle") as Particle;
        double2 particleVelocity3 = new(150, 50);
        p3.InitParticle(particlePosition, particleVelocity3, mass, C);
        double2 expectedReturnedVelocity3 = new(59, 50);
        double2 actualReturnedVelocity3 = fluidSimulator.UpdateCellVelocityWithEnforcedBoundaryConditionsG2P(p3);
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedReturnedVelocity3, actualReturnedVelocity3));

        // xN.y < wallMin
        Particle p4 = ScriptableObject.CreateInstance("Particle") as Particle;
        double2 particleVelocity4 = new(50, 1);
        p4.InitParticle(particlePosition, particleVelocity4, mass, C);
        double2 expectedReturnedVelocity4 = new(50, 2);
        double2 actualReturnedVelocity4 = fluidSimulator.UpdateCellVelocityWithEnforcedBoundaryConditionsG2P(p4);
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedReturnedVelocity4, actualReturnedVelocity4));

        // xN.y > wallMax
        Particle p5 = ScriptableObject.CreateInstance("Particle") as Particle;
        double2 particleVelocity5 = new(50, 150);
        p5.InitParticle(particlePosition, particleVelocity5, mass, C);
        double2 expectedReturnedVelocity5 = new(50, 59);
        double2 actualReturnedVelocity5 = fluidSimulator.UpdateCellVelocityWithEnforcedBoundaryConditionsG2P(p5);
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedReturnedVelocity5, actualReturnedVelocity5));
    }

    [Test]
    public void GridToParticleStepShouldUpdateParticlePositionVelocityAndAffineMomentumMatrix()
    {
        FluidSimulator fluidSimulator = GameObject.Find("ExampleGeo").AddComponent<FluidSimulator>();
        int testNeighborDimension = 1;
        fluidSimulator.SetNeighborDimension(testNeighborDimension);

        // Make particles
        Particle[,] particles = new Particle[1, 1];
        Particle particle = ScriptableObject.CreateInstance("Particle") as Particle;
        double2 initialVelocity = new(0, 0);
        double initialMass = 1;
        double2x2 initialC = new(1, 1, 1, 1);
        double2 initialPosition = new(16, 16);
        particle.InitParticle(initialPosition, initialVelocity, initialMass, initialC);
        particles[0, 0] = particle;
        fluidSimulator.SetParticles(particles);

        // Make grid
        MlsMpmGrid grid = ScriptableObject.CreateInstance("MlsMpmGrid") as MlsMpmGrid;
        grid.InitMlsMpmGrid(64);
        GridCell cell1 = grid.At(16, 16);
        double2 initialVelocityCell1 = new(0, 0);
        double2 initialVelocityCell2 = new(2.56, 2.5);
        double initialMassCell = 0.25;
        cell1.SetMass(initialMassCell);
        cell1.SetVelocity(initialVelocityCell1);
        GridCell cell2 = grid.At(15, 15);
        cell2.SetMass(initialMassCell);
        cell2.SetVelocity(initialVelocityCell2);
        grid.UpdateCellAt(16, 16, cell1);
        grid.UpdateCellAt(15, 15, cell2);
        fluidSimulator.SetGrid(grid);

        // Execute step
        fluidSimulator.GridToParticleStep();

        // Expected values
        double2x2 expectedC = new(-1.28, -1.25, -1.28, -1.25);
        double2 expectedVelocity = new(0.64, 0.625);
        double2 expectedPosition = new(16.128, 16.125);

        // Actual values
        Particle firstParticle = fluidSimulator.GetParticles()[0, 0];
        double2x2 actualC = firstParticle.GetAffineMomentumMatrix();
        double2 actualVelocity = firstParticle.GetVelocity();
        double2 actualPosition = firstParticle.GetPosition();

        //Equality tests
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedC, actualC));
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedVelocity, actualVelocity));
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedPosition, actualPosition));
    }

    [Test]
    public void SimulateShouldUpdateParticleAttributes()
    {
        FluidSimulator fluidSimulator = GameObject.Find("ExampleGeo").AddComponent<FluidSimulator>();
        fluidSimulator.InitializeFluidSimulator();
        double2 initialVelocity = fluidSimulator.GetParticles()[0, 0].GetVelocity();
        double2 initialPosition = fluidSimulator.GetParticles()[0, 0].GetPosition();
        double2x2 initialC = fluidSimulator.GetParticles()[0, 0].GetAffineMomentumMatrix();
        fluidSimulator.Simulate();
        // particle velocity, position, C. 
        double2 finalVelocity = fluidSimulator.GetParticles()[0, 0].GetVelocity();
        double2 finalPosition = fluidSimulator.GetParticles()[0, 0].GetPosition();
        double2x2 finalC = fluidSimulator.GetParticles()[0, 0].GetAffineMomentumMatrix();
        Assert.IsFalse(GeneralMathUtils.DeepEquals(initialVelocity, finalVelocity));
        Assert.IsFalse(GeneralMathUtils.DeepEquals(initialPosition, finalPosition));
        Assert.IsFalse(GeneralMathUtils.DeepEquals(initialC, finalC));

        // Step through the entire process.
    }
}
