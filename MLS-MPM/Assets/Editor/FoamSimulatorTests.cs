using NUnit.Framework;
using UnityEngine;
using Unity.Mathematics;

[TestFixture]
public class FoamSimulatorTests
{
    [Test]
    public void InitializeFoamSimulatorShouldSetUpGridAndParticles()
    {
        FoamSimulator foamSimulator = GameObject.Find("ExampleGeo").AddComponent<FoamSimulator>();
        foamSimulator.InitializeFoamSimulator();
        int2 expectedGridSize = new(64, 64);
        int expectedNumberOfParticles = 4096;
        int2 actualGridSize = foamSimulator.GetGrid().GetSize();
        int actualNumberOfParticles = foamSimulator.GetParticleCount();
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedGridSize, actualGridSize));
        Assert.AreEqual(expectedNumberOfParticles, actualNumberOfParticles);
    }

    [Test]
    public void InitializeGridShouldInstantiateTheGrid()
    {
        FoamSimulator foamSimulator = GameObject.Find("ExampleGeo").AddComponent<FoamSimulator>();
        foamSimulator.InitializeGrid();
        Assert.IsNotNull(foamSimulator.GetGrid());
        int2 expectedSize = new(64, 64);
        Assert.AreEqual(foamSimulator.GetGrid().GetSize(), expectedSize);
    }

    [Test]
    public void InitializeParticlesWrapperShouldInitialize4096Particles()
    {
        FoamSimulator foamSimulator = GameObject.Find("ExampleGeo").AddComponent<FoamSimulator>();
        foamSimulator.InitializeGrid();
        foamSimulator.InitializeParticles();
        // 16 to 48 with a spacing of 0.5, two dimensions = 4096 particles.
        int expectedParticleCount = 4096;
        Assert.AreEqual(foamSimulator.GetParticleCount(), expectedParticleCount);
        Particle particle = foamSimulator.GetParticles()[0, 0];
        Assert.IsNotNull(particle);
    }

    [Test]
    public void InitializeParticlesShouldAssignSomeParticlesToBeAirAndOthersToBeFluid()
    {
        // To consider: Do we want to hard-lock certain particles so the ratio is guaranteed, or do we want to use the randomizer which should still give us an approximate ratio? Does it matter?
        FoamSimulator foamSimulator = GameObject.Find("ExampleGeo").AddComponent<FoamSimulator>();
        foamSimulator.InitializeGrid();
        foamSimulator.InitializeParticles();
        Particle[,] particles = foamSimulator.GetParticles();
        // consider: checking statistically for ratio 
        Assert.IsTrue(particles[0, 0].GetMass() == 1.0 || particles[0, 0].GetMass() == 0.5);
    }

    [Test]
    public void BuildGridOfTemporaryParticlePositionsShouldCreateEvenlySpacedParticlesFrom16_16To48_48()
    {
        FoamSimulator foamSimulator = GameObject.Find("ExampleGeo").AddComponent<FoamSimulator>();
        double2[,] temporaryParticlePositionGrid = foamSimulator.BuildGridOfTemporaryParticlePositions();
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
        FoamSimulator foamSimulator = GameObject.Find("ExampleGeo").AddComponent<FoamSimulator>();
        // clear grid just re-initializes, so no need for first init call
        foamSimulator.ClearGrid();
        GridCell firstCell = foamSimulator.GetGrid().At(firstCellXPosition, firstCellYPosition);
        GridCell lastCell = foamSimulator.GetGrid().At(lastCellXPosition, lastCellYPosition);
        Assert.AreEqual(firstCell.GetVelocity(), expectedFirstCellVelocity);
        Assert.AreEqual(firstCell.GetMass(), expectedFirstCellMass);
        Assert.AreEqual(lastCell.GetVelocity(), expectedLastCellVelocity);
        Assert.AreEqual(lastCell.GetMass(), expectedLastCellMass);
    }

    [Test]
    public void P2G1ShouldModifyEachGridCellUsingParticleAttributes()
    {
        // Test expected particle masses and velocities
        FoamSimulator foamSimulator = GameObject.Find("ExampleGeo").AddComponent<FoamSimulator>();
        // Let's save ourselves a few loops.
        int testNeighborDimension = 1;
        foamSimulator.SetNeighborDimension(testNeighborDimension);
        Particle[,] particles = new Particle[1, 1];
        Particle particle = ScriptableObject.CreateInstance("Particle") as Particle;
        double2 initialVelocity = new(0, 0);
        double initialMass = 1;
        double2x2 initialC = new(0, 0, 0, 0);
        double2 initialPosition = new(16, 16);
        particle.InitParticle(initialPosition, initialVelocity, initialMass, initialC);
        particles[0, 0] = particle;
        foamSimulator.SetParticles(particles);
        double expectedNewMass = 0;
        double2 expectedNewVelocity = new(0, 0);
        foamSimulator.ParticleToGridStep1();
        GridCell firstCell = foamSimulator.GetGrid().At(0, 0);
        Assert.AreEqual(expectedNewMass, firstCell.GetMass());
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedNewVelocity, firstCell.GetVelocity()));
    }

    [Test]
    public void P2G2ShouldUpdateGridCellVelocityWithMomentum()
    {
        FoamSimulator foamSimulator = GameObject.Find("ExampleGeo").AddComponent<FoamSimulator>();

        int testNeighborDimension = 1;
        foamSimulator.SetNeighborDimension(testNeighborDimension);
        Particle[,] particles = new Particle[1, 1];
        Particle particle = ScriptableObject.CreateInstance("Particle") as Particle;
        double2 initialVelocity = new(0, 0);
        double initialMass = 1;
        double2x2 initialC = new(1, 1, 1, 1);
        double2 initialPosition = new(16, 16);
        particle.InitParticle(initialPosition, initialVelocity, initialMass, initialC);
        particles[0, 0] = particle;
        foamSimulator.SetParticles(particles);
        MlsMpmGrid grid = ScriptableObject.CreateInstance("MlsMpmGrid") as MlsMpmGrid;
        grid.InitMlsMpmGrid(20);
        GridCell cell = grid.At(16, 16);
        double initialCellMass = 0.25;
        cell.SetMass(initialCellMass);
        grid.UpdateCellAt(16, 16, cell);
        GridCell cell2 = grid.At(15, 15);
        cell2.SetMass(initialCellMass);
        grid.UpdateCellAt(15, 15, cell2);
        foamSimulator.SetGrid(grid);
        // Might want to double check on this, modified due to adapting my constitutive equation to Herschel-Bulkley
        double2 expectedVelocity = new(10.6017205683107, 10.6017205683107);
        foamSimulator.ParticleToGridStep2();
        double2 actualVelocity = foamSimulator.GetGrid().At(15, 15).GetVelocity();
        //Debug.Log(actualVelocity);
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedVelocity, actualVelocity));
    }

    [Test]
    public void UpdateGridShouldUpdateCellVelocitiesAndEnforceBoundaryConditions()
    {
        FoamSimulator foamSimulator = GameObject.Find("ExampleGeo").AddComponent<FoamSimulator>();
        int testNeighborDimension = 1;
        foamSimulator.SetNeighborDimension(testNeighborDimension);
        Particle[,] particles = new Particle[4, 4];
        Particle particle = ScriptableObject.CreateInstance("Particle") as Particle;
        double2 initialVelocity = new(0, 0);
        double initialMass = 1;
        double2x2 initialC = new(1, 1, 1, 1);
        double2 initialPosition = new(16, 16);
        particle.InitParticle(initialPosition, initialVelocity, initialMass, initialC);
        particles[0, 0] = particle;
        particles[0, 2] = particle;
        foamSimulator.SetParticles(particles);
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
        double2 expectedVelocity2 = new(2.56, 0.6);
        foamSimulator.SetGrid(grid);
        foamSimulator.UpdateGrid();
        double2 actualVelocity1 = foamSimulator.GetGrid().At(0, 0).GetVelocity();
        double2 actualVelocity2 = foamSimulator.GetGrid().At(3, 3).GetVelocity();
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedVelocity1, actualVelocity1));
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedVelocity2, actualVelocity2));
    }

    [Test]
    public void UpdateCellVelocityWithEnforcedBoundaryConditionsG2PShouldEnforceBoundaries()
    {
        FoamSimulator foamSimulator = GameObject.Find("ExampleGeo").AddComponent<FoamSimulator>();
        double mass = 1.0;
        double2x2 C = new(1, 1, 1, 1);
        double2 particlePosition = new(1, 1);

        // First: Test a particle that won't hit any boundaries
        Particle p1 = ScriptableObject.CreateInstance("Particle") as Particle;
        double2 particleVelocity1 = new(50, 50);
        p1.InitParticle(particlePosition, particleVelocity1, mass, C);
        double2 expectedReturnedVelocity1 = new(50, 50);
        double2 actualReturnedVelocity1 = foamSimulator.UpdateParticleVelocityWithEnforcedBoundaryConditions(p1);
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedReturnedVelocity1, actualReturnedVelocity1));

        // xN.x < wallMin
        Particle p2 = ScriptableObject.CreateInstance("Particle") as Particle;
        double2 particleVelocity2 = new(1, 50);
        p2.InitParticle(particlePosition, particleVelocity2, mass, C);
        double2 expectedReturnedVelocity2 = new(2, 50);
        double2 actualReturnedVelocity2 = foamSimulator.UpdateParticleVelocityWithEnforcedBoundaryConditions(p2);
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedReturnedVelocity2, actualReturnedVelocity2));

        // xN.x > wallMax
        Particle p3 = ScriptableObject.CreateInstance("Particle") as Particle;
        double2 particleVelocity3 = new(150, 50);
        p3.InitParticle(particlePosition, particleVelocity3, mass, C);
        double2 expectedReturnedVelocity3 = new(59, 50);
        double2 actualReturnedVelocity3 = foamSimulator.UpdateParticleVelocityWithEnforcedBoundaryConditions(p3);
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedReturnedVelocity3, actualReturnedVelocity3));

        // xN.y < wallMin
        Particle p4 = ScriptableObject.CreateInstance("Particle") as Particle;
        double2 particleVelocity4 = new(50, 1);
        p4.InitParticle(particlePosition, particleVelocity4, mass, C);
        double2 expectedReturnedVelocity4 = new(50, 2);
        double2 actualReturnedVelocity4 = foamSimulator.UpdateParticleVelocityWithEnforcedBoundaryConditions(p4);
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedReturnedVelocity4, actualReturnedVelocity4));

        // xN.y > wallMax
        Particle p5 = ScriptableObject.CreateInstance("Particle") as Particle;
        double2 particleVelocity5 = new(50, 150);
        p5.InitParticle(particlePosition, particleVelocity5, mass, C);
        double2 expectedReturnedVelocity5 = new(50, 59);
        double2 actualReturnedVelocity5 = foamSimulator.UpdateParticleVelocityWithEnforcedBoundaryConditions(p5);
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedReturnedVelocity5, actualReturnedVelocity5));
    }

    [Test]
    public void GridToParticleStepShouldUpdateParticlePositionVelocityAndAffineMomentumMatrix()
    {
        FoamSimulator foamSimulator = GameObject.Find("ExampleGeo").AddComponent<FoamSimulator>();
        int testNeighborDimension = 1;
        foamSimulator.SetNeighborDimension(testNeighborDimension);

        // Make particles
        Particle[,] particles = new Particle[1, 1];
        Particle particle = ScriptableObject.CreateInstance("Particle") as Particle;
        double2 initialVelocity = new(0, 0);
        double initialMass = 1;
        double2x2 initialC = new(1, 1, 1, 1);
        double2 initialPosition = new(16, 16);
        particle.InitParticle(initialPosition, initialVelocity, initialMass, initialC);
        particles[0, 0] = particle;
        foamSimulator.SetParticles(particles);

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
        foamSimulator.SetGrid(grid);

        // Execute step
        foamSimulator.GridToParticleStep();

        // Expected values
        double2x2 expectedC = new(-1.28, -1.25, -1.28, -1.25);
        double2 expectedVelocity = new(0.64, 0.625);
        double2 expectedPosition = new(16.128, 16.125);

        // Actual values
        Particle firstParticle = foamSimulator.GetParticles()[0, 0];
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
        FoamSimulator foamSimulator = GameObject.Find("ExampleGeo").AddComponent<FoamSimulator>();
        foamSimulator.InitializeFoamSimulator();
        double2 initialVelocity = foamSimulator.GetParticles()[0, 0].GetVelocity();
        double2 initialPosition = foamSimulator.GetParticles()[0, 0].GetPosition();
        double2x2 initialC = foamSimulator.GetParticles()[0, 0].GetAffineMomentumMatrix();
        foamSimulator.Simulate();
        // particle velocity, position, C. 
        double2 finalVelocity = foamSimulator.GetParticles()[0, 0].GetVelocity();
        double2 finalPosition = foamSimulator.GetParticles()[0, 0].GetPosition();
        double2x2 finalC = foamSimulator.GetParticles()[0, 0].GetAffineMomentumMatrix();
        Assert.IsFalse(GeneralMathUtils.DeepEquals(initialVelocity, finalVelocity));
        Assert.IsFalse(GeneralMathUtils.DeepEquals(initialPosition, finalPosition));
        Assert.IsFalse(GeneralMathUtils.DeepEquals(initialC, finalC));
    }

    // Since the first simulate test isn't strict enough
    [Test]
    public void SimulateShouldUpdateParticleAttributesCorrectly()
    {
        FoamSimulator foamSimulator = GameObject.Find("ExampleGeo").AddComponent<FoamSimulator>();
        foamSimulator.InitializeFoamSimulator();
        foamSimulator.Simulate();
        // Can't do an exact number because of the randomness between fluid and air. 
        double2 expectedFinalPosition = new(16, 15);
        double2 actualFinalPosition = foamSimulator.GetParticles()[0, 0].GetPosition();
        Assert.IsTrue(math.abs(expectedFinalPosition[0] - actualFinalPosition[0]) < 1 && math.abs(expectedFinalPosition[1] - actualFinalPosition[1]) < 1);
    }

    [Test]
    public void ShouldBeAirShouldReturnTrueDependingOnProbability()
    {
        FoamSimulator foamSimulator = GameObject.Find("ExampleGeo").AddComponent<FoamSimulator>();
        // First tested probability: 80 air%
        bool expectedAir = foamSimulator.ShouldBeAir(9);
        Assert.IsTrue(expectedAir);
        bool expectedFluid = foamSimulator.ShouldBeAir(1);
        Assert.IsFalse(expectedFluid);
        bool randomWithProbability = foamSimulator.ShouldBeAir();
        Assert.IsNotNull(randomWithProbability);
    }

    [Test]
    public void FluidAndAirParticlesShouldHaveDifferentEndPositions()
    {
        FoamSimulator foamSimulator = GameObject.Find("ExampleGeo").AddComponent<FoamSimulator>();
        foamSimulator.InitializeFoamSimulator();
        foamSimulator.ClearGrid();
        bool shouldCreateAirParticle = true;
        int tempParticleArrayResolution = 64;
        foamSimulator.SetParticles(new Particle[foamSimulator.GetGrid().GetGridResolution(), foamSimulator.GetGrid().GetGridResolution()]);
        double2[,] temporaryParticlePositions = foamSimulator.BuildGridOfTemporaryParticlePositions();
        Particle[,] particles = foamSimulator.GetParticles();
        for (int i = 0; i < tempParticleArrayResolution; i++)
        {
            for (int j = 0; j < tempParticleArrayResolution; j++)
            {
                shouldCreateAirParticle = !shouldCreateAirParticle; // Alternating
                if (i == 19 && j == 24)
                {
                    shouldCreateAirParticle = true;
                }
                double2 initialVelocity = new(0, 0);
                double2x2 initialC = new double2x2(0, 0, 0, 0);
                if (shouldCreateAirParticle)
                {
                    // verify that this is entered
                    AirParticle airParticle = ScriptableObject.CreateInstance("AirParticle") as AirParticle;
                    airParticle.InitParticle(temporaryParticlePositions[i, j], initialVelocity, initialC);
                    particles[i, j] = airParticle;
                }
                else
                {
                    FluidParticle fluidParticle = ScriptableObject.CreateInstance("FluidParticle") as FluidParticle;
                    fluidParticle.InitParticle(temporaryParticlePositions[i, j], initialVelocity, initialC);
                    particles[i, j] = fluidParticle;
                }
                foamSimulator.SetParticles(particles);
            }
        }
        // Initialize Particles
        // foamSimulator.ParticleToGridStep1(); // Actually, we DO want to call this, AS LONG AS we have set the particles beforehand. It won't overwrite anything
        // would normally call .Simulate(), but we're doing it in pieces to have manual control over one particle
        // P2G1: Must do manually, minus that one section
        //foamSimulator.ParticleToGridStep2();
        //foamSimulator.UpdateGrid();
        //foamSimulator.GridToParticleStep();
    }
}
