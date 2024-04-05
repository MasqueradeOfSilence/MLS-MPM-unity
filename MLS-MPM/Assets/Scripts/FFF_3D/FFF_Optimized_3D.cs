using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using System.Diagnostics;

/**
 * Goal: Fully optimize FFF for 3D so it doesn't freeze Unity.
 */

public class FFF_Optimized_3D : MonoBehaviour
{
    /**
    * Data members
    */
    private Particle_3D[] particles;
    private Grid_3D grid;
    private readonly int resolution = 16; // Was 64 for 2D
    private const double timestep = 0.2;
    private const int numSimsPerUpdate = (int)(1 / timestep);
    private const double gravity = -9.8;
    private int neighborDimension = 3;
    private GameInterface_3D gameInterface;
    private WaterSurfacer_3D waterSurfacer;
    int iteration = 0;
    private const string geoAttacher = "ExampleGeo";
    // it is a subservient god, for it creates and destroys upon command
    private const string geoGod = "CreatorDestroyer";
    private int numUpdates = 1;
    private int howManyStepsToTest = 1; // usually set to 1
    private bool runLimitedUpdates = false; // Set to TRUE only for debug purposes
    private bool onlyDisplayInitialSetup = false; // Set to TRUE only for debug purposes
    private bool onlySimOnce = false; // Set to TRUE only for debug
    private bool haveSimmedOnce = false; // No touchy
    private bool renderWater = true;
    private bool alembicEnabled = false;

    // Computational Performance Metrics
    private float elapsedTime = 0f;
    private float updateTime = 0f;
    private DateTime startTime;
    private int numUpdatesForMetrics = 8;
    private List<float> updateTimes = new();
    string timestamp = "";

    public enum SimType
    {
        defaultSim, jacuzzi, foamingSoap, laundryDetergent, bubbleBath
    }

    // For now, change depending on what sim type you want
    private readonly SimType simType = SimType.laundryDetergent;
    private bool started = false;

    // Start is called before the first frame update
    //void Start()
    IEnumerator Start()
    {
        yield return new WaitForSeconds(3);
        started = true;
        startTime = DateTime.Now;
        Init();
        bool shouldUseFFFShader = true;
        bool shouldUseWhiteShader = false;
        if (simType == SimType.jacuzzi || simType == SimType.foamingSoap || simType == SimType.laundryDetergent || simType == SimType.bubbleBath)
        {
            shouldUseFFFShader = false;
            shouldUseWhiteShader = true;
        }
        bool allFluid = false;
        if (simType == SimType.foamingSoap)
        {
            // should this boolean be called allFoam?
            allFluid = true;
        }
        gameInterface.DumpParticlesIntoScene(particles, shouldUseFFFShader, shouldUseWhiteShader, allFluid); 
        gameInterface.NukeClones();
        if (!alembicEnabled)
        {
            EnableAlembic();
            alembicEnabled = true;
        }
        timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
    }

    // Update is called once per frame
    void Update()
    {
        if (!started)
        {
            return;
        }
        //RunATestOnShading();
        if (onlyDisplayInitialSetup)
        {
            return;
        }
        if (haveSimmedOnce && onlySimOnce)
        {
            return;
        }
        if (runLimitedUpdates && numUpdates > howManyStepsToTest)
        {
            numUpdates++;
            return;
        }
        // Performance metrics
        CalculatePerformanceMetrics();
        for (int i = 0; i < numSimsPerUpdate; i++)
        {
            int numTimesToSim = 2;
            if (onlySimOnce && i == numTimesToSim)
            {
                haveSimmedOnce = true;
                return;
            }
            Simulate();
        }
        bool useFFF = true;
        bool useWhite = false;
        if (simType == SimType.jacuzzi || simType == SimType.foamingSoap || simType == SimType.laundryDetergent || simType == SimType.bubbleBath)
        {
            useFFF = false;
            useWhite = true;
        }
        gameInterface.UpdateParticles(particles, useFFF, useWhite);
        gameInterface.NukeClones();
        if (numUpdates <= numUpdatesForMetrics)
        {
            ExportFluidParticleDataToCSVForAlembic(BuildListOfFluidParticlesOnly());
        }
        numUpdates++;
    }

