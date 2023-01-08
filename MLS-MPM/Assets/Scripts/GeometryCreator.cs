using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Utility class to spawn geometry.
 */
public class GeometryCreator: MonoBehaviour
{
    private static int sphereId = 0;
    /*
     * Currently using tiny spheres to represent particles. 
     * May need to turn off collider physics.
     * Currently a Vector2, but will be extended to a Vector3.
     * Need to have another function that uses Particles on top
     */
    public static GameObject SpawnParticleSphere(Vector2 location)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = location;
        // Fix Z at 0 for now
        sphere.transform.localScale = new Vector3(location.x, location.y, 0f);
        sphere.name = "Sphere" + sphereId.ToString();
        sphereId++;
        return sphere;
    }
}
