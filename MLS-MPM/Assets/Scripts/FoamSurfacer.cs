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
    private VoronoiDiagram<Color> voronoiDiagram;
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
        foreach (KeyValuePair<int, VoronoiDiagramGeneratedSite<Color>> voronoiCellPair in this.voronoiDiagram.GeneratedSites)
        {
            foreach(VoronoiDiagramGeneratedEdge edge in voronoiCellPair.Value.Edges)
            {
                Gizmos.color = Color.cyan;
                Vector3 p0 = new(edge.LeftEndPoint[0], edge.LeftEndPoint[1], 0.0f);
                Vector3 p1 = new(edge.RightEndPoint[0], edge.RightEndPoint[1], 0.0f);
                Gizmos.DrawLine(p0, p1);
            }
        }
    }

    // For preliminary testing purposes
    public VoronoiDiagram<Color> CreateUnweightedVoronoiDiagram(Particle[,] particles, int dimension)
    {
        // TODO position may be off, or at least it isn't calculating the cells on the edge properly
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
        // TODO adjust me
        int lloydRelaxationParameter = 1;
        voronoiDiagram.GenerateSites(lloydRelaxationParameter);
        this.voronoiDiagram = voronoiDiagram;
        return voronoiDiagram;
    }

    public void CreatePowerDiagram()
    {

    }
}
