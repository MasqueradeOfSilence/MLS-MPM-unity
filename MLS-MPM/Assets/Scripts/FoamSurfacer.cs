using PixelsForGlory.VoronoiDiagram;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

/**
 * Class for ensuring that bubbles contact each other in a physically realistic manner, instead of overlapping. 
 * We are using weighted Voronoi (AKA power) diagrams to accomplish this. 
 */

public class FoamSurfacer : MonoBehaviour
{
    private VoronoiDiagram<Color> voronoiDiagram;
    private VoronoiDiagram<Color> weightedVD;
    private Rect rectAtZero;
    private Rect adjustedRectForCurrentPosition;
    private Rect rectAtZeroWeighted;
    private Rect adjustedRectForCurrentPositionWeighted;

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
        //bool drawUntranslatedVoronoi = false;
        //double distX = Math.Abs(rectAtZero.center.x - adjustedRectForCurrentPosition.center.x);
        //double distY = Math.Abs(rectAtZero.center.y - adjustedRectForCurrentPosition.center.y);
        //foreach (KeyValuePair<int, VoronoiDiagramGeneratedSite<Color>> voronoiCellPair in voronoiDiagram.GeneratedSites)
        //{
        //    Color randomizedColor = new(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        //    foreach (VoronoiDiagramGeneratedEdge edge in voronoiCellPair.Value.Edges)
        //    {
        //        if (drawUntranslatedVoronoi)
        //        {
        //            Gizmos.color = Color.cyan;
        //            Vector3 p0 = new(edge.LeftEndPoint[0], edge.LeftEndPoint[1], 0.0f);
        //            Vector3 p1 = new(edge.RightEndPoint[0], edge.RightEndPoint[1], 0.0f);
        //            Gizmos.DrawLine(p0, p1);
        //        }
        //        Gizmos.color = Color.green;
        //        Vector3 p0_2 = new(edge.LeftEndPoint[0] + (float)distX, edge.LeftEndPoint[1] + (float)distY, 0.0f);
        //        Vector3 p1_2 = new(edge.RightEndPoint[0] + (float)distX, edge.RightEndPoint[1] + (float)distY, 0.0f);
        //        Gizmos.DrawLine(p0_2, p1_2);

        //    }
        //}

        //foreach (KeyValuePair<int, VoronoiDiagramGeneratedSite<Color>> voronoiCellPair in weightedVD.GeneratedSites)
        //{
        //    Color randomizedColor = new(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        //    foreach (VoronoiDiagramGeneratedEdge edge in voronoiCellPair.Value.Edges)
        //    {
        //        Gizmos.color = Color.yellow;
        //        Vector3 p0_2 = new(edge.LeftEndPoint[0] + (float)distX, edge.LeftEndPoint[1] + (float)distY, 0.0f);
        //        Vector3 p1_2 = new(edge.RightEndPoint[0] + (float)distX, edge.RightEndPoint[1] + (float)distY, 0.0f);
        //        Gizmos.DrawLine(p0_2, p1_2);
        //    }
        //}

