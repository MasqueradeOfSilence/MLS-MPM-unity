using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;

/**
 * Fluid Simulator: Simulate a basic liquid. 
 * Foam simulation will be done differently, in the Foam Simulator class.
 * Source: Nialltl's incremental MPM repository and corresponding tutorial.
 */

public class FoamSimulator : MonoBehaviour
{
    private Particle[,] particles;
    private MlsMpmGrid grid;
    // 64 x 64
    private int gridResolution = 64;
    private const double timestep = 0.2;
    // should be 5 if timestep is 0.2
    private const int numSimulationsPerUpdate = (int)(1 / timestep);
    private const double gravity = -9.8;
    private int neighborDimension = 3;
    private GameInterface gameInterface;
    int iteration = 0;

    public enum WhereToPutFluid
    {
        RANDOMLY_THROUGHOUT,
        ON_BOTTOM,
        ON_TOP
    };
    WhereToPutFluid fluidPositioning = WhereToPutFluid.ON_BOTTOM;

    public void InitializeFoamSimulator()
    {
        InitializeGrid();
        InitializeParticles();
        gameInterface = GameObject.Find("CreatorDestroyer").AddComponent<GameInterface>();
    }

    public void InitializeGrid()
    {
        grid = ScriptableObject.CreateInstance("MlsMpmGrid") as MlsMpmGrid;
        grid.InitMlsMpmGrid(gridResolution);
    }

    public void InitializeParticles()
    {
        if (fluidPositioning == WhereToPutFluid.ON_TOP)
        {
            InitializeParticlesWithFluidOnTop();
        }
        else if (fluidPositioning == WhereToPutFluid.ON_BOTTOM)
        {
            InitializeParticlesWithFluidAtBottom();
        }
        else
        {
            particles = new Particle[grid.GetGridResolution(), grid.GetGridResolution()];
            double2[,] temporaryParticlePositions = BuildGridOfTemporaryParticlePositions();
            InitializeParticles(temporaryParticlePositions);
        }
    }

    public void InitializeParticlesWithFluidOnTop()
    {
        particles = new Particle[grid.GetGridResolution(), grid.GetGridResolution()];
        double2[,] temporaryParticlePositions = BuildGridOfTemporaryParticlePositions();
        InitializeParticlesWithFluidOnTop(temporaryParticlePositions);
    }

    public void InitializeParticlesWithFluidAtBottom()
    {
        particles = new Particle[grid.GetGridResolution(), grid.GetGridResolution()];
        double2[,] temporaryParticlePositions = BuildGridOfTemporaryParticlePositions();
        InitializeParticlesWithFluidAtBottom(temporaryParticlePositions);
    }

    public double2[,] BuildGridOfTemporaryParticlePositions()
    {
        double spacing = 0.5;
        double startPosition = 16;
        double endPosition = 48;
        int tempParticleArrayResolution = 64;
        double2[,] gridOfTemporaryParticlePositions = new double2[tempParticleArrayResolution, tempParticleArrayResolution];
        int iInt = 0;
        int jInt = 0;
        for (double i = startPosition; i < endPosition; i+= spacing)
        {
            for (double j = startPosition; j < endPosition; j += spacing)
            {
                double2 position = new(i, j);
                gridOfTemporaryParticlePositions[iInt, jInt] = position;
                jInt++;
            }
            jInt = 0;
            iInt++;
        }
        return gridOfTemporaryParticlePositions;
    }

    public void ClearGrid()
    {
        if (grid == null)
        {
            grid = ScriptableObject.CreateInstance("MlsMpmGrid") as MlsMpmGrid;
        }
        grid.InitMlsMpmGrid(gridResolution);
    }

    private bool GridSizeIsZero()
    {
        return grid == null || (grid.GetSize()[0] == 0 && grid.GetSize()[1] == 0);
    }

    private bool ParticlesSizeIsZero()
    {
        return particles == null || particles.Length == 0;
    }

