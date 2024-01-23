using System.Collections;
using System.Collections.Generic;
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
    private int resolution = 64;
    private const double timestep = 0.2;
    private const int numSimsPerUpdate = (int) (1 / timestep);
    private const double gravity = -9.8;
    private int neighborDimension = 3;
    private GameInterface_3D gameInterface;
    private WaterSurfacer_3D waterSurfacer;
    int iteration = 0;
    private const string geoAttacher = "ExampleGeo";
    // it is a subservient god, for it creates and destroys upon command
    private const string geoGod = "CreatorDestroyer";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /**
     * Steps of MLS-MPM
     */
    public void ClearGrid()
    {
        if (grid == null)
        {
            grid = ScriptableObject.CreateInstance("Grid_3D") as Grid_3D;
        }
        grid.Init(resolution);
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
                    for (int nx = 0; nx < neighborDimension; nx++)
                    {
                        for (int ny = 0; ny < neighborDimension; ny++) 
                        {
                            for (int nz = 0; nz < neighborDimension; nz++)
                            {

                            }
                        }
                    }
                }
            }
        }
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
        grid = ScriptableObject.CreateInstance("Grid_3D") as Grid_3D;
        grid.Init(resolution);
    }

    public void InitParticles()
    {
        particles = new Particle_3D[resolution][][];
        double3[][][] tempParticlePositions = InitTempGrid();
        
        // Fluid at bottom
        for (int i = 0; i < resolution; i++) 
        {
            for (int j = 0; j < resolution; j++)
            {
                for (int k = 0; k < resolution; k++)
                {
                    bool shouldCreateFluidParticle = (j < 3);
                    double3 initialVelocity = new(0);
                    double3x3 initialC = new(0);
                    if (shouldCreateFluidParticle)
                    {
                        FluidParticle_3D p = ScriptableObject.CreateInstance("FluidParticle_3D") as FluidParticle_3D;
                        p.Init(tempParticlePositions[i][j][k], initialVelocity, initialC);
                        particles[i][j][k] = p;
                    }
                    else
                    {
                        AirParticle_3D p = ScriptableObject.CreateInstance("AirParticle_3D") as AirParticle_3D;
                        p.Init(tempParticlePositions[i][j][k], initialVelocity, initialC);
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
        double startPosition = 16;
        double endPosition = 48;
        double3[][][] grid = new double3[resolution][][];
        // Initializing grid
        for (int i = 0; i < resolution; i++)
        {
            grid[i] = new double3[resolution][];
            for (int j = 0; j < resolution; j++)
            {
                grid[i][j] = new double3[resolution];
            }
        }
        int iInt = 0;
        int jInt = 0;
        int kInt = 0;
        for (double i = startPosition; i < endPosition; i += spacing)
        {
            for (double j = startPosition; j < endPosition; j += spacing)
            {
                for (double k = startPosition; k < endPosition; k += spacing)
                {
                    double3 position = new(i, j, k);
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
     * Boolean check functions
     */

    private bool GridSizeIsZero()
    {
        return grid == null || (grid.GetSize()[0] == 0 && grid.GetSize()[1] == 0 && grid.GetSize()[2] == 0);
    }

    private bool ParticlesSizeIsZero()
    {
        return particles == null || particles.Length == 0;
    }
}
