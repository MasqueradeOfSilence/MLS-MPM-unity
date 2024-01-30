using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

/**
 * FFF_3D: Simulates a 3D foam with a small pool of water at the bottom.
 *  Uses the Foam Fraction Flow method. 
 */

public class FFF_3D : MonoBehaviour
{
    /**
     * Data members
     */
    private Particle_3D[][][] particles;
    private Grid_3D grid;
    private readonly int resolution = 16; // Was 64 for 2D
    private readonly int zResolution = 4; // Experimenting with lower resolution for Z
    private const double timestep = 0.2;
    private const int numSimsPerUpdate = (int) (1 / timestep);
    private const double gravity = -9.8;
    private readonly int neighborDimension = 3;
    private readonly int zNeighborDimension = 3;
    private GameInterface_3D gameInterface;
    private WaterSurfacer_3D waterSurfacer;
    int iteration = 0;
    private const string geoAttacher = "ExampleGeo";
    // it is a subservient god, for it creates and destroys upon command
    private const string geoGod = "CreatorDestroyer";
    private int numUpdates = 1;
    private bool shouldStopEarly = false; // Set to TRUE only for debug purposes

    // Start is called before the first frame update
    void Start()
    {
        Init();
        gameInterface.DumpParticlesIntoScene(GetFlattenedParticleList().ToArray(), true);
    }

    // Update is called once per frame
    void Update()
    {
        //return;
        if (shouldStopEarly && numUpdates > 1)
        {
            numUpdates++;
            return;
        }
        for (int i = 0; i < numSimsPerUpdate; i++)
        {
            Simulate();
        }
        gameInterface.UpdateParticles(GetFlattenedParticleList().ToArray(), true);
        gameInterface.NukeClones();
        numUpdates++;
    }

    /**
     * High-level simulation logic
     */
    public void Simulate()
    {
        ClearGrid();
        //return;
        ParticleToGridStep1();
        ParticleToGridStep2();
        UpdateGrid();
        GridToParticleStep();
        if (iteration == 1)
        {
            Debug.Log("Foam simulator beginning!");
            DetermineBubbleSizes();
        }
        if (waterSurfacer != null)
        {
            //waterSurfacer.InitializeFluidSurface(particles);
        }
        ComputeVoronoi();
        iteration++;
    }

    /**
     * Steps of MLS-MPM
     */
    public void ClearGrid()
    {
        if (grid == null)
        {
            grid = GeometryCreator_3D.CreateNewGrid(resolution, zResolution);
        }
        else
        {
            grid.Init(resolution, zResolution);
        }
    }

    public void ParticleToGridStep1()
    {
        if (GridSizeIsZero())
        {
            InitGrid();
        }
        if (ParticlesSizeIsZero())
        {
            InitParticles();
        }
        for (int i = 0; i < particles.Length; i++)
        {
            for (int j = 0; j < particles[i].Length; j++)
            {
                for (int k = 0; k < particles[i][j].Length; k++)
                {
                    Particle_3D p = particles[i][j][k];
                    double3 particlePosition = p.GetPosition();
                    int3 cellPosition = MathUtils_3D.ParticlePositionToCellPosition(particlePosition);
                    double3 distanceFromParticleToCell = MathUtils_3D.ComputeDistanceFromParticleToCell(particlePosition, cellPosition);
                    List<double3> weights = MathUtils_3D.ComputeAllWeights(distanceFromParticleToCell);
                    double3x3 C = p.GetC();
                    for (int nx = 0; nx < neighborDimension; nx++)
                    {
                        for (int ny = 0; ny < neighborDimension; ny++) 
                        {
                            for (int nz = 0; nz < zNeighborDimension; nz++)
                            {
                                double mass = p.GetMass();
                                double3 velocity = p.GetVelocity();
                                double weight = MathUtils_3D.ComputeWeight(weights, nx, ny, nz);
                                int3 neighborPosition = MathUtils_3D.ComputeNeighborPosition(cellPosition, nx, ny, nz);
                                double3 distanceFromParticleToNeighbor = MathUtils_3D.ComputeDistanceFromParticleToNeighbor(neighborPosition, particlePosition);
                                double3 Q = MathUtils_3D.ComputeQ(C, distanceFromParticleToNeighbor);
                                // Mass contribution of neighbor
                                double massContribution = MathUtils_3D.ComputeMassContribution(weight, mass);
                                Debug.Log("GRID.AT E " + neighborPosition);
                                // 16, 16, 31 cellPosition at 0, 0, 2
                                Debug.Log("OTHERS: " + cellPosition + ", " + nx + ", " + ny + ", " + nz + ", " + particlePosition);
                                Cell_3D correspondingCell = grid.At(neighborPosition);
                                double updatedMass = MathUtils_3D.UpdateMass(mass, massContribution);
                                correspondingCell.SetMass(updatedMass);
                                double3 updatedVelocity = MathUtils_3D.UpdateVelocity(massContribution, velocity, Q, correspondingCell.GetVelocity());
                                correspondingCell.SetVelocity(updatedVelocity);
                                grid.UpdateCellAt(neighborPosition, correspondingCell);
                            }
                        }
                        particles[i][j][k] = p;
                    }
                }
            }
        }
    }

