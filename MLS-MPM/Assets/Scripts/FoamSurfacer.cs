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
    Rect rect;
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
        foreach (KeyValuePair<int, VoronoiDiagramGeneratedSite<Color>> voronoiCellPair in voronoiDiagram.GeneratedSites)
        {
            foreach(VoronoiDiagramGeneratedEdge edge in voronoiCellPair.Value.Edges)
            {
                Gizmos.color = Color.cyan;
                Vector3 p0 = new(edge.LeftEndPoint[0], edge.LeftEndPoint[1], 0.0f);
                Vector3 p1 = new(edge.RightEndPoint[0], edge.RightEndPoint[1], 0.0f);
                Gizmos.DrawLine(p0, p1);
            }
        }
        if (rect != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(new Vector3(rect.center.x, rect.center.y, 0.01f), new Vector3(rect.size.x, rect.size.y, 0.01f));
        }
    }

    // For preliminary testing purposes
    public VoronoiDiagram<Color> CreateUnweightedVoronoiDiagram(Particle[,] particles, int dimension)
    {
        // TODO top of the rectangle needs to be the top of the sim, not just hardcoded to 0f!
        // and it seems to dislike that due to the relaxation parameters...I now am not sure if I can use this library
        // not using dim currently. tbd

        /**
         * NOTE:
         * so, the bounds might actually be fine based on what we have. I drew the Rect to the screen as a Gizmo and it didn't seem off. 
         * it also moved with the sim at each timestep as expected. 
         * 
         * so now I'm wondering if the cells are getting generated too high up or something. lloyd relaxation should not be causing this issue, but can't rule it out yet.
         * 
         * error is coming from https://github.com/PixelsForGlory/VoronoiDiagram/blob/e9deee94641bb2f07c4988a7923386a589bfeeb1/Runtime/VoronoiDiagram.cs#L77 
         * 
         * but I am not sure where that is called from? gross.
         * 
         * it seems that it really wants to use the original box area for some reason.
         * 
         * possible courses of action:
         * - generate with max bounding box such that nothing is OOB, then cull the OOB stuff in the Gizmo render. we will also need to cull it in our intersection computation. 
         * - look closer into the sites computed as OOB and see how close they are to the actual bounds. maybe they are close enough to where we can just expand it a bit. 
         * --> it's also possible that they are getting generated all the way to the top of the original box
         */

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

        rect = new Rect((float)lowestX, (float)lowestY, width, height);
        var voronoiDiagram = new VoronoiDiagram<Color>(rect); // not working yet
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
        // This cannot be zero it seems.
        int lloydRelaxationParameter = 1;
        voronoiDiagram.GenerateSites(lloydRelaxationParameter);
        this.voronoiDiagram = voronoiDiagram;
        return voronoiDiagram;
    }

    public void CreatePowerDiagram()
    {

    }
}
