using UnityEngine;
using Unity.Mathematics;

/**
 * Fluid Simulator: Simulate a basic liquid. 
 * Foam simulation will be done differently, in the Foam Simulator class.
 * Source: Nialltl's incremental MPM repository and corresponding tutorial.
 */

public class FluidSimulator : MonoBehaviour
{
    private Particle[,] particles;
    private MlsMpmGrid grid;
    // 64 x 64
    private int gridResolution = 64;
    private const double timestep = 0.2;
    // should be 5 if timestep is 0.2
    private const int numSimulationsPerUpdate = (int)(1 / timestep);
    private const double dynamicViscosity = 0.1;
    private const double restDensity = 4;
    private const double eosStiffness = 10;
    private const double eosPower = 4;
    private const double gravity = -0.3;
    private const int neighborDimension = 3;

    public void InitializeFluidSimulator()
    {
        InitializeGrid();
        InitializeParticles();
        PutNewParticlesIntoGame();
    }

    public void InitializeGrid()
    {
        grid = ScriptableObject.CreateInstance("MlsMpmGrid") as MlsMpmGrid;
        grid.InitMlsMpmGrid(gridResolution);
    }

    public void InitializeParticles()
    {
        particles = new Particle[grid.GetGridResolution(), grid.GetGridResolution()];
        double2[,] temporaryParticlePositions = BuildGridOfTemporaryParticlePositions();
        InitializeParticles(temporaryParticlePositions);
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

    public void ParticleToGridStep1()
    {
        // Particle array must be initialized and populated with position + velocity for each particle, before calling this step. Specifically, call InitializeParticles(), no parameters.
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
                        double[] distanceFromCurrentParticleToCurrentNeighbor = P2G1Math.ComputeDistanceFromCurrentParticleToCurrentNeighbor(neighborPosition, particlePosition);
                        double2 Q = P2G1Math.ComputeQ(C, distanceFromCurrentParticleToCurrentNeighbor);
                        double massContribution = P2G1Math.ComputeMassContribution(weight, particle.GetMass());
                        // note: number of particles = number of cells, since they are controlled by gridResolution
                        GridCell correspondingCell = grid.At(i, j);
                        correspondingCell.SetMass(P2G1Math.RecomputeCellMassAndReturnIt(correspondingCell.GetMass(), massContribution));
                        correspondingCell.SetVelocity(P2G1Math.RecomputeCellVelocityAndReturnIt(massContribution, particle.GetVelocity(), Q));
                        // deep copy
                        grid.UpdateCellAt(i, j, correspondingCell);
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
                int[] nearestGridCellToParticlePosition = P2G2Math.FindNearestGridCellToParticle(particlePosition);
                GridCell nearestGridCellToParticle = grid.At(nearestGridCellToParticlePosition);
                double mass = nearestGridCellToParticle.GetMass();
                double density = 0;
                for (int nx = 0; nx < neighborDimension; nx++)
                {
                    for (int ny = 0; ny < neighborDimension; ny++)
                    {
                        double weight = GeneralMathUtils.ComputeWeight(weights, nx, ny);
                        density += P2G2Math.ComputeUpdatedDensity(weight, mass, density);
                    }
                }
                double volume = P2G2Math.ComputeVolume(mass, density);
                double pressure = P2G2Math.ComputePressure(eosStiffness, density, restDensity, eosPower);
                double2x2 stress = P2G2Math.CreateStressMatrix(pressure);
                double2x2 strain = P2G2Math.InitializeStrainMatrix(particle.GetAffineMomentumMatrix());
                double trace = P2G2Math.ComputeTrace(strain);
                strain = P2G2Math.UpdateStrainAndReturnUnityMatrix(strain, trace);
                double2x2 viscosity = P2G2Math.ComputeViscosity(strain, dynamicViscosity);
                stress = P2G2Math.UpdateStress(stress, viscosity);
                double2x2 equation16Term0 = P2G2Math.ComputeEquation16Term0(stress, volume, timestep);
                for (int nx = 0; nx < neighborDimension; nx++)
                {
                    for (int ny = 0; ny < neighborDimension; ny++)
                    {
                        double weight = GeneralMathUtils.ComputeWeight(weights, nx, ny);
                        int[] neighborPosition = GeneralMathUtils.ComputeNeighborPosition(cellPosition, nx, ny);
                        double[] distanceFromCellToNeighbor = P2G2Math.ComputeDistanceFromCellToNeighbor(neighborPosition, cellPosition);
                        GridCell correspondingCell = grid.At(i, j);
                        double2 momentum = P2G2Math.ComputeMomentum(equation16Term0, weight, distanceFromCellToNeighbor);
                        double2 updatedVelocity = P2G2Math.UpdateCellVelocity(momentum, correspondingCell.GetVelocity());
                        correspondingCell.SetVelocity(updatedVelocity);
                        // deep copy
                        grid.UpdateCellAt(i, j, correspondingCell);
                    }
                }
                particles[i, j] = particle;
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
                    double2 updatedCellVelocityWithEnforcedBoundaryConditions = UpdateCellVelocityWithEnforcedBoundaryConditions(i, currentCell.GetVelocity());
                    currentCell.SetVelocity(updatedCellVelocityWithEnforcedBoundaryConditions);
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
                particle.SetVelocity(0);
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
                        double[] distanceFromCellToNeighbor = P2G2Math.ComputeDistanceFromCellToNeighbor(neighborPosition, cellPosition);
                        double[] neighborCellVelocity = GeneralMathUtils.Format2DVectorForMath(grid.At(neighborPosition).GetVelocity());
                        double[] weightedVelocity = G2PMath.ComputeWeightedVelocity(neighborCellVelocity, weight);
                        double[,] term = G2PMath.ComputeTerm(weightedVelocity, distanceFromCellToNeighbor);
                        B = G2PMath.ComputeUpdatedB(B, term);
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
                double2 boundaryConditionEnforcedVelocity = UpdateCellVelocityWithEnforcedBoundaryConditionsG2P(particle);
                particle.SetVelocity(boundaryConditionEnforcedVelocity);
                particles[i, j] = particle;
            }
        }
    }

    public void Simulate()
    {
        ClearGrid();
        ParticleToGridStep1();
        ParticleToGridStep2();
        UpdateGrid();
        GridToParticleStep();
    }

    private double2 UpdateCellVelocityWithEnforcedBoundaryConditionsG2P(Particle particle)
    {
        double2 updatedVelocity = particle.GetVelocity();
        double2 xN = particle.GetPosition() + particle.GetVelocity();
        const double wallMin = 3;
        double wallMax = gridResolution - 4;
        double particleVelocityX = particle.GetVelocity().x;
        double particleVelocityY = particle.GetVelocity().y;
        if (xN.x < wallMin)
        {
            particle.UpdateVelocityX(particleVelocityX + (wallMin - xN.x));
        }
        if (xN.x > wallMax)
        {
            particle.UpdateVelocityX(particleVelocityX + (wallMax - xN.x));
        }
        if (xN.y < wallMin)
        {
            particle.UpdateVelocityX(particleVelocityY + (wallMin - xN.y));
        }
        if (xN.y > wallMax)
        {
            particle.UpdateVelocityX(particleVelocityY + (wallMax - xN.y));
        }
        return updatedVelocity;
    }

    private double2 ClampPosition(Particle particle)
    {
        return math.clamp(particle.GetPosition(), 1, gridResolution - 2);
    }

    private double2 UpdateCellVelocityWithEnforcedBoundaryConditions(int i, double2 velocity)
    {
        double2 updatedVelocity = velocity;
        int x = i / gridResolution;
        int y = i % gridResolution;
        if (x < 2 || x > gridResolution - 3)
        {
            velocity.x = 0;
        }
        if (y < 2 || y > gridResolution - 3)
        {
            velocity.y = 0;
        }
        return updatedVelocity;
    }

    private void InitializeParticles(double2[,] temporaryParticlePositions)
    {
        int tempParticleArrayResolution = 64;
        for (int i = 0; i < tempParticleArrayResolution; i++)
        {
            for (int j = 0; j < tempParticleArrayResolution; j++)
            {
                Particle particle = ScriptableObject.CreateInstance("Particle") as Particle;
                double2 initialVelocity = new(0, 0);
                double initialMass = 1;
                double2x2 initialC = new double2x2(0, 0, 0, 0);
                particle.InitParticle(temporaryParticlePositions[i, j], initialVelocity, initialMass, initialC);
                particles[i, j] = particle;
            }
        }
    }

    private Particle[] flattenParticles()
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

    private void PutNewParticlesIntoGame()
    {
        Debug.Log("Adding particles");
        GameObject exampleGeo = GameObject.Find("ExampleGeo");
        GameInterface gameInterface = exampleGeo.GetComponent<GameInterface>();
        gameInterface.DumpParticlesIntoScene(flattenParticles());
        //gameInterface.AddAllParticles();
    }

    private void RefreshGameParticles()
    {
        Debug.Log("Nuking particles");
        // Note: We still have a list of particles here. Shouldn't those be cleared each time?
        //particles = new Particle[grid.GetGridResolution(), grid.GetGridResolution()];
        //double2[,] temporaryParticlePositions = BuildGridOfTemporaryParticlePositions();
        //InitializeParticles(temporaryParticlePositions);
        GameObject exampleGeo = GameObject.Find("ExampleGeo");
        GameInterface gameInterface = exampleGeo.GetComponent<GameInterface>();
        gameInterface.NukeAllParticles();
    }

    // Start is called before the first frame update
    void Start()
    {
        print("LOG: Starting fluid simulator");
        InitializeGrid();
        PutNewParticlesIntoGame();
    }

    // Update is called once per frame
    void Update()
    {
        RefreshGameParticles();

        for (int i = 0; i < numSimulationsPerUpdate; i++)
        {
            Simulate();
        }
        // TODO: Bug: particles either don't delete or constantly do, or remain motionless
        PutNewParticlesIntoGame();
        // this will cause null errors
        // particles = new Particle[gridResolution, gridResolution];
    }

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
}