    public void ParticleToGridStep2()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            for (int j = 0; j < particles[i].Length; j++)
            {
                for (int k = 0; k < particles[i][j].Length; k++)
                {
                    // Obtaining requisite data
                    Particle_3D p = particles[i][j][k];
                    double3 particlePosition = p.GetPosition();
                    int3 cellPosition = MathUtils_3D.ParticlePositionToCellPosition(particlePosition);
                    double3 distanceFromParticleToCell = MathUtils_3D.ComputeDistanceFromParticleToCell(particlePosition, cellPosition);
                    List<double3> weights = MathUtils_3D.ComputeAllWeights(distanceFromParticleToCell);

                    double density = 0;

                    for (int nx = 0; nx < neighborDimension; nx++)
                    {
                        for (int ny = 0; ny < neighborDimension; ny++)
                        {
                            for (int nz = 0; nz < zNeighborDimension; nz++)
                            {
                                double weight = MathUtils_3D.ComputeWeight(weights, nx, ny, nz);
                                int3 cellCoordinates = MathUtils_3D.ComputeNeighborPosition(cellPosition, nx, ny, nz);
                                Debug.Log("GRID.AT A: " + cellCoordinates);
                                Cell_3D nearestCellToParticle = grid.At(cellCoordinates);
                                double mass = nearestCellToParticle.GetMass();
                                density = MathUtils_3D.UpdateDensity(weight, mass, density);
                            }
                        }
                    }
                    double volume = MathUtils_3D.ComputeVolume(p.GetMass(), density);
                    double3x3 strain = p.GetC();
                    double trace = MathUtils_3D.ComputeTrace(strain);
                    strain.c0.z = strain.c1.y = strain.c2.x = trace;

                    // Herschel-Bulkley
                    double yieldStress_T0 = 0.319;
                    double viscosity_mu = 2.72;
                    double flowIndex_n = 0.22;
                    double eosStiffness = 19.6;
                    double restDensity = 3.108;
                    int eosPower = 2;
                    double smallestValue = double.MaxValue;
                    // Mapping
                    for (int l = 0; l < 2; l++)
                    {
                        for (int m = 0; m < 2; m++)
                        {
                            double currentValue = strain[l][m];
                            if (currentValue < smallestValue)
                            {
                                smallestValue = currentValue;
                            }
                        }
                    }
                    double extraOffset = 0;
                    if (smallestValue < 0)
                    {
                        extraOffset = 0.001;
                        smallestValue = math.abs(smallestValue);
                        extraOffset += smallestValue;
                        for (int row = 0; row < 2; row++)
                        {
                            for (int col = 0; col < 2; col++)
                            {
                                strain[row][col] += extraOffset;
                            }
                        }
                    }
                    double3x3 herschelBulkleyStress = MathUtils_3D.ComputeHerschelBulkleyStress(yieldStress_T0,
                        strain, viscosity_mu, flowIndex_n, eosStiffness, density, restDensity, eosPower, extraOffset);
                    double3x3 equation16Term0 = MathUtils_3D.ComputeEquation16Term0(herschelBulkleyStress, volume, timestep);
                    for (int nx = 0; nx < neighborDimension; nx++)
                    {
                        for (int ny = 0; ny < neighborDimension; ny++)
                        {
                            for (int nz = 0; nz < zNeighborDimension; nz++)
                            {
                                double weight = MathUtils_3D.ComputeWeight(weights, nx, ny, nz);
                                int3 neighborPosition = MathUtils_3D.ComputeNeighborPosition(cellPosition, nx, ny, nz);
                                double3 distanceFromParticleToNeighbor = MathUtils_3D.ComputeDistanceFromParticleToNeighbor(neighborPosition, particlePosition);
                                Debug.Log("GRID.AT B " + neighborPosition);
                                Cell_3D correspondingCell = grid.At(neighborPosition);
                                double3 momentum = MathUtils_3D.ComputeMomentum(equation16Term0, weight, distanceFromParticleToNeighbor);
                                double3 updatedVelocity = MathUtils_3D.AddMomentumToVelocity(momentum, correspondingCell.GetVelocity());
                                correspondingCell.SetVelocity(updatedVelocity);
                                grid.UpdateCellAt(neighborPosition, correspondingCell);
                            }
                        }
                    }
                }
                // T-T <~ holy crap that's a lot of curly braces
            }
        }
    }

    public void UpdateGrid()
    {
        for (int i = 0; i < resolution; i++) 
        {
            for (int j =  0; j < resolution; j++) 
            {
                for (int k = 0; k < zResolution; k++)
                {
                    int3 position = new(i, j, k);
                    Debug.Log("GRID.AT C " + position);
                    Cell_3D cell = grid.At(position);
                    if (cell.GetMass() > 0)
                    {
                        cell.SetVelocity(cell.GetVelocity() / cell.GetMass());
                        cell.SetVelocity(cell.GetVelocity() + (timestep * new double3(0, gravity, 0)));
                        double3 updatedVelocityWithBoundary = UpdateVelocityWithBoundary(i, j, k, cell.GetVelocity());
                        cell.SetVelocity(updatedVelocityWithBoundary);
                        grid.UpdateCellAt(position, cell);
                    }
                }
            }
        }
    }

    private double3 UpdateVelocityWithBoundary(int i, int j, int k, double3 velocity)
    {
        double3 velocityWithBoundary = velocity;
        if (i < 2 || i > resolution - 3)
        {
            velocityWithBoundary.x = 0;
        }
        if (j < 2 || j > resolution - 3)
        {
            velocityWithBoundary.y = 0;
        }
        if (k < 2 || k > zResolution - 3)
        {
            velocityWithBoundary.z = 0;
        }
        return velocityWithBoundary;
    }

    public void GridToParticleStep()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            for (int j = 0; j < particles[i].Length; j++)
            {
                for (int k = 0; k < particles[i][j].Length; k++)
                {
                    Particle_3D p = particles[i][j][k];
                    double3 particlePosition = p.GetPosition();
                    p.ResetVelocity();
                    int3 cellPosition = MathUtils_3D.ParticlePositionToCellPosition(particlePosition);
                    double3 distanceFromParticleToCell = MathUtils_3D.ComputeDistanceFromParticleToCell(particlePosition, cellPosition);
                    List<double3> weights = MathUtils_3D.ComputeAllWeights(distanceFromParticleToCell);
                    // APIC matrix, see equation 8 of MLS-MPM paper
                    double3x3 B = 0;
                    for (int nx = 0; nx < neighborDimension; nx++) 
                    {
                        for (int ny = 0; ny < neighborDimension; ny++) 
                        {
                            for (int nz = 0; nz < zNeighborDimension; nz++)
                            {
                                double weight = MathUtils_3D.ComputeWeight(weights, nx, ny, nz);
                                int3 neighborPosition = MathUtils_3D.ComputeNeighborPosition(cellPosition, nx, ny, nz);
                                double3 distanceFromParticleToNeighbor = MathUtils_3D.ComputeDistanceFromParticleToNeighbor(neighborPosition, particlePosition);
                                Debug.Log("GRID.AT D " + neighborPosition);
                                Cell_3D neighborCell = grid.At(neighborPosition);
                                double3 neighborVelocity = neighborCell.GetVelocity();
                                double3 weightedVelocity = MathUtils_3D.ComputeWeightedVelocity(neighborVelocity, weight);
                                double3x3 term = MathUtils_3D.ComputeTerm(weightedVelocity, distanceFromParticleToNeighbor);
                                B = MathUtils_3D.UpdateB(B, term);
                                double3 updatedVelocity = MathUtils_3D.AddWeightedVelocity(p.GetVelocity(), weightedVelocity);
                                p.SetVelocity(updatedVelocity);
                            }
                        }
                    }
                    double3x3 updatedC = MathUtils_3D.RecomputeCMatrix(B);
                    p.SetC(updatedC);
                    // Move particle
                    double3 advectedPosition = MathUtils_3D.AdvectParticle(p.GetPosition(), p.GetVelocity(), timestep);
                    p.SetPosition(advectedPosition);
                    // Clamp
                    double3 clampedPosition = ClampPosition(p);
                    p.SetPosition(clampedPosition);
                    Debug.Log("Clamped position: " + clampedPosition);
                    // Enforce boundaries
                    double3 boundedVelocity = EnforceBoundaryVelocity(p);
                    p.SetVelocity(boundedVelocity);
                    particles[i][j][k] = p;
                }
            }
        }
    }

    private double3 ClampPosition(Particle_3D p)
    {
        return math.clamp(p.GetPosition(), 1, resolution - 2);
    }

    private double3 ClampPosition(double3 p)
    {
        return math.clamp(p, 1, resolution - 2);
    }

    public double3 EnforceBoundaryVelocity(Particle_3D p)
    {
        double3 velocity = p.GetVelocity();
        double3 xN = p.GetPosition() + velocity;
        const double wallMin = 3;
        double wallMax = resolution - 4;
        if (xN.x < wallMin)
        {
            velocity.x += (wallMin - xN.x);
        }
        if (xN.x > wallMax)
        {
            velocity.x += (wallMax - xN.x);
        }
        if (xN.y < wallMin)
        {
            velocity.y += (wallMin - xN.y);
        }
        if (xN.y > wallMax)
        {
            velocity.y += (wallMax - xN.y);
        }
        if (xN.z < wallMin)
        {
            velocity.z += (wallMin - xN.z);
        }
        if (xN.z > wallMax)
        {
            velocity.z += (wallMax - xN.z);
        }

        return velocity;
    }

    /**
     * Bubble functionality
     */
    private void DetermineBubbleSizes()
    {
        List<Particle_3D> flatParticleList = GetFlattenedParticleList();
        for (int i = 0; i < particles.Length; i++)
        {
            for (int j = 0; j < particles[i].Length; j++)
            {
                for (int k = 0; k < particles[i][j].Length; k++)
                {
                    bool skipBubble = false;
                    Particle_3D p = particles[i][j][k];
                    if ((i % 2 == 0 || j % 2 == 0 || k % 2 == 0) && MathUtils_3D.IsAir(p))
                    {
                        skipBubble = true;
                    }
                    System.Random random = new();
                    double randomValue = random.NextDouble();
                    if (randomValue < 0.5 && MathUtils_3D.IsAir(p))
                    {
                        skipBubble = true;
                    }
                    if (skipBubble)
                    {
                        p.SetBubble(-200, true);
                    }
                    else
                    {
                        double volumeFraction = VolumeFractionUtils_3D.ComputeVolumeFraction(flatParticleList, p);
                        p.SetBubble(volumeFraction);
                    }
                }
            }
        }
    }

    private List<Particle_3D> GetFlattenedParticleList()
    {
        return particles.SelectMany(array2D => array2D.SelectMany(array1D => array1D)).ToList();
    }

    /**
     * Setup (Init) functions
     */
    public void Init()
    {
        InitGrid();
        InitParticles();
        InitGameCommunication();
    }

    public void InitGrid()
    {
        grid = GeometryCreator_3D.CreateNewGrid(resolution, zResolution);
    }

    public void InitParticles()
    {
        particles = new Particle_3D[resolution][][];
        double3[][][] tempParticlePositions = InitTempGrid();
        
        // Fluid at bottom
        for (int i = 0; i < resolution; i++) 
        {
            particles[i] = new Particle_3D[resolution][];
            for (int j = 0; j < resolution; j++)
            {
                particles[i][j] = new Particle_3D[zResolution];
                for (int k = 0; k < zResolution; k++)
                {
                    bool shouldCreateFluidParticle = (j < 3);
                    double3 initialVelocity = new(0);
                    double3x3 initialC = new(0);
                    if (shouldCreateFluidParticle)
                    {
                        FluidParticle_3D p = GeometryCreator_3D.CreateNewFluidParticle(tempParticlePositions[i][j][k], initialVelocity, initialC);
                        particles[i][j][k] = p;
                    }
                    else
                    {
                        AirParticle_3D p = GeometryCreator_3D.CreateNewAirParticle(tempParticlePositions[i][j][k], initialVelocity, initialC);
                        particles[i][j][k] = p;
                    }
                }
            }
        }
    }

    private void InitGameCommunication()
    {
        gameInterface = GameObject.Find(geoGod).AddComponent<GameInterface_3D>();
        waterSurfacer = GameObject.Find(geoAttacher).AddComponent<WaterSurfacer_3D>();
    }

    public double3[][][] InitTempGrid()
    {
        double spacing = 0.5;
        double startPosition = resolution / 4;
        double endPosition = startPosition + resolution * spacing;
        double startPositionZ = zResolution / 4;
        double endPositionZ = startPositionZ + zResolution * spacing;
        double3[][][] grid = new double3[resolution][][];
        // Initializing grid
        for (int i = 0; i < resolution; i++)
        {
            grid[i] = new double3[resolution][];
            for (int j = 0; j < resolution; j++)
            {
                grid[i][j] = new double3[zResolution];
            }
        }
        int iInt = 0;
        int jInt = 0;
        int kInt = 0;
        for (double i = startPosition; i < endPosition; i += spacing)
        {
            for (double j = startPosition; j < endPosition; j += spacing)
            {
                for (double k = startPositionZ; k < endPositionZ; k += spacing)
                {
                    double3 position = new(i, j, k);
                    position = ClampPosition(position); // So it doesn't init at 32
                    grid[iInt][jInt][kInt] = position;
                    kInt++;
                }
                kInt = 0;
                jInt++;
            }
            jInt = 0;
            iInt++;
        }
        return grid;
    }

    /**
     * Voronoi
     */
    private void ComputeVoronoi()
    {
        VoronoiShaderDTO_3D dto = ScriptableObject.CreateInstance<VoronoiShaderDTO_3D>();
        //return;
        dto.Init(GetFlattenedParticleList());
        dto.UpdateVoronoiTexture();
    }

    /**
     * Boolean check functions
     */

    private bool GridSizeIsZero()
    {
        return grid == null || (grid.GetSize() == 0);
    }

    private bool ParticlesSizeIsZero()
    {
        return particles == null || particles.Length == 0;
    }
}
