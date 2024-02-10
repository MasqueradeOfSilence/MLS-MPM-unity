using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TriangleNet;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using Unity.Mathematics;
using UnityEngine;

/**
 * Water Surfacer: Creates a mesh of water over fluid particles. 
 */
public class WaterSurfacer_3D : MonoBehaviour
{
    /**
     * Data members
     */
    private TriangleNetMesh fluidSurface = null;
    private GameObject plane;
    private Material planeMaterial;
    private static string waterMaterialName = "ClearBubbleTest";

    // Start is called before the first frame update
    void Start()
    {
        plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "WaterPlane";
        planeMaterial = Resources.Load(waterMaterialName, typeof(Material)) as Material;
        plane.GetComponent<MeshRenderer>().material = planeMaterial;
        plane.GetComponent<Renderer>().material = planeMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeFluidSurface(Particle_3D[] particles, int resolution, bool fluidOnly = true)
    {
        Polygon polygon = InitializePolygon(particles, resolution, fluidOnly);
        fluidSurface = CreateMesh(polygon);
        MakeMesh(fluidSurface);
    }

    private bool ParticleIsAir(Particle_3D p)
    {
        return p.GetMass() != 3;
    }

    public Polygon InitializePolygon(Particle_3D[] particles, int resolution, bool fluidOnly = false)
    {
        Polygon polygon = new();
        // Plane polygon for front row. Doing z = 0 for now, but will do all 4 edge faces.
        for (int i = 0; i < particles.Length; i++)
        {
            int x = i / (resolution * resolution);
            int y = (i / resolution) % resolution;
            int z = i % resolution;
            Particle_3D p = particles[i];
            // probably not gonna use
            if (fluidOnly && ParticleIsAir(p))
            {
                continue;
            }
            // front row 
            if (z == 0)
            {
                double3 position = p.GetPosition();
                polygon.Add(new Vertex((float)position.x, (float)position.y));
            }
        }
        //for (int i = 0; i < particles.Length; i++) 
        //{
        //    for (int j = 0; j < particles[i].Length; j++)
        //    {
        //        Particle_3D p = particles[i][j][0];
        //        if (fluidOnly && ParticleIsAir(p))
        //        {
        //            continue;
        //        }
        //        double3 position = p.GetPosition();
        //        polygon.Add(new Vertex((float)position.x, (float)position.y));
        //    }
        //}
        return polygon;
    }

    public TriangleNetMesh CreateMesh(Polygon polygon)
    {
        ConstraintOptions options = new() { ConformingDelaunay = true };
        TriangleNetMesh mesh = (TriangleNetMesh)polygon.Triangulate(options);
        return mesh;
    }

    private void MakeMesh(TriangleNetMesh mesh)
    {
        List<int> triangles = new();
        List<Vector3> vertices = new();
        List<Vector3> normals = new();
        List<Vector2> UVs = new();
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

    public void SetPlane(GameObject plane)
    {
        this.plane = plane;
    }

    public TriangleNetMesh GetFluidSurface()
    {
        return fluidSurface;
    }

}
