using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class VoronoiShaderDTO_3D : ScriptableObject
{
    private class ShaderSphere
    {
        public double3 center;
        public float radius;
        public ShaderSphere(double3 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }
    }
    private List<ShaderSphere> spheres;

    public void Init(List<Particle_3D> particles)
    {
        spheres = new List<ShaderSphere>();
        foreach (Particle_3D p in particles)
        {
            Bubble_3D b = p.GetBubble();
            if (b == null)
            {
                continue;
            }
            if (b.GetBubbleSize() == Bubble_3D.BubbleSize.SKIP) // could also add microscopic here
            {
                continue;
            }
            if (!b.IsInstantiated())
            {
                continue;
            }
            ShaderSphere shaderSphere = new(p.GetPosition(), b.GetRadius());
            spheres.Add(shaderSphere);
        }
    }

    public void UpdateVoronoiTexture()
    {
        // Grab a reference to the FFF shader -- NOTE, not all spheres may have it!!
        string sphereWithTexture = "Sphere27";
        GameObject sphere = GameObject.Find(sphereWithTexture);
        Material material = sphere.GetComponent<Renderer>().sharedMaterial;
        List<Vector4> sphereCenters = new();
        List<float> radii = new();
        foreach (ShaderSphere shaderSphere in spheres)
        {
            // 4th value is meaningless
            sphereCenters.Add(new Vector4((float)shaderSphere.center.x, (float)shaderSphere.center.y, (float)shaderSphere.center.z, 0));
            radii.Add(shaderSphere.radius);
            Debug.Log("Center: " + shaderSphere.center);
            Debug.Log("radius: " + shaderSphere.radius);
        }
        if (sphereCenters.Count <= 1 || radii.Count <= 1)
        {
            return;
        }
        material.SetVectorArray("_SphereCenters", sphereCenters);
        material.SetFloatArray("_SphereRadii", radii);
        material.SetInteger("_Count", spheres.Count);
    }
}
