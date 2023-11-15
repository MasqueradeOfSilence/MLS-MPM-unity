using PixelsForGlory.VoronoiDiagram;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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
        // TODO top of the rectangle needs to be the top of the sim, not just hardcoded to 0f!
        // and it seems to dislike that due to the relaxation parameters...I now am not sure if I can use this library

        double lowestX = double.MaxValue;
        double highestX = double.MinValue;
        double lowestY = double.MaxValue;
        double highestY = double.MinValue;
        if (particles.Length == 0)
        {
            lowestX = highestX = lowestY = highestY = 0;
            // not generally entered (tried printing)
        }

        foreach (Particle p in particles)
        {
            double x = p.GetPosition().x;
            double y = p.GetPosition().y;

            lowestX = Math.Min(lowestX, x);
            highestX = Math.Max(highestX, x);
            lowestY = Math.Min(lowestY, y);
            highestY = Math.Max(highestY, y);
        }

        float width = Mathf.Abs((float)(highestX - lowestX));
        float height = Mathf.Abs((float)(highestY - lowestY));

        Debug.Log("lowest x: " + lowestX);
        Debug.Log("highest y: " + highestY);
        Debug.Log("highest x: " + highestX);
        Debug.Log("lowest y: " + lowestY);
        Debug.Log("Y len: " + (int)Math.Round(height));
        Debug.Log("X len: " + (int)Math.Round(width));

        var voronoiDiagram = new VoronoiDiagram<Color>(new Rect((float)lowestX - 1f, (float)lowestY - 1f, width + 1f, height + 1f)); // not working, need correct bounds computation
        var points = new List<VoronoiDiagramSite<Color>>();

        foreach (Particle p in particles)
        {
            // add to points array
            if (p.GetMass() == 3)
            {
                // TODO skip if fluid, need to clean this up so we stop hardcoding that magic number 3
                continue;
            }
            // It does not like decimals for some reason (will cause NaN issues in determinant calculation), so cast to integers.
            Vector2 position = new((int)p.GetPosition()[0], (int)p.GetPosition()[1]);

            if (!points.Any(item => item.Coordinate == position))
            {
                // Randomizing the color right now, but not really needed, since we are visualizing with gizmos
                points.Add(new VoronoiDiagramSite<Color>(position, new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f))));
            }
        }
        // README says the call is AddPoints, but it is AddSites.
        bool success = voronoiDiagram.AddSites(points);
        // This cannot be zero.
        int lloydRelaxationParameter = 1;
        voronoiDiagram.GenerateSites(lloydRelaxationParameter);
        this.voronoiDiagram = voronoiDiagram;
        return voronoiDiagram;
    }

    public void CreatePowerDiagram()
    {

    }
}
