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

    private static GameObject ConstructSphereFromParticle(Particle_3D p, string shaderName = defaultClearMaterial, bool allFluid = false)
    {
        if (p.HasBubble())
        {
            float radius = p.GetBubble().ComputeUnitySphereRadius();
            return SpawnParticleSphere3D(p.GetPosition(), p.GetMass(), radius, shaderName, allFluid);
        }
        return SpawnParticleSphere3D(p.GetPosition(), p.GetMass(), materialName: shaderName, allFluid: allFluid);
    }

    public static GameObject SpawnParticleSphere3D(double3 location, double mass, float sphereSize = 0.01f, string materialName = defaultClearMaterial, bool allFluid = false)
    {
        bool isFoam = IsAir(mass);
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = new Vector3((float)location.x, (float)location.y, (float)location.z);
        sphere.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
        Material mat;
        bool renderingVideos = true; // Turn this on to hide the blue particles
        if (isFoam || allFluid)
        {
            mat = Resources.Load(materialName, typeof(Material)) as Material;
            if (renderingVideos)
            {
                sphere.GetComponent<MeshRenderer>().enabled = true;
                sphere.GetComponent<Renderer>().enabled = true;
            }
        }
        else
        {
            // It's a fluid
            mat = Resources.Load(fluidMatForViewport, typeof(Material)) as Material;
            if (renderingVideos)
            {
                // just choosing a random transparent one for now
                sphere.GetComponent<MeshRenderer>().enabled = false;
                sphere.GetComponent<Renderer>().enabled = false;
            }
        }
        sphere.GetComponent<MeshRenderer>().material = mat;
        sphere.GetComponent<Renderer>().material = mat;

        sphere.name = "Sphere" + sphereID.ToString();
        sphereID++;
        return sphere;
    }

    public static GameObject[] SpawnFinalParticleSpheres(Particle_3D[] particles, bool shouldUseFFFShader = false, bool shouldUseWhiteShader = false, bool allFluid = false)
    {
        GameObject[] finalParticleSpheres = new GameObject[particles.Length];
        for (int i = 0; i < particles.Length; i++) 
        {
            Particle_3D p = particles[i];
            if (shouldUseFFFShader)
            {
                GameObject particleSphere = ConstructSphereFromParticle(p, "FFFBubbles");
                finalParticleSpheres[i] = particleSphere;
                if (particleSphere != null)
                {
                    finalParticleSpheres[i] = particleSphere;
                }
            }
            else if (shouldUseWhiteShader)
            {
                // Only foaming soap gets allFluid
                GameObject particleSphere = ConstructSphereFromParticle(p, "WhiteBubbleShader", allFluid);
                finalParticleSpheres[i] = particleSphere;
                if (particleSphere != null)
                {
                    finalParticleSpheres[i] = particleSphere;
                }
            }
            else
            {
                GameObject particleSphere = ConstructSphereFromParticle(p);
                if (particleSphere != null)
                {
                    finalParticleSpheres[i] = particleSphere;
                }
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