    public static GameObject FindGameObjectsAll(string name) =>
        Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == name);

    private void EnableAlembic()
    {
        if (renderWater && simType != SimType.foamingSoap)
        {
            string abcName = simType.ToString();
            if (abcName == "defaultSim")
            {
                abcName = "defaultFoam";
            }
            abcName += "2";
            GameObject abcInScene = FindGameObjectsAll(abcName);
            if (abcInScene == null)
            {
                UnityEngine.Debug.Log("Error: ABC " + abcName + " not found");
            }
            else
            {
                abcInScene.SetActive(true);
            }
        }
    }

    private Particle_3D[] BuildListOfFluidParticlesOnly()
    {
        List<Particle_3D> fluidParticlesOnly = new();
        foreach (Particle_3D p in particles)
        {
            if (p.GetBubble().GetBubbleSize() == Bubble_3D.BubbleSize.SKIP ||
                p.GetBubble().GetBubbleSize() == Bubble_3D.BubbleSize.MICROSCOPIC)
            {
                fluidParticlesOnly.Add(p);
            }
        }
        return fluidParticlesOnly.ToArray();
    }

    private void ExportFluidParticleDataToCSVForAlembic(Particle_3D[] fluidParticlesOnly)
    {
        CSVExporter csvExporter = ScriptableObject.CreateInstance<CSVExporter>();
        // Note: Updates do not necessarily correlate to frames, but we are starting with that
        csvExporter.ExportParticleDataToCSV(fluidParticlesOnly, numUpdates, timestamp, numUpdatesForMetrics, simType.ToString());
    }

    private float GetNow()
    {
        return (float)(DateTime.Now - startTime).TotalSeconds;
    }

    private void CalculatePerformanceMetrics()
    {
        // Time
        if (numUpdates <= numUpdatesForMetrics)
        {
            updateTime = GetNow();
            elapsedTime += updateTime;
            updateTimes.Add(updateTime);
        }
        if (numUpdates == numUpdatesForMetrics)
        {
            float total = 0f;
            UnityEngine.Debug.Log("Count: " + updateTimes.Count);
            UnityEngine.Debug.Assert(numUpdatesForMetrics == updateTimes.Count);
            for (int i = 0; i < numUpdatesForMetrics; i++)
            {
                total += updateTimes.ElementAt(i);
            }
            float averageUpdateTime = total / numUpdatesForMetrics;
            string totalTimeInfo = "Total time to run " + numUpdatesForMetrics + " updates: " + elapsedTime + " seconds";
            string averageTimeInfo = "Average time per update: " + averageUpdateTime + " seconds";
            UnityEngine.Debug.Log(totalTimeInfo);
            UnityEngine.Debug.Log(averageTimeInfo);
            StreamWriter sw;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                sw = File.CreateText(@"c:\Users\alexc\School_Repos\MLS-MPM-unity\MLS-MPM\Assets\Resources\metrics_" + simType.ToString() + ".txt");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                sw = File.CreateText(@"/Users/Alex/Documents/Alex's Crap/Escuela/MS/Winter_2023/MLS-MPM-unity/MLS-MPM/Assets/Resources/metrics_" + simType.ToString() + ".txt");
            }
            else
            {
                // Linux
                sw = File.CreateText(@"~/Desktop/metrics_" + simType.ToString() + ".txt");
            }
            sw?.WriteLine("Computational Metrics for " + simType.ToString() + ": ");
            sw?.WriteLine(totalTimeInfo);
            sw?.WriteLine(averageTimeInfo);
            sw?.Close();
        }
        // CPU, GPU, RAM will not be done programmatically at this time. Instead we will be looking at the Unity profiler.
    }

    /**
     * High-level simulation logic
     */
    public void Simulate()
    {
        ClearGrid();
        ParticleToGridStep1();
        ParticleToGridStep2();
        UpdateGrid();
        GridToParticleStep();
        if (iteration == 1)
        {
            UnityEngine.Debug.Log("Foam simulator beginning!");
            DetermineBubbleSizes();
        }
        if (waterSurfacer != null && simType != SimType.foamingSoap)
        {
            //waterSurfacer.InitializeFluidSurface(particles, resolution);
        }
        ComputeVoronoi();
        iteration++;
    }

    private void RunATestOnShading()
    {
        GameObject cube = GameObject.Find("Cube");
        Material material = cube.GetComponent<Renderer>().sharedMaterial;
        List<float> testNumbers = new()
        {
            7f,
            1924f
        };
        material.SetFloatArray("_TestNumbers", testNumbers);
        material.SetInteger("_CountMe", 2);
    }

    private bool IsSphere1611Round1(double3 positionBeforeModification)
    {
        double3 target = new(7, 6, 9.5);
        double3 pos = positionBeforeModification;
        return (pos.x == target.x && pos.y == target.y && pos.z == target.z);
    }

    private bool ApproxEquals(double3 target, double3 pos)
    {
        double threshold = 0.01;
        double3 lowerBound = target - threshold;
        double3 upperBound = target + threshold;
        return pos.x > lowerBound.x && pos.x < upperBound.x
            && pos.y > lowerBound.y && pos.y < upperBound.y
            && pos.z > lowerBound.z && pos.z < upperBound.z;
    }

    /**
     * Steps of MLS-MPM
     */
    public void ClearGrid()
    {
        grid = GeometryCreator_3D.CreateNewGrid(resolution);
    }

    private void P2G1NeighborAlgorithm(int nx, int ny, int nz, Particle_3D p, List<double3> weights, double3x3 C, double3 particlePosition, int3 cellPosition)
    {
        double mass = p.GetMass();
        double3 velocity = p.GetVelocity();
        double weight = MathUtils_3D.ComputeWeight(weights, nx, ny, nz);
        int3 neighborPosition = MathUtils_3D.ComputeNeighborPosition(cellPosition, nx, ny, nz);
        double3 distanceFromParticleToNeighbor = MathUtils_3D.ComputeDistanceFromParticleToNeighbor(neighborPosition, particlePosition);
        double3 Q = MathUtils_3D.ComputeQ(C, distanceFromParticleToNeighbor);
        // Mass contribution of neighbor
        double massContribution = MathUtils_3D.ComputeMassContribution(weight, mass);
        Cell_3D correspondingCell = grid.At(neighborPosition);
        // Need to pass in cell mass, not particle mass
        double cellMass = correspondingCell.GetMass();
        double updatedMass = MathUtils_3D.UpdateMass(cellMass, massContribution);
        correspondingCell.SetMass(updatedMass);
        double3 updatedVelocity = MathUtils_3D.UpdateVelocity(massContribution, velocity, Q, correspondingCell.GetVelocity());
        correspondingCell.SetVelocity(updatedVelocity);
        grid.UpdateCellAt(neighborPosition, correspondingCell);
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
            Particle_3D p = particles[i];
            double3 particlePosition = p.GetPosition();
            int3 cellPosition = MathUtils_3D.ParticlePositionToCellPosition(particlePosition);
            double3 distanceFromParticleToCell = MathUtils_3D.ComputeDistanceFromParticleToCell(particlePosition, cellPosition);
            List<double3> weights = MathUtils_3D.ComputeAllWeights(distanceFromParticleToCell);
            double3x3 C = p.GetC();
            //int nx = 0;
            //int ny = 0;
            //int nz = 0;
            //P2G1NeighborAlgorithm(nx, ny, nz, p, weights, C, particlePosition, cellPosition); // We can eliminate the triple-for here if desired
            for (int nx = 0; nx < neighborDimension; nx++)
            {
                for (int ny = 0; ny < neighborDimension; ny++)
                {
                    for (int nz = 0; nz < neighborDimension; nz++)
                    {
                        P2G1NeighborAlgorithm(nx, ny, nz, p, weights, C, particlePosition, cellPosition);
                    }
                }
            }
            particles[i] = p;
        }
    }

    private double P2G2NeighborAlgorithm1(int nx, int ny, int nz, List<double3> weights, double density, int3 cellPosition)
    {
        double weight = MathUtils_3D.ComputeWeight(weights, nx, ny, nz);
        int3 cellCoordinates = MathUtils_3D.ComputeNeighborPosition(cellPosition, nx, ny, nz);
        Cell_3D nearestCellToParticle = grid.At(cellCoordinates);
        double mass = nearestCellToParticle.GetMass();
        density = MathUtils_3D.UpdateDensity(weight, mass, density);
        return density;
    }

    private void P2G2NeighborAlgorithm2(int nx, int ny, int nz, List<double3> weights, int3 cellPosition, double3 particlePosition, double3x3 equation16Term0)
    {
        double weight = MathUtils_3D.ComputeWeight(weights, nx, ny, nz);
        int3 neighborPosition = MathUtils_3D.ComputeNeighborPosition(cellPosition, nx, ny, nz);
        double3 distanceFromParticleToNeighbor = MathUtils_3D.ComputeDistanceFromParticleToNeighbor(neighborPosition, particlePosition);
        Cell_3D correspondingCell = grid.At(neighborPosition);
        double3 savedVelocityInitialForTesting = correspondingCell.GetVelocity(); // Where does this velocity get weird from?
        double3 momentum = MathUtils_3D.ComputeMomentum(equation16Term0, weight, distanceFromParticleToNeighbor);
        double3 updatedVelocity = MathUtils_3D.AddMomentumToVelocity(momentum, correspondingCell.GetVelocity());
        correspondingCell.SetVelocity(updatedVelocity);
        grid.UpdateCellAt(neighborPosition, correspondingCell);
    }

    public void ParticleToGridStep2()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            Particle_3D p = particles[i];
            double3 particlePosition = p.GetPosition();
            int3 cellPosition = MathUtils_3D.ParticlePositionToCellPosition(particlePosition);
            double3 distanceFromParticleToCell = MathUtils_3D.ComputeDistanceFromParticleToCell(particlePosition, cellPosition);
            List<double3> weights = MathUtils_3D.ComputeAllWeights(distanceFromParticleToCell);
            double density = 0;
            for (int nx = 0; nx < neighborDimension; nx++)
            {
                for (int ny = 0; ny < neighborDimension; ny++)
                {
                    for (int nz = 0; nz < neighborDimension; nz++)
                    {
                        density = P2G2NeighborAlgorithm1(nx, ny, nz, weights, density, cellPosition);
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
            for (int l = 0; l < 3; l++)
            {
                for (int m = 0; m < 3; m++)
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
                for (int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 3; col++)
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
                    for (int nz = 0; nz < neighborDimension; nz++)
                    {
                        P2G2NeighborAlgorithm2(nx, ny, nz, weights, cellPosition, particlePosition, equation16Term0);
                    }
                }
            }
        }
    }

    public void UpdateGrid()
    {
        int gridSize = grid.GetResolution() * grid.GetResolution() * grid.GetResolution();
        for (int i = 0; i < gridSize; i++)
        {
            int x = i / (resolution * resolution);
            int y = (i / resolution) % resolution;
            int z = i % resolution;
            int3 testIndex = new(x, y, z);
            Cell_3D cell = grid.At(i); // Using i and testIndex is the same thing.
            if (cell.GetMass() > 0)
            {
                cell.SetVelocity(cell.GetVelocity() / cell.GetMass());
                cell.SetVelocity(cell.GetVelocity() + (timestep * new double3(0, gravity, 0)));
                double3 updatedVelocityWithBoundary = UpdateCellVelocityWithBoundary(x, y, z, cell.GetVelocity());
                cell.SetVelocity(updatedVelocityWithBoundary);
                grid.UpdateCellAt(i, cell);
            }
        }
    }

    private double3 UpdateCellVelocityWithBoundary(int i, int j, int k, double3 velocity)
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
        if (k < 2 || k > resolution - 3)
        {
            velocityWithBoundary.z = 0;
        }
        return velocityWithBoundary;
    }

    public void GridToParticleStep()
    {
        double3 originalPosition;
        for (int i = 0; i < particles.Length; i++)
        {
            Particle_3D p = particles[i];
            double3 particlePosition = p.GetPosition();
            originalPosition = p.GetPosition(); // Just for debug purposes
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
                    for (int nz = 0; nz < neighborDimension; nz++)
                    {
                        double weight = MathUtils_3D.ComputeWeight(weights, nx, ny, nz);
                        int3 neighborPosition = MathUtils_3D.ComputeNeighborPosition(cellPosition, nx, ny, nz);
                        double3 distanceFromParticleToNeighbor = MathUtils_3D.ComputeDistanceFromParticleToNeighbor(neighborPosition, particlePosition);
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
            // Enforce boundaries
            double3 boundedVelocity = EnforceBoundaryVelocity(p);
            p.SetVelocity(boundedVelocity);
            particles[i] = p;
        }
    }

    private double3 ClampPosition(Particle_3D p)
    {
        return math.clamp(p.GetPosition(), 1, resolution - 2);
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
        // Excel file to determine volume fraction thresholding
        /*
         * VERDICT: Min = 105.1584. Max = 420.2093.
         *  So, basically 105 - 421
         */
        StreamWriter sw = null;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            sw = File.CreateText(@"c:\Users\alexc\School_Repos\MLS-MPM-unity\MLS-MPM\Assets\Resources\volumeFractions_3D.csv");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // hardcoding :/ the whitespace actually works though
            sw = File.CreateText(@"/Users/Alex/Documents/Alex's Crap/Escuela/MS/Winter_2023/MLS-MPM-unity/MLS-MPM/Assets/Resources/volumeFractions_3D.csv");
        }
        else
        {
            // Linux case not handled right now, yeeting it to the desktop
            sw = File.CreateText(@"~/Desktop/volumeFractions_3D.csv");
        }

        List<Particle_3D> flatParticleList = GetFlattenedParticleList();
        // TODO duplication is BAD, fix. 
        int fluidLevel = 3;
        float initialSizingFactor = 1f;
        double skipProbability = 0.7;
        switch (simType)
        {
            case SimType.defaultSim:
                fluidLevel = 3;
                initialSizingFactor = 1f;
                skipProbability = 0.7;
                break;
            case SimType.jacuzzi:
                // Bubbles up top, water down below
                fluidLevel = 6;
                initialSizingFactor = 0.3f;
                skipProbability = 0.3;
                break;
            case SimType.foamingSoap:
                fluidLevel = 3;
                initialSizingFactor = 0.3f;
                skipProbability = 0.1;
                break;
            case SimType.laundryDetergent:
                fluidLevel = 10;
                initialSizingFactor = 0.35f;
                skipProbability = 0.1;
                break;
            case SimType.bubbleBath:
                fluidLevel = 1;
                initialSizingFactor = 0.8f;
                skipProbability = 0.3;
                break;
            default:
                break;
        }
        for (int i = 0; i < particles.Length; i++)
        {
            int x = i / (resolution * resolution);
            int y = (i / resolution) % resolution;
            int z = i % resolution;
            Particle_3D p = particles[i];
            bool skipBubble = false;
            if ((x % 2 == 0 || y % 2 == 0 || z % 2 == 0) && MathUtils_3D.IsAir(p))
            {
                skipBubble = true;
                if (simType == SimType.foamingSoap && new System.Random().NextDouble() > 0.5)
                {
                    skipBubble = false;
                }
            }
            System.Random random = new();
            double randomValue = random.NextDouble();
            // Closer skipProbability is to 1 = more bubbles skipped
            if (randomValue < skipProbability)// && MathUtils_3D.IsAir(p))
            {
                skipBubble = true;
            }
            if (skipBubble || (y < fluidLevel && simType == SimType.foamingSoap))
            {
                // skipping is correct
                p.SetBubble(-200, true);
            }
            else if (y < fluidLevel)
            {
                p.SetBubble(50); // very small size
            }
            //else if (!MathUtils_3D.IsAir(p))
            //{
            //    // Enforcing microscopic size for fluid particles -- may delete
            //    p.SetBubble(120);
            //    sw.WriteLine(120);
            //}
            else
            {
                double volumeFraction = VolumeFractionUtils_3D.ComputeVolumeFraction(flatParticleList, p);
                p.SetBubble(volumeFraction, initialSizingFactor: initialSizingFactor);
                sw?.WriteLine(volumeFraction);
            }
            particles[i] = p;
        }
        sw.Close();
    }

    private List<Particle_3D> GetFlattenedParticleList()
    {
        List<Particle_3D> flattenedList = particles.ToList();
        return flattenedList;
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
        grid = GeometryCreator_3D.CreateNewGrid(resolution);
    }

    public void InitParticles()
    {
        particles = new Particle_3D[resolution * resolution * resolution];
        double3[] tempParticlePositions = InitTempGrid();

        // Fluid at bottom
        int fluidLevel = 3;
        switch (simType)
        {
            case SimType.defaultSim:
                fluidLevel = 3;
                break;
            case SimType.jacuzzi:
                // Bubbles up top, water down below
                fluidLevel = 6;
                break;
            default:
                break;
        }
        for (int i = 0; i < particles.Length; i++)
        {
            int y = (i / resolution) % resolution;
            bool shouldCreateFluidParticle = (y < fluidLevel);
            double3 initialVelocity = new(0);
            double3x3 initialC = new(0);
            if (shouldCreateFluidParticle)
            {
                FluidParticle_3D p = GeometryCreator_3D.CreateNewFluidParticle(tempParticlePositions[i], initialVelocity, initialC);
                particles[i] = p;
            }
            else
            {
                AirParticle_3D p = GeometryCreator_3D.CreateNewAirParticle(tempParticlePositions[i], initialVelocity, initialC);
                particles[i] = p;
            }
        }
    }

    private void InitGameCommunication()
    {
        gameInterface = GameObject.Find(geoGod).AddComponent<GameInterface_3D>();
        waterSurfacer = GameObject.Find(geoAttacher).AddComponent<WaterSurfacer_3D>();
    }

    public double3[] InitTempGrid()
    {
        double spacing = 0.5;
        double startPosition = 4; // resolution / 4
        double endPosition = 12; // resolution - (resolution - startPosition)
        double3[] tempGrid = new double3[resolution * resolution * resolution];
        int index = 0;
        for (double i = startPosition; i < endPosition; i += spacing)
        {
            for (double j = startPosition; j < endPosition; j += spacing)
            {
                for (double k = startPosition; k < endPosition; k += spacing)
                {
                    double3 position = new(i, j, k);
                    tempGrid[index] = position;
                    index++;
                }
            }
        }
        return tempGrid;
    }

    /**
     * Voronoi
     */
    private void ComputeVoronoi()
    {
        VoronoiShaderDTO_3D dto = ScriptableObject.CreateInstance<VoronoiShaderDTO_3D>();
        dto.Init(GetFlattenedParticleList());
        dto.UpdateVoronoiTexture();
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

    /**
     * Getters and Setters
     */
    public Grid_3D GetGrid()
    {
        return grid;
    }

    public Particle_3D[] GetParticles()
    {
        return particles;
    }

    public void SetNeighborDimension(int neighborDimension)
    {
        this.neighborDimension = neighborDimension;
    }

    public void SetParticles(Particle_3D[] particles)
    {
        this.particles = particles;
    }
}
