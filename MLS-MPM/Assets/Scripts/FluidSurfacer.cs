using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using Unity.Mathematics;
using UnityEngine;
/**
 * A class for surfacing fluid particles by transforming them into a mesh, because Unity doesn't have built-in water particle surfacing for some weird reason.
 */

public class FluidSurfacer : MonoBehaviour
{
    private TriangleNet.TriangleNetMesh fluidSurface = null;
    public void InitializeFluidSurface(Particle[,] particles)
    {
        Polygon polygon = InitializePolygon(particles, true);
        fluidSurface = CreateMesh(polygon);
    }

    public Polygon InitializePolygon(Particle[,] particles, bool fluidOnly = false)
    {
        Polygon polygon = new Polygon();
        for (int i = 0; i < particles.GetLength(0); i++)
        {
            for (int j = 0; j < particles.GetLength(1); j++)
            {
                Particle particle = particles[i, j];
                // 3 is fluid mass, need utility function for isFluid
                if (fluidOnly && particle.GetMass() != 3)
                {
                    // skip; we only want fluid particles here
                    continue; // TODO: Even when I remove this continue, it still renders nothing but a straight line.
                }
                double2 position = particle.GetPosition();
                //Debug.Log(position); // Not sure if casting this is losing too much accuracy
                polygon.Add(new Vertex((float)position[0], (float)position[1]));
            }
        }
        return polygon;
    }

    public TriangleNet.TriangleNetMesh CreateMesh(Polygon polygon)
    {
        // Note: must verify that this bool should actually be true in our case, if not, don't need options
        ConstraintOptions options = new() { ConformingDelaunay = true };
        TriangleNet.TriangleNetMesh mesh = (TriangleNet.TriangleNetMesh)polygon.Triangulate(options);
        return mesh;
    }

    public void OnDrawGizmos()
    {
        if (fluidSurface == null)
        {
            return;
        }
        Gizmos.color = Color.red;
        foreach (Edge edge in fluidSurface.Edges)
        {
            Vertex v0 = fluidSurface.Vertices.ElementAt(edge.P0);
            Vertex v1 = fluidSurface.Vertices.ElementAt(edge.P1);
            Vector3 p0 = new Vector3((float)v0[0], 0.0f, (float)v0[1]);
            Vector3 p1 = new Vector3((float)v1[0], 0.0f, (float)v1[1]);
            Gizmos.DrawLine(p0, p1);
        }
    }
}