    public void ParticleToGridStep1()
    {
        if (GridSizeIsZero())
        {
            InitializeGrid();
        }
        // Particle array must be initialized and populated with position + velocity for each particle for this function to perform correctly.
        if (ParticlesSizeIsZero())
        {
            InitializeParticles();
        }
        for (int i = 0; i < particles.GetLength(0); i++)
        {
            for (int j = 0; j < particles.GetLength(1); j++)
            {
                Particle particle = particles[i, j];
                double[] particlePosition = GeneralMathUtils.Format2DVectorForMath(particle.GetPosition());
                int[] cellPosition = GeneralMathUtils.ParticlePositionToCellPosition(particlePosition);
                double[] distanceFromParticleToCell = GeneralMathUtils.ComputeDistanceFromParticleToCell(particlePosition, cellPosition);
                double[][] weights = GeneralMathUtils.ComputeAllWeights(distanceFromParticleToCell);
                double2x2 C = particle.GetAffineMomentumMatrix();
                for (int nx = 0; nx < neighborDimension; nx++)
                {
                    for (int ny = 0; ny < neighborDimension; ny++)
                    {
                        double weight = GeneralMathUtils.ComputeWeight(weights, nx, ny);
                        int[] neighborPosition = GeneralMathUtils.ComputeNeighborPosition(cellPosition, nx, ny);
                        double[] distanceFromCurrentParticleToCurrentNeighbor = GeneralMathUtils.ComputeDistanceFromCurrentParticleToCurrentNeighbor(neighborPosition, particlePosition);
                        double2 Q = P2G1Math.ComputeQ(C, distanceFromCurrentParticleToCurrentNeighbor);
                        double massContribution = P2G1Math.ComputeMassContribution(weight, particle.GetMass());
                        // note: number of particles = number of cells, since they are controlled by gridResolution
                        GridCell correspondingCell = grid.At(neighborPosition[0], neighborPosition[1]);
                        correspondingCell.SetMass(P2G1Math.RecomputeCellMassAndReturnIt(correspondingCell.GetMass(), massContribution));
                        correspondingCell.SetVelocity(P2G1Math.RecomputeCellVelocityAndReturnIt(massContribution, particle.GetVelocity(), Q, correspondingCell.GetVelocity()));
                        // deep copy, see above comment
                        grid.UpdateCellAt(neighborPosition[0], neighborPosition[1], correspondingCell);
                    }
                }
                particles[i, j] = particle;
            }
        }
    }

