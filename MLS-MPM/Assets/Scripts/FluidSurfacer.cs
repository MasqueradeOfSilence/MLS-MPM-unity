using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TriangleNet;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using Unity.Mathematics;
using UnityEngine;
/**
 * A class for surfacing fluid particles by transforming them into a mesh, because Unity doesn't have built-in water particle surfacing for some weird reason.
 */

public class FluidSurfacer : MonoBehaviour
{
    private TriangleNetMesh fluidSurface = null;
    private TriangleNetMesh foamSurface = null;
    public Transform fluidPrefab;
    private GameObject plane;
    private Material planeMaterial;

    void Start()
    {
        fluidPrefab = Resources.Load("Prefabs/FluidPrefab", typeof(Transform)) as Transform;
        plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "WaterPlane";
        planeMaterial = Resources.Load("ClearBubbleTest", typeof(Material)) as Material;
        plane.GetComponent<MeshRenderer>().material = planeMaterial;
        plane.GetComponent<Renderer>().material = planeMaterial;
    }

    private void EnableGizmoToViewFoamDelaunay(Particle[,] particles)
    {
        Polygon polygon = new();
        for (int i = 0; i < particles.GetLength(0); i++)
        {
            for (int j = 0; j < particles.GetLength(1); j++)
            {
                Particle particle = particles[i, j];
                // if particle is NOT a fluid, TODO write isFluid
                if (particle.GetMass() != 3)
                {
                    double2 position = particle.GetPosition();
                    polygon.Add(new Vertex((float)position[0], (float)position[1]));
                }
            }
        }
        foamSurface = CreateMesh(polygon);
    }

    public void InitializeFluidSurface(Particle[,] particles)
    {
        Polygon polygon = InitializePolygon(particles, true);
        fluidSurface = CreateMesh(polygon);
        MakeMesh(fluidSurface);
        // Can flip bool for visualization. It will appear in the viewport, not in the gameplay or rendered view.
        bool viewDelaunayTriangulationForFoamSurface = true;
        if (viewDelaunayTriangulationForFoamSurface)
        {
            EnableGizmoToViewFoamDelaunay(particles);
        }
    }

    public Polygon InitializePolygon(Particle[,] particles, bool fluidOnly = false)
    {
        Polygon polygon = new();
        for (int i = 0; i < particles.GetLength(0); i++)
        {
            for (int j = 0; j < particles.GetLength(1); j++)
            {
                Particle particle = particles[i, j];
                // 3 is fluid mass, need utility function for isFluid
                if (fluidOnly && particle.GetMass() != 3)
                {
                    // skip; we only want fluid particles here
                    continue;
                }
                double2 position = particle.GetPosition();
                polygon.Add(new Vertex((float)position[0], (float)position[1]));
            }
        }
        return polygon;
    }

    public TriangleNetMesh CreateMesh(Polygon polygon)
    {
        // Note: must verify that this bool should actually be true in our case, if not, don't need options
        ConstraintOptions options = new() { ConformingDelaunay = true };
        TriangleNetMesh mesh = (TriangleNetMesh) polygon.Triangulate(options);
        return mesh;
    }

    public void MakeMesh(TriangleNetMesh mesh)
    {
        List<int> triangles = new();
        List<Vector3> vertices = new();
        List<Vector3> normals = new();
        List<Vector2> UVs = new();
        // Alternate loop (TBD)
        foreach (var triangle in mesh.Triangles)
        {
            Vector3 v0 = Get3DPoint(triangle.GetVertex(0).ID, mesh);
            Vector3 v1 = Get3DPoint(triangle.GetVertex(1).ID, mesh);
            Vector3 v2 = Get3DPoint(triangle.GetVertex(2).ID, mesh);
            triangles.Add(vertices.Count);
            triangles.Add(vertices.Count + 1);
            triangles.Add(vertices.Count + 2);
            vertices.Add(v0);
            vertices.Add(v1);
            vertices.Add(v2);
            Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            UVs.Add(new Vector2(0.0f, 0.0f));
            UVs.Add(new Vector2(0.0f, 0.0f));
            UVs.Add(new Vector2(0.0f, 0.0f));
        }

        Mesh meshForUnity = new()
        {
            vertices = vertices.ToArray(),
            uv = UVs.ToArray(),
            triangles = triangles.ToArray(),
            normals = normals.ToArray()
        };

        plane.GetComponent<MeshFilter>().mesh = meshForUnity;
        plane.GetComponent<MeshCollider>().sharedMesh = meshForUnity;
    }

    private Vector3 Get3DPoint(int index, TriangleNetMesh mesh)
    {
        Vertex vertex = mesh.Vertices.ElementAt(index);
        // most likely 0 for now in 2D
        return new Vector3(vertex.X, vertex.Y, 0);
    }

    public void OnDrawGizmos()
    {
        if (foamSurface != null)
        {
            Gizmos.color = Color.blue;
            foreach (Edge edge in foamSurface.Edges)
            {
                Vertex v0 = foamSurface.Vertices.ElementAt(edge.P0);
                Vertex v1 = foamSurface.Vertices.ElementAt(edge.P1);
                Vector3 p0 = new((float)v0[0], (float)v0[1], 0.0f);
                Vector3 p1 = new((float)v1[0], (float)v1[1], 0.0f);
                Gizmos.DrawLine(p0, p1);
            }
        }
        if (fluidSurface == null)
        {
            return;
        }
        Gizmos.color = Color.red;
        foreach (Edge edge in fluidSurface.Edges)
        {
            Vertex v0 = fluidSurface.Vertices.ElementAt(edge.P0);
            Vertex v1 = fluidSurface.Vertices.ElementAt(edge.P1);
            Vector3 p0 = new((float)v0[0], (float)v0[1], 0.0f);
            Vector3 p1 = new((float)v1[0], (float)v1[1], 0.0f);
            Gizmos.DrawLine(p0, p1);
        }
    }
}
