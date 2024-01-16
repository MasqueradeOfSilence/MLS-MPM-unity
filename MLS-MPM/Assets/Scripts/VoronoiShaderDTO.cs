using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class VoronoiShaderDTO : ScriptableObject
{
    private class ShaderSphere
    {
        // TODO will need to change for 3D
        public double2 center;
        public float radius;
        public ShaderSphere(double2 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }
    }
    private List<ShaderSphere> spheres;
    public void Initialize(Particle[,] particles)
    {
        spheres = new List<ShaderSphere>();
        foreach (Particle p in particles)
        {
            if (p.GetBubble() == null)
            {
                continue;
            }
            ShaderSphere shaderSphere = new(p.GetPosition(), p.GetBubble().ComputeUnitySphereRadius());
            spheres.Add(shaderSphere);
        }
    }
    public void UpdateVoronoiTexture()
    {
        // Grab a reference to the FFF shader -- NOTE, not all spheres may have it!!
        GameObject sphere = GameObject.Find("Sphere27");
        Material material = sphere.GetComponent<Renderer>().sharedMaterial;
        List<Vector4> sphereCenters = new();
        List<float> radii = new();
        foreach (ShaderSphere shaderSphere in spheres)
        {
            // TODO change z-position when 3D, also 4th value is meaningless
            sphereCenters.Add(new Vector4((float)shaderSphere.center.x, (float)shaderSphere.center.y, 0, 0));
            radii.Add(shaderSphere.radius);
        }
        if (sphereCenters.Count == 0 || radii.Count == 0)
        {
            return;
        }
        material.SetVectorArray("_SphereCenters", sphereCenters);
        material.SetFloatArray("_SphereRadii", radii);
        material.SetInteger("_Count", sphereCenters.Count);
    }
}
