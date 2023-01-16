using UnityEngine;
using Unity.Mathematics;

public class FluidSimulator : MonoBehaviour
{
    private Particle[,] particles;
    private MlsMpmGrid grid;
    // nialltl used 64...delete this comment if 96 works fine
    private int gridResolution = 96;
    private const double timestep = 0.2;
    // should be 5 if timestep is 0.2
    private const int numSimulationsPerUpdate = (int)(1 / timestep);

    public void InitializeGridAndParticleArrays()
    {
        grid = ScriptableObject.CreateInstance("MlsMpmGrid") as MlsMpmGrid;
        grid.InitMlsMpmGrid(gridResolution);
        particles = new Particle[grid.GetGridResolution(), grid.GetGridResolution()];
        double2[,] temporaryParticlePositions = BuildGridOfTemporaryParticlePositions();
        InitializeParticles(temporaryParticlePositions);
    }

    public double2[,] BuildGridOfTemporaryParticlePositions()
    {
        double spacing = 0.5;
        double startPosition = 16;
        double endPosition = 64;
        int tempParticleArrayResolution = 96;
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
        for (int i = 0; i < particles.GetLength(0); i++)
        {
            for (int j = 0; j < particles.GetLength(1); j++)
            {
                Particle particle = particles[i, j];
                double[] particlePosition = GeneralMathUtils.Format2DVectorForMath(particle.GetPosition());
                int[] cellPosition = GeneralMathUtils.ParticlePositionToCellPosition(particlePosition);
                double[] distanceFromParticleToCell = GeneralMathUtils.ComputeDistanceFromParticleToCell(GeneralMathUtils.Format2DVectorForMath(particle.GetPosition()), cellPosition);
                double[][] weights = GeneralMathUtils.ComputeAllWeights(distanceFromParticleToCell);
                double2x2 C = particle.GetAffineMomentumMatrix();
                int neighborDimension = 3;
                for (int nx = 0; nx < neighborDimension; nx++)
                {
                    for (int ny = 0; ny < neighborDimension; ny++)
                    {
                        double weight = GeneralMathUtils.ComputeWeight(weights, nx, ny);
                        int[] neighborPosition = P2G1Math.ComputeNeighborPosition(cellPosition, nx, ny);
                        double[] distanceFromCurrentParticleToCurrentNeighbor = P2G1Math.ComputeDistanceFromCurrentParticleToCurrentNeighbor(neighborPosition, particlePosition);
                        double2 Q = P2G1Math.ComputeQ(C, GeneralMathUtils.Format2DVectorForMath(distanceFromCurrentParticleToCurrentNeighbor));
                        double massContribution = P2G1Math.ComputeMassContribution(weight, particle.GetMass());
                        // note: number of particles = number of cells, since they are controlled by gridResolution
                        GridCell correspondingCell = grid.At(i, j);
                        correspondingCell.SetMass(P2G1Math.RecomputeCellMassAndReturnIt(correspondingCell.GetMass(), massContribution));
                        correspondingCell.SetVelocity(P2G1Math.RecomputeCellVelocityAndReturnIt(massContribution, particle.GetVelocity(), Q));
                        // deep copy
                        grid.UpdateCellAt(i, j, correspondingCell);
                    }
                }
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
                double[] distanceFromParticleToCell = GeneralMathUtils.ComputeDistanceFromParticleToCell(GeneralMathUtils.Format2DVectorForMath(particle.GetPosition()), cellPosition);
                double[][] weights = GeneralMathUtils.ComputeAllWeights(distanceFromParticleToCell);
                /* 
                 * We estimate the particle's volume by summing up the neighborhood's weighted mass contribution.
                 * See Equation 152 of MPM course. 
                 */
                int[] nearestGridCellToParticlePosition = P2G2Math.FindNearestGridCellToParticle(particlePosition);
                GridCell nearestGridCellToParticle = grid.At(nearestGridCellToParticlePosition);
                double mass = nearestGridCellToParticle.GetMass();
                double density = 0;
                int neighborDimension = 3;
                for (int nx = 0; nx < neighborDimension; nx++)
                {
                    for (int ny = 0; ny < neighborDimension; ny++)
                    {
                        double weight = GeneralMathUtils.ComputeWeight(weights, nx, ny);
                        density += P2G2Math.ComputeUpdatedDensity(weight, mass, density);
                    }
                }

            }
        }
    }

    public void GridToParticleStep()
    {

    }

    public void Simulate()
    {
        ClearGrid();
        ParticleToGridStep1();
        ParticleToGridStep2();
        GridToParticleStep();
    }

    private void InitializeParticles(double2[,] temporaryParticlePositions)
    {
        // we could try decreasing the size to 8192 like nialltl does?
        int tempParticleArrayResolution = 96;
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

    private void PutInitialParticlesIntoGame()
    {
        GameObject exampleGeo = GameObject.Find("ExampleGeo");
        GameInterface gameInterface = exampleGeo.GetComponent<GameInterface>();
        gameInterface.DumpParticlesIntoScene(flattenParticles());
    }

    // Start is called before the first frame update
    void Start()
    {
        print("LOG: Starting fluid simulator");
        InitializeGridAndParticleArrays();
        PutInitialParticlesIntoGame();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < numSimulationsPerUpdate; i++)
        {
            Simulate();
        }
    }

    public int GetParticleCount()
    {
        return particles.GetLength(0) + particles.GetLength(1);
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
