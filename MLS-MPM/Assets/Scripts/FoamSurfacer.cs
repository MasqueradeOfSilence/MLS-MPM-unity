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
         * 
         * (n/a)
         * 
         * HERE MIGHT BE THE PROBLEM:
         * public void GenerateClippedEndPoints(Rect bounds)
         * permalink: https://github.com/PixelsForGlory/VoronoiDiagram/blob/e9deee94641bb2f07c4988a7923386a589bfeeb1/Runtime/VoronoiDiagramEdge.cs#L107 
         * 
         * 
         * This function inside of PixelsForGlory sets a ZERO VECTOR as the LOWER BOUND,
         *  and overwrites values as Float.MinValue if it's less than that. This could be a problem. 
         *  
         * This function also sets the Rect bounds as the UPPER BOUND, but then it would rewrite as 
         *  Float.MinValue anyway for some reason. 
         *  
         * I am not yet sure which of the zillion IF statements is screwing it up. 
         * 
         * Almost certainly this is the issue. 
         * 
         * In reality, the minimum values we want will be dynamically adjusting at each timestep. 
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

        //Debug.Log("lowest x: " + lowestX);
        //Debug.Log("highest y: " + highestY);
        //Debug.Log("highest x: " + highestX);
        //Debug.Log("lowest y: " + lowestY);
        //Debug.Log("Y len: " + (int)Math.Round(height));
        //Debug.Log("X len: " + (int)Math.Round(width));
        Debug.Log("Lowest x for rect: " + lowestX);
        Debug.Log("Lowest y for rect: " + lowestY);
        Debug.Log("width: " + width);
        Debug.Log("height: " + height);

        // adding 0.01f will stop the OOB issues, but not the whole bug
        // well, some OOB issues must still be remaining
        rect = new Rect((float)lowestX, (float)lowestY, width + 0.01f, height + 0.01f);
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
            // In the past I got NaN issues with determinant calculation using doubles, but integer casts will screw it up too
            Vector2 position = new((float)p.GetPosition().x, (float)p.GetPosition().y);
            if (!rect.Contains(position))
            {
                Debug.LogError("Oops! Rect " + rect + " does not contain " + position);
                // no longer entered
            }

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
