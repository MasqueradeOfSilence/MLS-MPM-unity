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
        int current = 0;
        foreach (ShaderSphere shaderSphere in spheres)
        {
            // 4th value is meaningless
            sphereCenters.Add(new Vector4((float)shaderSphere.center.x, (float)shaderSphere.center.y, (float)shaderSphere.center.z, 0));
            radii.Add(shaderSphere.radius);
            //Debug.Log("Center: " + shaderSphere.center);
            //Debug.Log("Center to float: " + sphereCenters.ElementAt(current));
            //Debug.Log("radius: " + shaderSphere.radius);
            current++; // remove me, debug only
        }
        //Debug.Log("Count: " + sphereCenters.Count);
        if (sphereCenters.Count <= 1 || radii.Count <= 1)
        {
            return;
        }
        material.SetVectorArray("_SphereCenters", sphereCenters); // For some reason, these are turning into 0, 0, 0 when passed in, and it doesn't matter what w is
        //Vector4[] returnedCenters = material.GetVectorArray("_SphereCenters");
        //foreach(Vector4 v in returnedCenters) 
        //{
        //    Debug.Log("Vector: " + v); // despite this being correct, it is all zeroes in the shader
        // HYPOTHESIS: something LATER, after this class, resets it
        //}
        material.SetFloatArray("_SphereRadii", radii);
        // This is still zeroed out
        //Shader.SetGlobalVectorArray("_TheData", sphereCenters);
        material.SetInteger("_Count", sphereCenters.Count);


        // doesn't work, this still turns into zeroes somehow
        //var materialProperty = new MaterialPropertyBlock();
        //float[] floatArray = new float[] { 2f, 1f };
        //materialProperty.SetFloatArray("arrayName", floatArray);
        //sphere.GetComponent<Renderer>().SetPropertyBlock(materialProperty);


        //// doesn't do anything
        //Material mat2 = sphere.GetComponent<Renderer>().material;
        //mat2.SetVectorArray("_SphereCenters", sphereCenters);
        //mat2.SetFloatArray("_SphereRadii", radii);
        //mat2.SetInteger("_Count", sphereCenters.Count);


        /// TEST
        GameObject cube = GameObject.Find("Cube");
        Material cubeMaterial = cube.GetComponent<Renderer>().sharedMaterial;
        List<float> testNumbers = new()
        {
            7f,
            666f
        };
        cubeMaterial.SetFloatArray("_TestNumbers", testNumbers);
        cubeMaterial.SetInteger("_CountMe", 2);
    }
}
