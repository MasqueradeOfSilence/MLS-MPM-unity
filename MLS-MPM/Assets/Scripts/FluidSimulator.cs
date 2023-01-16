using UnityEngine;

public class FluidSimulator : MonoBehaviour
{
    private Particle[,] particles;
    private MlsMpmGrid grid;
    private int gridResolution = 64;

    public void InitializeParticleArray()
    {
        grid = ScriptableObject.CreateInstance("MlsMpmGrid") as MlsMpmGrid;
        grid.InitMlsMpmGrid(gridResolution);
        particles = new Particle[grid.GetGridResolution(), grid.GetGridResolution()];
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
}
