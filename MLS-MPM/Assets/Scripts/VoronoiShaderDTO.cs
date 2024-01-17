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
            Bubble b = p.GetBubble();
            if (b == null)
            {
                continue;
            }
            if (b.GetBubbleSize() == Bubble.BubbleSize.SKIP)
            {
                continue;
            }
            if (!b.IsInstantiated())
            {
                continue;
            }
            // Mild issue: ComputeUnitySphereRadius has a randomness factor. This should only be done ONCE
            // TODO: Bug -- If we do the randomness twice (ComputeUnitySphereRad instead of GetRad), we get a bunch of shattered glasslike fragments. If we only do it once, we see no Voronoi.
            ShaderSphere shaderSphere = new(p.GetPosition(), b.GetRadius());
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
            radii.Add(shaderSphere.radius); // TODO are these radii correct?
            Debug.Log(shaderSphere.radius);
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
