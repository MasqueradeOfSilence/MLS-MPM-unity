using Unity.Mathematics;
using UnityEngine;

public class GeometryCreator_3D : MonoBehaviour
{
    private static int sphereID = 0;
    private const string defaultClearMaterial = "ClearBubbleTest";
    private const string fluidMatForViewport = "FluidTest";

    private static bool IsAir(double mass)
    {
        AirParticle_3D airParticle = ScriptableObject.CreateInstance("AirParticle_3D") as AirParticle_3D;
        airParticle.Init(new double3(0), new double3(0), new double3x3(0));
        return mass == airParticle.GetMass();
    }

    private static GameObject ConstructSphereFromParticle(Particle_3D p, string shaderName = defaultClearMaterial)
    {
        if (p.HasBubble())
        {
            float radius = p.GetBubble().ComputeUnitySphereRadius();
            return SpawnParticleSphere3D(p.GetPosition(), p.GetMass(), radius, shaderName);
        }
        return SpawnParticleSphere3D(p.GetPosition(), p.GetMass(), materialName: shaderName);
    }

    public static GameObject SpawnParticleSphere3D(double3 location, double mass, float sphereSize = 0.01f, string materialName = defaultClearMaterial)
    {
        bool isFoam = IsAir(mass);
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = new Vector3((float)location.x, (float)location.y, (float)location.z);
        sphere.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
        Material mat;
        if (materialName == "WhiteBubbleShader")
        {
            Debug.Log("HOLA3");
        }
        if (isFoam)
        {
            Debug.Log("HOLA4");
            mat = Resources.Load(materialName, typeof(Material)) as Material;
        }
        else
        {
            Debug.Log("RADIUS: " + sphereSize); // TODO not sure why this is jittering
            // It's a fluid
            mat = Resources.Load(fluidMatForViewport, typeof(Material)) as Material;
        }
        sphere.GetComponent<MeshRenderer>().material = mat;
        sphere.GetComponent<Renderer>().material = mat;
        //sphere.GetComponent<Renderer>().sharedMaterial = mat;
        //sphere.GetComponent<MeshRenderer>().sharedMaterial = mat;
        sphere.name = "Sphere" + sphereID.ToString();
        sphereID++;
        return sphere;
    }

    public static GameObject[] SpawnFinalParticleSpheres(Particle_3D[] particles, bool shouldUseFFFShader = false, bool shouldUseWhiteShader = false)
    {
        GameObject[] finalParticleSpheres = new GameObject[particles.Length];
        for (int i = 0; i < particles.Length; i++) 
        {
            Particle_3D p = particles[i];
            if (shouldUseFFFShader)
            {
                GameObject particleSphere = ConstructSphereFromParticle(p, "FFFBubbles");
                finalParticleSpheres[i] = particleSphere;
            }
            if (shouldUseWhiteShader)
            {
                GameObject particleSphere = ConstructSphereFromParticle(p, "WhiteBubbleShader");
                finalParticleSpheres[i] = particleSphere;
                Debug.Log("HOLA2");
            }
            else
            {
                GameObject particleSphere = ConstructSphereFromParticle(p);
                finalParticleSpheres[i] = particleSphere;
            }
        }
        return finalParticleSpheres;
    }

    /**
     * Since no constructors exist for ScriptableObjects
     */
    public static Particle_3D CreateNewParticle(double3 position, double3 velocity, double mass, double3x3 c)
    {
        Particle_3D p = ScriptableObject.CreateInstance("Particle_3D") as Particle_3D;
        p.Init(position, velocity, mass, c);
        return p;
    }

    public static FluidParticle_3D CreateNewFluidParticle(double3 position, double3 velocity, double3x3 c)
    {
        FluidParticle_3D p = ScriptableObject.CreateInstance("FluidParticle_3D") as FluidParticle_3D;
        p.Init(position, velocity, c);
        return p;
    }

    public static AirParticle_3D CreateNewAirParticle(double3 position, double3 velocity, double3x3 c)
    {
        AirParticle_3D p = ScriptableObject.CreateInstance("AirParticle_3D") as AirParticle_3D;
        p.Init(position, velocity, c);
        return p;
    }

    public static Grid_3D CreateNewGrid(int resolution)
    {
        Grid_3D grid = ScriptableObject.CreateInstance("Grid_3D") as Grid_3D;
        grid.Init(resolution);
        return grid;
    }

    public static Grid_3D CreateNewGrid(int resolution, int zResolution)
    {
        Grid_3D grid = ScriptableObject.CreateInstance("Grid_3D") as Grid_3D;
        grid.Init(resolution, zResolution);
        return grid; // TODO delete this
    }

    public static int GetSphereID()
    {
        return sphereID;
    }

    // Mainly for testing purposes
    public static void ResetSphereID()
    {
        sphereID = 0;
    }
}
