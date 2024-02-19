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
        string sphereWithTexture = "Sphere106";
        GameObject sphere = GameObject.Find(sphereWithTexture);
        Material material = sphere.GetComponent<Renderer>().sharedMaterial;
        List<Vector4> sphereCenters = new();
        List<float> radii = new();
        foreach (ShaderSphere shaderSphere in spheres)
        {
            // 4th value is meaningless
            sphereCenters.Add(new Vector4((float)shaderSphere.center.x, (float)shaderSphere.center.y, (float)shaderSphere.center.z, 0));
            radii.Add(shaderSphere.radius);
        }
        if (sphereCenters.Count <= 1 || radii.Count <= 1)
        {
            return;
        }
        material.SetVectorArray("_SphereCenters", sphereCenters);
        material.SetFloatArray("_SphereRadii", radii);
        material.SetInteger("_Count", sphereCenters.Count);


        /// TEST
        GameObject cube = GameObject.Find("Cube");
        Material cubeMaterial = cube.GetComponent<Renderer>().sharedMaterial;
        cubeMaterial.SetFloatArray("_SphereRadii", radii);
        cubeMaterial.SetVectorArray("_SphereCenters", sphereCenters);
        //List<float> testNumbers = new()
        //{
        //    7f,
        //    666f
        //};
        //cubeMaterial.SetFloatArray("_TestNumbers", testNumbers);
        //cubeMaterial.SetInteger("_CountMe", 2);

        // TEST 2
        GameObject testingSphere1 = GameObject.Find("TestingSphere1");
        GameObject testingSphere2 = GameObject.Find("TestingSphere2");
        Material testingSphere1Mat = testingSphere1.GetComponent<Renderer>().sharedMaterial;
        Material testingSphere2Mat = testingSphere2.GetComponent<Renderer>().sharedMaterial;
        List<float> miniRadii = new()
        {
            1,
            1
        };
        List<Vector4> miniSphereCenters = new()
        {
            new Vector4(10.9499998f, 4.19869328f, -5.03000021f, 0),
            new Vector4(11.0299997f, 4.19869328f, -4.75f, 0)
        };
        testingSphere1Mat.SetFloatArray("_SphereRadii", miniRadii);
        testingSphere1Mat.SetVectorArray("_SphereCenters", miniSphereCenters);
        testingSphere2Mat.SetFloatArray("_SphereRadii", miniRadii);
        testingSphere2Mat.SetVectorArray("_SphereCenters", miniSphereCenters);

        GameObject yetAnotherTestCube = GameObject.Find("HiIAmATestCube");
        Material anotherTestCubeMat = yetAnotherTestCube.GetComponent<Renderer>().sharedMaterial;
        anotherTestCubeMat.SetFloatArray("_SphereRadii", radii);
        anotherTestCubeMat.SetVectorArray("_SphereCenters", sphereCenters);

        //anotherTestCubeMat.SetFloatArray("_SphereRadii", miniRadii);
        //anotherTestCubeMat.SetVectorArray("_SphereCenters", miniSphereCenters);
        GameObject thereIsAnother = GameObject.Find("ThereIsAnother");
        Material anotherMaterial = thereIsAnother.GetComponent<Renderer>().sharedMaterial;
        anotherMaterial.SetFloatArray("_SphereRadii", radii);
        anotherMaterial.SetVectorArray("_SphereCenters", sphereCenters);

        GameObject aCapsule = GameObject.Find("Capsule");
        Material capsuleMat = aCapsule.GetComponent<Renderer>().sharedMaterial;
        capsuleMat.SetFloatArray("_SphereRadii", radii);
        capsuleMat.SetVectorArray("_SphereCenters", sphereCenters);
    }
}
