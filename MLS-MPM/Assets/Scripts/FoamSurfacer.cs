using PixelsForGlory.VoronoiDiagram;
using System.Collections.Generic;
using System.Linq;
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
    public VoronoiDiagram<Color> CreateUnweightedVoronoiDiagram(Particle[,] particles, int dimension)
    {
        var voronoiDiagram = new VoronoiDiagram<Color>(new Rect(0f, 0f, dimension, dimension));
        var points = new List<VoronoiDiagramSite<Color>>();

        foreach (Particle p in particles)
        {
            // add to points array
            if (p.GetMass() == 3)
            {
                // skip if fluid, need to clean this up so we stop hardcoding that magic number 3
                continue;
            }
            // It does not like decimals for some reason (will cause NaN issues in determinant calculation), so cast to integers.
            Vector2 position = new((int)p.GetPosition()[0], (int)p.GetPosition()[1]);

            if (!points.Any(item => item.Coordinate == position))
            {
                points.Add(new VoronoiDiagramSite<Color>(position, new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f))));

            }
        }
        // README says the call is AddPoints, but it is AddSites.
        bool success = voronoiDiagram.AddSites(points);
        int lloydRelaxationParameter = 2;
        voronoiDiagram.GenerateSites(lloydRelaxationParameter);
        return voronoiDiagram;
    }

    public void CreatePowerDiagram()
    {

    }
}
