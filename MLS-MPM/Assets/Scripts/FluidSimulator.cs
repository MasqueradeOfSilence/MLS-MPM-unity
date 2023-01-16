using UnityEngine;
using Unity.Mathematics;

public class FluidSimulator : MonoBehaviour
{
    private Particle[,] particles;
    private MlsMpmGrid grid;
    // nialltl used 64...
    private int gridResolution = 96;

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
                double initialMass = 0;
                double2x2 initialC = new double2x2();
                particle.InitParticle(temporaryParticlePositions[i, j], initialVelocity, initialMass, initialC);
                // grid might be too small
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

    // Start is called before the first frame update
    void Start()
    {
        print("LOG: Starting fluid simulator");
        InitializeGridAndParticleArrays();
        GameObject exampleGeo = GameObject.Find("ExampleGeo");
        GameInterface gameInterface = exampleGeo.GetComponent<GameInterface>();
        gameInterface.DumpParticlesIntoScene(flattenParticles());
    }

    // Update is called once per frame
    void Update()
    {
        
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
