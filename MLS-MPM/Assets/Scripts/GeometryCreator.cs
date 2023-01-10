using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Utility class to spawn geometry.
 */
public class GeometryCreator: MonoBehaviour
{
    private static int sphereID = 0;

    /*
     * Currently using tiny spheres to represent particles. 
     * May need to turn off collider physics.
     * Currently a Vector2, but will be extended to a Vector3 in the 3D Version (to be coded).
     * Need to have another function that uses Particles on top
     */
    public static GameObject SpawnParticleSphere_2DVersion(Vector2 location)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = new Vector3(location.x, location.y, 0);
        // Fix Z at 0 for now
        sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        sphere.name = "Sphere" + sphereID.ToString();
        sphereID++;
        return sphere;
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
