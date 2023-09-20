using UnityEngine;
using Unity.Mathematics;

/*
 * Utility class to spawn geometry.
 */
public class GeometryCreator: MonoBehaviour
{
    private static int sphereID = 0;

    /*
     * Currently using tiny spheres to represent particles. 
     * May need to turn off collider physics.
     * Currently a Vector3 with a 0 for z, but will be extended to a full Vector3 in the 3D Version (to be coded).
     * Need to have another function that uses Particles on top
     */
    public static GameObject SpawnParticleSphere_2DVersion(double2 location, double mass, float sphereSize = 0.1f)
    {
        bool spawnClearBubbleSphere = false;
        AirParticle air = ScriptableObject.CreateInstance("AirParticle") as AirParticle;
        air.InitParticle(new double2(0), new double2(0), new double2x2(0));
        bool isMacroscopic = sphereSize > 0.11f; // TODO not working yet, non-micro bubbles are still blue
        // Consider rendering as particle system: https://stackoverflow.com/questions/66468212/unityis-there-a-way-to-set-the-position-of-every-particle-in-a-particle-system 
        // Particle system as fluid: https://oxgamestudio.wordpress.com/2014/12/18/use-particles-to-create-flowing-liquid-in-unity/
        if (mass == air.GetMass() || isMacroscopic)
        {
            spawnClearBubbleSphere = true;
        }
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = new Vector3((float)location.x, (float)location.y, 0);
        // Fix Z-position at 0 for now
        sphere.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
        Material materialForSphere;
        if (spawnClearBubbleSphere)
        {
            //materialForSphere = Resources.Load("AirTest", typeof(Material)) as Material;
            materialForSphere = Resources.Load("ClearBubbleTest", typeof(Material)) as Material;
        }
        else
        {
            materialForSphere = Resources.Load("FluidTest", typeof(Material)) as Material;
        }
        sphere.GetComponent<MeshRenderer>().material = materialForSphere;
        sphere.GetComponent<Renderer>().material = materialForSphere;
        sphere.name = "Sphere" + sphereID.ToString();
        sphereID++;
        return sphere;
    }

    // Note: Spawn may not be the best name, as we don't put them into the game until later
    public static GameObject[] SpawnFinalParticleSpheres(Particle[] particles)
    {
        GameObject[] finalParticleSpheres = new GameObject[particles.Length];
        for (int i = 0; i < particles.Length; i++)
        {
            Particle p = particles[i];
            GameObject particleSphere = p.ConstructSphereFromParticle();
            finalParticleSpheres[i] = particleSphere;
        }
        return finalParticleSpheres;
    }

    // Use this wrapper for the convoluted way Unity makes you do it
    public static Particle CreateNewParticle(double2 position, double2 velocity, double mass, double2x2 c)
    {
        Particle particle = ScriptableObject.CreateInstance("Particle") as Particle;
        particle.InitParticle(position, velocity, mass, c);
        return particle;
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