    public void ParticleToGridStep2()
    {
        for (int i = 0; i < particles.GetLength(0); i++)
        {
            for (int j = 0; j < particles.GetLength(1); j++)
            {
                Particle particle = particles[i, j];
                double[] particlePosition = GeneralMathUtils.Format2DVectorForMath(particle.GetPosition());
                int[] cellPosition = GeneralMathUtils.ParticlePositionToCellPosition(particlePosition);
                double[] distanceFromParticleToCell = GeneralMathUtils.ComputeDistanceFromParticleToCell(particlePosition, cellPosition);
                double[][] weights = GeneralMathUtils.ComputeAllWeights(distanceFromParticleToCell);
                /* 
                 * We estimate the particle's volume by summing up the neighborhood's weighted mass contribution.
                 * See Equation 152 of MPM course. 
                 */
                double density = 0;
                for (int nx = 0; nx < neighborDimension; nx++)
                {
                    for (int ny = 0; ny < neighborDimension; ny++)
                    {
                        double weight = GeneralMathUtils.ComputeWeight(weights, nx, ny);
                        int[] pertinentGridCellCoordinates = GeneralMathUtils.ComputeNeighborPosition(cellPosition, nx, ny);
                        GridCell nearestGridCellToParticle = grid.At(pertinentGridCellCoordinates);
                        double mass = nearestGridCellToParticle.GetMass();
                        density = P2G2Math.ComputeUpdatedDensity(weight, mass, density);
                    }
                }
                double volume = P2G2Math.ComputeVolume(particle.GetMass(), density);
                double2x2 strain = P2G2Math.InitializeStrainMatrix(particle.GetAffineMomentumMatrix());
                double trace = P2G2Math.ComputeTrace(strain);
                strain.c0.y = strain.c1.x = trace;
                // Herschel-Bulkley stress here. 
                // If T0 = 0 and n = 1, it should reduce to a Newtonian fluid.
                // with n = 0.22 like in the columbia paper -> we have negative strain values that get raised to a decimal. thus we use a mapping and reverse-mapping function to avoid the imaginary plane
                // Columbia paper values and Rutgers/UPenn paper values (https://par.nsf.gov/servlets/purl/10290710) have to be scaled down to match Niall's. 

                // The true H-B values
                double yieldStress_T0 = 0.319;// 3.19;// was 31.9; // try 0.319;
                double viscosity_mu = 2.72; // was 27.2
                double flowIndex_n = 0.22; // pinching stops with correct strain add
                double eosStiffness = 19.6;//10.9;// was 109;
                double restDensity = 3.108;//0.3108;//3.2175; // in comparison to water //7.77; // was 77.7; I am trying a proportion with water being 1000 kg/m^3 and 4 in Niall's code. 
                int eosPower = 2; // was 7. If restDensity is too low then anything above 4 will break the sim. I tried it at 4. Experimenting still. 

                // will abstract this mapping function out
                double smallestValue = double.MaxValue;
                for (int k = 0; k < 2; k++)
                {
                    for (int l = 0; l < 2; l++)
                    {
                        double currentValue = strain[k][l];
                        if (currentValue < smallestValue)
                        {
                            smallestValue = currentValue;
                        }    
                    }
                }
                double extraOffset = 0;
                if (smallestValue < 0)
                {
                    // then we need to offset
                    extraOffset = 0.001;
                    smallestValue = math.abs(smallestValue);
                    extraOffset += smallestValue;
                    //strain += extraOffset; // note: this will produce weird behavior. do not do this.
                    for (int row = 0; row < 2; row++)
                    {
                        for (int col = 0; col < 2; col++)
                        {
                            strain[row][col] += extraOffset;
                        }
                    }
                }
                // reverse mapping is currently taken care of inside of here, will clean up
                double2x2 herschelBulkleyStress = P2G2Math.ComputeHerschelBulkleyStress(yieldStress_T0, strain, viscosity_mu, flowIndex_n, eosStiffness, density, restDensity, eosPower, extraOffset);
                //Debug.Log("H-B stress: " + herschelBulkleyStress);
                double2x2 equation16Term0 = P2G2Math.ComputeEquation16Term0(herschelBulkleyStress, volume, timestep);
                for (int nx = 0; nx < neighborDimension; nx++)
                {
                    for (int ny = 0; ny < neighborDimension; ny++)
                    {
                        double weight = GeneralMathUtils.ComputeWeight(weights, nx, ny);
                        int[] neighborPosition = GeneralMathUtils.ComputeNeighborPosition(cellPosition, nx, ny);
                        double[] distanceFromParticleToNeighbor = GeneralMathUtils.ComputeDistanceFromCurrentParticleToCurrentNeighbor(neighborPosition, particlePosition);
                        GridCell correspondingCell = grid.At(neighborPosition[0], neighborPosition[1]);
                        double2 momentum = P2G2Math.ComputeMomentum(equation16Term0, weight, distanceFromParticleToNeighbor);
                        // Update cell velocity by adding momentum to it.
                        double2 updatedVelocity = P2G2Math.UpdateCellVelocity(momentum, correspondingCell.GetVelocity());
                        correspondingCell.SetVelocity(updatedVelocity);
                        grid.UpdateCellAt(neighborPosition[0], neighborPosition[1], correspondingCell);
                    }
                }
            }
        }
    }

    public void UpdateGrid()
    {
        for (int i = 0; i < gridResolution; i++)
        {
            for (int j = 0; j < gridResolution; j++)
            {
                GridCell currentCell = grid.At(i, j);
                if (currentCell.GetMass() > 0)
                {
                    currentCell.SetVelocity(currentCell.GetVelocity() / currentCell.GetMass());
                    currentCell.SetVelocity(currentCell.GetVelocity() + (timestep * new double2(0, gravity)));
                    double2 updatedCellVelocityWithEnforcedBoundaryConditions = UpdateCellVelocityWithEnforcedBoundaryConditions(i, j, currentCell.GetVelocity());
                    currentCell.SetVelocity(updatedCellVelocityWithEnforcedBoundaryConditions);
                    grid.UpdateCellAt(i, j, currentCell);
                }
            }
        }
    }

