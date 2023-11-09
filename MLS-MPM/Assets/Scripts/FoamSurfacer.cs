using PixelsForGlory.VoronoiDiagram;
using UnityEngine;

/**
 * Class for ensuring that bubbles contact each other in a physically realistic manner, instead of overlapping. 
 * We are using weighted Voronoi (AKA power) diagrams to accomplish this. 
 */

public class FoamSurfacer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrawGizmos()
    {
        
    }

    // For preliminary testing purposes
    public VoronoiDiagram<Color> CreateUnweightedVoronoiDiagram(Particle[,] particles)
    {
        var voronoiDiagram = new VoronoiDiagram<Color>(new Rect(0f, 0f, particles.GetLength(0), particles.GetLength(1)));
        return voronoiDiagram;
    }

    public void CreatePowerDiagram()
    {

    }
}
