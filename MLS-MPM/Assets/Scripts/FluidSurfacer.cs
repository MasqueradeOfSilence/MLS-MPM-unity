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
                    continue;
                }
                double2 position = particle.GetPosition();
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

    public void MakeMesh(TriangleNet.TriangleNetMesh mesh)
    {
        // Only use this if we need a speed-up
        //int trianglesInChunk = 20000; // https://github.com/Chaosed0/DelaunayUnity/blob/master/Assets/DelaunayTerrain.cs
        //IEnumerator<TriangleNet.Topology.Triangle> triangleEnumerator = mesh.Triangles.GetEnumerator();
        //for (int chunkStart = 0; chunkStart < mesh.Triangles.Count; chunkStart += trianglesInChunk)
        //{

        //}
        List<int> triangles = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> UVs = new List<Vector2>();
        // Alternate loop (TBD)
        foreach (var triangle in mesh.Triangles)
        {
            Vector3 v0 = Get3DPoint(triangle.GetVertex(2).ID, mesh);
            Vector3 v1 = Get3DPoint(triangle.GetVertex(1).ID, mesh);
            Vector3 v2 = Get3DPoint(triangle.GetVertex(0).ID, mesh);
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

        Mesh meshForUnity = new Mesh();
        meshForUnity.vertices = vertices.ToArray();
        meshForUnity.uv = UVs.ToArray();
        meshForUnity.triangles = triangles.ToArray();
        meshForUnity.normals = normals.ToArray();

        // TODO create chunkPrefab and uncomment this
        //Transform gameObject = Instantiate<Transform>(chunkPrefab, transform.position, transform.rotation);
        //gameObject.GetComponent<MeshFilter>().mesh = meshForUnity;
        //gameObject.GetComponent<MeshCollider>().sharedMesh = meshForUnity;
        //gameObject.transform.parent = transform;

        // we need to call this function at the correct time to surface the water

    }

    private Vector3 Get3DPoint(int index, TriangleNet.TriangleNetMesh mesh)
    {
        Vertex vertex = mesh.Vertices.ElementAt(index);
        // most likely 0 for now in 2D
        return new Vector3(vertex.X, vertex.Y, 0);
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
            Vector3 p0 = new((float)v0[0], (float)v0[1], 0.0f);
            Vector3 p1 = new((float)v1[0], (float)v1[1], 0.0f);
            Gizmos.DrawLine(p0, p1);
        }
    }
}