    public void GridToParticleStep()
    {
        for (int i = 0; i < particles.GetLength(0); i++)
        {
            for (int j = 0; j < particles.GetLength(1); j++) 
            {
                Particle particle = particles[i, j];
                // Reset particle velocity
                particle.SetVelocity(new double2(0, 0));
                double[] particlePosition = GeneralMathUtils.Format2DVectorForMath(particle.GetPosition());
                int[] cellPosition = GeneralMathUtils.ParticlePositionToCellPosition(particlePosition);
                double[] distanceFromParticleToCell = GeneralMathUtils.ComputeDistanceFromParticleToCell(particlePosition, cellPosition);
                double[][] weights = GeneralMathUtils.ComputeAllWeights(distanceFromParticleToCell);
                // APIC matrix (equation 8)
                double2x2 B = 0;
                for (int nx = 0; nx < neighborDimension; nx++)
                {
                    for (int ny = 0; ny < neighborDimension; ny++)
                    {
                        double weight = GeneralMathUtils.ComputeWeight(weights, nx, ny);
                        int[] neighborPosition = GeneralMathUtils.ComputeNeighborPosition(cellPosition, nx, ny);
                        double[] distanceFromParticleToNeighbor = GeneralMathUtils.ComputeDistanceFromCurrentParticleToCurrentNeighbor(neighborPosition, particlePosition);
                        double[] neighborCellVelocity = GeneralMathUtils.Format2DVectorForMath(grid.At(neighborPosition).GetVelocity());
                        double[] weightedVelocity = G2PMath.ComputeWeightedVelocity(neighborCellVelocity, weight);
                        double[,] term = G2PMath.ComputeTerm(weightedVelocity, distanceFromParticleToNeighbor);
                        B = G2PMath.ComputeUpdatedB(B, term);
                        // Because we're transferring the grid velocity to the particle velocity.
                        double[] updatedVelocity = G2PMath.ComputeUpdatedParticleVelocity(particle.GetVelocity(), weightedVelocity);
                        particle.SetVelocity(updatedVelocity);
                    }
                }
                double2x2 updatedC = G2PMath.RecomputeCMatrix(B);
                particle.SetAffineMomentumMatrix(updatedC);
                double[] updatedPosition = G2PMath.AdvectParticle(particle.GetPosition(), particle.GetVelocity(), timestep);
                particle.SetPosition(updatedPosition);
                double2 clampedPosition = ClampPosition(particle);
                particle.SetPosition(clampedPosition);
                double2 boundaryConditionEnforcedVelocity = UpdateParticleVelocityWithEnforcedBoundaryConditions(particle);
                particle.SetVelocity(boundaryConditionEnforcedVelocity);
                particles[i, j] = particle;
            }
        }
    }

    public void DetermineBubbleSizes()
    {
        List<Particle> particlesList = new List<Particle>();
        foreach (Particle p in particles)
        {
            particlesList.Add(p);
        }

        foreach (Particle p in particles)
        {
            double volumeFraction = VolumeFractionCalculator.CalculateVolumeFractionForParticleAtPosition(particlesList, p);
            Debug.Log("VOLUME FRACTION: " + volumeFraction);
        }
    }

    public void Simulate()
    {
        print("RUNNING FOAM SIMULATOR");
        ClearGrid();
        ParticleToGridStep1();
        ParticleToGridStep2();
        UpdateGrid();
        GridToParticleStep();
        // Putting after first computations for now
        if (iteration == 0)
        {
            DetermineBubbleSizes();
        }
        iteration++;
    }

    public double2 UpdateParticleVelocityWithEnforcedBoundaryConditions(Particle particle)
    {
        double2 updatedVelocity = particle.GetVelocity();
        double2 xN = particle.GetPosition() + particle.GetVelocity();
        const double wallMin = 3;
        double wallMax = gridResolution - 4;
        if (xN.x < wallMin)
        {
            updatedVelocity.x += (wallMin - xN.x);
        }
        if (xN.x > wallMax)
        {
            updatedVelocity.x += (wallMax - xN.x);
        }
        if (xN.y < wallMin)
        {
            updatedVelocity.y += (wallMin - xN.y);
        }
        if (xN.y > wallMax)
        {
            updatedVelocity.y += (wallMax - xN.y);
        }
        return updatedVelocity;
    }

    private double2 ClampPosition(Particle particle)
    {
        return math.clamp(particle.GetPosition(), 1, gridResolution - 2);
    }

    private double2 UpdateCellVelocityWithEnforcedBoundaryConditions(int i, int j, double2 velocity)
    {
        double2 updatedVelocity = velocity;

        if (i < 2 || i > gridResolution - 3)
        {
            updatedVelocity.x = 0;
        }
        if (j < 2 || j > gridResolution - 3)
        {
            updatedVelocity.y = 0;
        }
        return updatedVelocity;
    }

    private void InitializeParticlesWithFluidOnTop(double2[,] temporaryParticlePositions)
    {
        int tempParticleArrayResolution = 64;
        for (int i = 0; i < tempParticleArrayResolution; i++)
        {
            for (int j = 0; j < tempParticleArrayResolution; j++)
            {
                // Top rows only
                bool shouldCreateFluidParticle = (j > tempParticleArrayResolution - 3);
                double2 initialVelocity = new(0, 0);
                double2x2 initialC = new double2x2(0, 0, 0, 0);
                if (shouldCreateFluidParticle)
                {
                    FluidParticle fluidParticle = ScriptableObject.CreateInstance("FluidParticle") as FluidParticle;
                    fluidParticle.InitParticle(temporaryParticlePositions[i, j], initialVelocity, initialC);
                    particles[i, j] = fluidParticle;
                }
                else
                {
                    AirParticle airParticle = ScriptableObject.CreateInstance("AirParticle") as AirParticle;
                    airParticle.InitParticle(temporaryParticlePositions[i, j], initialVelocity, initialC);
                    particles[i, j] = airParticle;
                }
            }
        }
    }

