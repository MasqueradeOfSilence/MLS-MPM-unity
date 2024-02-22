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

    // Extras
    private GameObject sidePlane1;
    private TriangleNetMesh sideFluidSurface1 = null;
    private GameObject sidePlane2;
    private TriangleNetMesh sideFluidSurface2 = null;
    private GameObject sidePlane3;
    private TriangleNetMesh sideFluidSurface3 = null;

    // Start is called before the first frame update
    void Start()
    {
        plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "WaterPlane";
        planeMaterial = Resources.Load(waterMaterialName, typeof(Material)) as Material;
        plane.GetComponent<MeshRenderer>().material = planeMaterial;
        plane.GetComponent<Renderer>().material = planeMaterial;

        sidePlane1 = GameObject.CreatePrimitive(PrimitiveType.Plane);
        sidePlane1.name = "SidePlane1";
        sidePlane1.GetComponent<MeshRenderer>().material = planeMaterial;
        sidePlane1.GetComponent<Renderer>().material = planeMaterial;

        sidePlane2 = GameObject.CreatePrimitive(PrimitiveType.Plane);
        sidePlane2.name = "SidePlane2";
        sidePlane2.GetComponent<MeshRenderer>().material = planeMaterial;
        sidePlane2.GetComponent<Renderer>().material = planeMaterial;

        sidePlane3 = GameObject.CreatePrimitive(PrimitiveType.Plane);
        sidePlane3.name = "SidePlane3";
        sidePlane3.GetComponent<MeshRenderer>().material = planeMaterial;
        sidePlane3.GetComponent<Renderer>().material = planeMaterial;
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

        Polygon side1 = InitializePolygonSide1(particles, resolution, fluidOnly);
        sideFluidSurface1 = CreateMesh(side1);
        MakeMeshSide1(sideFluidSurface1);

        Polygon side2 = InitializePolygonSide2(particles, resolution, fluidOnly);
        sideFluidSurface2 = CreateMesh(side2);
        MakeMeshSide2(sideFluidSurface2);

        Polygon side3 = InitializePolygonSide3(particles, resolution, fluidOnly);
        sideFluidSurface3 = CreateMesh(side3);
        MakeMeshSide3(sideFluidSurface3);
    }

    private bool ParticleIsAir(Particle_3D p)
    {
        return p.GetMass() != 3;
    }

    private Polygon InitializePolygonSide3(Particle_3D[] particles, int resolution, bool fluidOnly = false)
    {
        Polygon polygon = new();
        for (int i = 0; i < particles.Length; i++)
        {
            int z = i % resolution;
            Particle_3D p = particles[i];
            if (fluidOnly && ParticleIsAir(p))
            {
                continue;
            }
            // side 1
            if (z == resolution - 1)
            {
                double3 position = p.GetPosition();
                polygon.Add(new Vertex((float)position.x, (float)position.y));
            }
        }
        return polygon;
    }

    private Polygon InitializePolygonSide2(Particle_3D[] particles, int resolution, bool fluidOnly = false)
    {
        Polygon polygon = new();
        for (int i = 0; i < particles.Length; i++)
        {
            int x = i / (resolution * resolution);
            Particle_3D p = particles[i];
            if (fluidOnly && ParticleIsAir(p))
            {
                continue;
            }
            // side 1
            if (x == resolution - 1)
            {
                double3 position = p.GetPosition();
                polygon.Add(new Vertex((float)position.y, (float)position.z));
            }
        }
        return polygon;
    }

    private Polygon InitializePolygonSide1(Particle_3D[] particles, int resolution, bool fluidOnly = false)
    {
        Polygon polygon = new();
        for (int i = 0; i < particles.Length; i++)
        {
            int x = i / (resolution * resolution);
            Particle_3D p = particles[i];
            if (fluidOnly && ParticleIsAir(p))
            {
                continue;
            }
            // side 2
            if (x == 0)
            {
                double3 position = p.GetPosition();
                polygon.Add(new Vertex((float)position.y, (float)position.z));
            }
        }
        return polygon;
    }

    public Polygon InitializePolygon(Particle_3D[] particles, int resolution, bool fluidOnly = false)
    {
        Polygon polygon = new();
        // Plane polygon for front row. 
        for (int i = 0; i < particles.Length; i++)
        {
            int z = i % resolution;
            Particle_3D p = particles[i];
            if (fluidOnly && ParticleIsAir(p))
            {
                continue;
            }
            // back row 
            if (z == 0)
            {
                double3 position = p.GetPosition();
                polygon.Add(new Vertex((float)position.x, (float)position.y));
            }
        }
        return polygon;
    }

    public TriangleNetMesh CreateMesh(Polygon polygon)
    {
        ConstraintOptions options = new() { ConformingDelaunay = true };
        TriangleNetMesh mesh = (TriangleNetMesh)polygon.Triangulate(options);
        return mesh;
    }

    private void MakeMeshSide3(TriangleNetMesh mesh)
    {
        List<int> triangles = new();
        List<Vector3> vertices = new();
        List<Vector3> normals = new();
        List<Vector2> UVs = new();
        foreach (var triangle in mesh.Triangles)
        {
            Vector3 v0 = Get3DPointSide3(triangle.GetVertex(0).ID, mesh);
            Vector3 v1 = Get3DPointSide3(triangle.GetVertex(1).ID, mesh);
            Vector3 v2 = Get3DPointSide3(triangle.GetVertex(2).ID, mesh);
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

        sidePlane3.GetComponent<MeshFilter>().mesh = meshForUnity;
        sidePlane3.GetComponent<MeshCollider>().sharedMesh = meshForUnity;
    }

    private void MakeMeshSide2(TriangleNetMesh mesh)
    {
        List<int> triangles = new();
        List<Vector3> vertices = new();
        List<Vector3> normals = new();
        List<Vector2> UVs = new();
        foreach (var triangle in mesh.Triangles)
        {
            Vector3 v0 = Get3DPointSide2(triangle.GetVertex(0).ID, mesh);
            Vector3 v1 = Get3DPointSide2(triangle.GetVertex(1).ID, mesh);
            Vector3 v2 = Get3DPointSide2(triangle.GetVertex(2).ID, mesh);
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
        sidePlane2.GetComponent<MeshFilter>().mesh = meshForUnity;
        sidePlane2.GetComponent<MeshCollider>().sharedMesh = meshForUnity;
    }

    private void MakeMeshSide1(TriangleNetMesh mesh)
    {
        List<int> triangles = new();
        List<Vector3> vertices = new();
        List<Vector3> normals = new();
        List<Vector2> UVs = new();
        foreach (var triangle in mesh.Triangles)
        {
            Vector3 v0 = Get3DPointSide1(triangle.GetVertex(0).ID, mesh);
            Vector3 v1 = Get3DPointSide1(triangle.GetVertex(1).ID, mesh);
            Vector3 v2 = Get3DPointSide1(triangle.GetVertex(2).ID, mesh);
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
        sidePlane1.GetComponent<MeshFilter>().mesh = meshForUnity;
        sidePlane1.GetComponent<MeshCollider>().sharedMesh = meshForUnity;
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

    private Vector3 Get3DPointSide3(int index, TriangleNetMesh mesh)
    {
        Vertex vertex = mesh.Vertices.ElementAt(index);
        return new Vector3(vertex.X, vertex.Y, 14);
    }

    private Vector3 Get3DPointSide2(int index, TriangleNetMesh mesh)
    {
        Vertex vertex = mesh.Vertices.ElementAt(index);
        return new Vector3(12, vertex.X, vertex.Y);
    }

    private Vector3 Get3DPointSide1(int index, TriangleNetMesh mesh)
    {
        Vertex vertex = mesh.Vertices.ElementAt(index);
        // x normally is around 2
        return new Vector3(2, vertex.X, vertex.Y);
    }

    private Vector3 Get3DPoint(int index, TriangleNetMesh mesh)
    {
        Vertex vertex = mesh.Vertices.ElementAt(index);
        // Hardcoding 2 is not ideal but it gets close to our current sim
        return new Vector3(vertex.X, vertex.Y, 2);
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