        //if (rectAtZero != null && drawUntranslatedVoronoi)
        //{
        //    Gizmos.color = Color.black;
        //    Gizmos.DrawWireCube(new Vector3(rectAtZero.center.x, rectAtZero.center.y, 0.01f), new Vector3(rectAtZero.size.x, rectAtZero.size.y, 0.01f));
        //}
        //if (adjustedRectForCurrentPosition != null) 
        //{
        //    Gizmos.color = Color.magenta;
        //    Gizmos.DrawWireCube(new Vector3(adjustedRectForCurrentPosition.center.x, adjustedRectForCurrentPosition.center.y, 0.01f), new Vector3(adjustedRectForCurrentPosition.size.x, adjustedRectForCurrentPosition.size.y, 0.01f));
        //}
    }

    // For preliminary testing purposes. Fine if the bubbles overlap the adjustedRect a bit since it's unweighted currently.
    public VoronoiDiagram<Color> CreateUnweightedVoronoiDiagram(Particle[,] particles)
    {
        double lowestX = double.MaxValue;
        double highestX = double.MinValue;
        double lowestY = double.MaxValue;
        double highestY = double.MinValue;

        foreach (Particle p in particles)
        {
            double x = p.GetPosition().x;
            double y = p.GetPosition().y;

            lowestX = Math.Min(lowestX, x);
            highestX = Math.Max(highestX, x);
            lowestY = Math.Min(lowestY, y);
            highestY = Math.Max(highestY, y);
        }
        float width = Mathf.Abs((float)(highestX - lowestX)) + 0.01f;
        float height = Mathf.Abs((float)(highestY - lowestY)) + 0.01f;

        double untranslatedX = lowestX;
        double untranslatedY = lowestY;

        Rect rect = new(0f, 0f, width, height);
        adjustedRectForCurrentPosition = new Rect((float)lowestX, (float)lowestY, width, height);
        var voronoiDiagram = new VoronoiDiagram<Color>(rect);
        rectAtZero = rect;
        var points = new List<VoronoiDiagramSite<Color>>();
        double2 distance = new(-untranslatedX, -untranslatedY);
        // Color doesn't matter if we are visualizing as Gizmo
        Color defaultColor = new(0f, 0f, 0f);
        double2 lowerRHCornerPosition = new(lowestX, lowestY);

        foreach (Particle p in particles)
        {
            if (p.GetMass() == 3) // TODO change to p.isFluid()
            {
                continue;
            }
            double2 position = p.GetPosition();
            double2 translatedPosition = position + distance;
            Vector2 translatedPositionFormatted = new((int)translatedPosition.x, (int)translatedPosition.y);
            if (!points.Any(item => item.Coordinate == translatedPositionFormatted)) // TODO change to !points.ContainsPosition()
            {
                points.Add(new VoronoiDiagramSite<Color>(translatedPositionFormatted, defaultColor));
            }
        }
        voronoiDiagram.AddSites(points);
        voronoiDiagram.GenerateSites(2);
        this.voronoiDiagram = voronoiDiagram;
        return voronoiDiagram;
    }


    private double ComputeWeightOfParticle(Particle p)
    {
        if (p.GetBubble() != null && p.GetBubble().GetVolumeFraction() != 0)
        {
            return p.GetBubble().ComputeUnitySphereRadius();
        }
        return 0;
    }
    /*
     * A power diagram is a weighted Voronoi diagram based on the sizes of the bubbles
     */
    public VoronoiDiagram<Color> CreateWeightedVoronoiDiagram(Particle[,] particles)
    {
        double scalingFactor = 0.02;
        foreach (Particle p in particles)
        {
            if (p.GetBubble() != null && p.GetBubble().GetVolumeFraction() != 0)
            {
                p.SetPosition(p.GetPosition() - (ComputeWeightOfParticle(p) * scalingFactor));
            }
        }

        double lowestX = double.MaxValue;
        double highestX = double.MinValue;
        double lowestY = double.MaxValue;
        double highestY = double.MinValue;

        foreach (Particle p in particles)
        {
            double x = p.GetPosition().x;
            double y = p.GetPosition().y;

            lowestX = Math.Min(lowestX, x);
            highestX = Math.Max(highestX, x);
            lowestY = Math.Min(lowestY, y);
            highestY = Math.Max(highestY, y);
        }

        float width = Mathf.Abs((float)(highestX - lowestX)) + (float)(0.41 * scalingFactor);
        float height = Mathf.Abs((float)(highestY - lowestY)) + (float)(0.41 * scalingFactor);
        double untranslatedX = lowestX;
        double untranslatedY = lowestY;

        Rect rect = new(0f, 0f, width, height);
        adjustedRectForCurrentPositionWeighted = new Rect((float)lowestX, (float)lowestY, width, height);
        var voronoiDiagram = new VoronoiDiagram<Color>(rect);
        rectAtZeroWeighted = rect;
        var points = new List<VoronoiDiagramSite<Color>>();
        double2 distance = new(-untranslatedX, -untranslatedY);
        // Color doesn't matter if we are visualizing as Gizmo
        Color defaultColor = new(0f, 0f, 0f);
        double2 lowerRHCornerPosition = new(lowestX, lowestY);

        foreach (Particle p in particles)
        {
            if (p.GetMass() == 3) // TODO change to p.isFluid()
            {
                continue;
            }
            double2 position = p.GetPosition();
            double2 translatedPosition = position + distance;
            Vector2 translatedPositionFormatted = new((int)translatedPosition.x, (int)translatedPosition.y);
            if (!points.Any(item => item.Coordinate == translatedPositionFormatted)) // TODO change to !points.ContainsPosition()
            {
                points.Add(new VoronoiDiagramSite<Color>(translatedPositionFormatted, defaultColor));
            }
        }
        voronoiDiagram.AddSites(points);
        voronoiDiagram.GenerateSites(2);
        weightedVD = voronoiDiagram;
        return weightedVD;
    }
}