    private void InitializeParticlesWithFluidAtBottom(double2[,] temporaryParticlePositions)
    {
        int tempParticleArrayResolution = 64;
        for (int i = 0; i < tempParticleArrayResolution; i++)
        {
            for (int j = 0; j < tempParticleArrayResolution; j++)
            {
                // Top rows only
                bool shouldCreateFluidParticle = (j < 3);
                double2 initialVelocity = new(0, 0);
                double2x2 initialC = new double2x2(0, 0, 0, 0);
                if (shouldCreateFluidParticle)
                {
                    FluidParticle fluidParticle = ScriptableObject.CreateInstance("FluidParticle") as FluidParticle;
                    fluidParticle.InitParticle(temporaryParticlePositions[i, j], initialVelocity, initialC);
                    particles[i, j] = fluidParticle;
                }
                else
                {
                    AirParticle airParticle = ScriptableObject.CreateInstance("AirParticle") as AirParticle;
                    airParticle.InitParticle(temporaryParticlePositions[i, j], initialVelocity, initialC);
                    particles[i, j] = airParticle;
                }
            }
        }
    }

    private void InitializeParticles(double2[,] temporaryParticlePositions)
    {
        int tempParticleArrayResolution = 64;
        for (int i = 0; i < tempParticleArrayResolution; i++)
        {
            for (int j = 0; j < tempParticleArrayResolution; j++)
            {
                // Need to create either AirParticle or FluidParticle here. Probability: Consider 80-20 so we actually have some smaller bubbles. 
                // Consider color coding by volume fraction.
                bool shouldCreateAirParticle = ShouldBeAir(GeneralMathUtils.Random1To10());
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
            }
        }
    }

    private Particle[] FlattenParticles()
    {
        Particle[] toReturn = new Particle[gridResolution * gridResolution];
        int index = 0;
        for (int i = 0; i < particles.GetLength(0); i++)
        {
            for (int j = 0; j < particles.GetLength(1); j++)
            {
                toReturn[index] = particles[i, j];
                index++;
            }
        }
        return toReturn;
    }

    public bool ShouldBeAir(int value = 0)
    {
        if (value == 0)
        {
            return ShouldBeAir(GeneralMathUtils.Random1To10());
        }
        // 80% probability if < 2, 90% if < 1
        return value > 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        print("Starting foam simulator");
        InitializeFoamSimulator();
        gameInterface.DumpParticlesIntoScene(FlattenParticles());
    }
    // test variable for stopping early -- will need to clean up when finished
    int testMe = 0;

    // Update is called once per frame
    void Update()
    {
        //if (testMe == 1)
        //{
        //    Debug.Log("stopping...");
        //}
        //if (testMe > 1)
        //{
        //    // test
        //    testMe++;
        //    return;
        //}
        // Each frame, run x simulations, then update the position of each particle
        for (int i = 0; i < numSimulationsPerUpdate; i++)
        {
            Simulate();
        }
        gameInterface.UpdateParticles(FlattenParticles());
        gameInterface.NukeClones();
        testMe++;
    }

    // Getters and Setters
    public int GetParticleCount()
    {
        return particles.GetLength(0) * particles.GetLength(1);
    }

    public Particle[,] GetParticles()
    {
        return particles;
    }

    public MlsMpmGrid GetGrid()
    {
        return grid;
    }

    public WhereToPutFluid GetFluidPositioning()
    {
        return this.fluidPositioning;
    }

    public void SetFluidPositioning(WhereToPutFluid fluidPositioning)
    {
        this.fluidPositioning = fluidPositioning;
    }

    public void SetParticles(Particle[,] particles)
    {
        this.particles = particles;
    }

    public void SetGrid(MlsMpmGrid grid)
    {
        this.grid = grid;
    }

    // For testing purposes
    public void SetNeighborDimension(int neighborDimension)
    {
        this.neighborDimension = neighborDimension;
    }

    public void SetGridResolution(int gridResolution)
    {
        this.gridResolution = gridResolution;
    }
}
