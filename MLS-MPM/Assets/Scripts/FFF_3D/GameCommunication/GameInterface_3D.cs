using UnityEngine;

public class GameInterface_3D : MonoBehaviour
{
    /**
     * Data members
     */
    private GameObject[] particleSphereList;
    private double macroscopicThreshold = 0.7;
    private const string defaultMaterial = "ClearBubbleTest";
    private const string voronoiMaterial = "FFFBubbles";
    // TODO: if FFF and White should not be true at the same time, we need to explicitly enforce that
    private const string whiteFoamMaterial = "WhiteBubbleShader";

    public void NukeClones()
    {
        foreach (GameObject p in particleSphereList)
        {
            Destroy(GameObject.Find(p.name + "(Clone)"));
        }
    }

    private void NukeAllParticles()
    {
        foreach (GameObject p in particleSphereList)
        {
            Destroy(GameObject.Find(p.name + "(Clone)"));
            Destroy(GameObject.Find(p.name));
        }
    }

    private void AddAllParticles()
    {
        foreach (GameObject p in particleSphereList)
        {
            Instantiate(p);
        }
    }

    public void DumpParticlesIntoScene(Particle_3D[] particles, bool shouldUseFFFShader = false, bool shouldUseWhiteShader = false, bool allFluid = false)
    {
        GameObject[] particleSpheres = GeometryCreator_3D.SpawnFinalParticleSpheres(particles, shouldUseFFFShader, shouldUseWhiteShader, allFluid);
        particleSphereList = particleSpheres;
        AddAllParticles();
    }

    public void UpdateParticles(Particle_3D[] particles, bool fffMaterial = false, bool whiteMaterial = false)
    {
        if (particles.Length != particleSphereList.Length)
        {
            Debug.LogError("OH NO! Lengths are not equal!");
        }
        
        for (int i = 0; i < particleSphereList.Length; i++)
        {
            GameObject currentParticleSphere = particleSphereList[i];
            Particle_3D currentParticle = particles[i];
            currentParticleSphere.transform.position = new Vector3((float)currentParticle.GetPosition().x, (float)currentParticle.GetPosition().y, (float)currentParticle.GetPosition().z);
            // only do if it hasn't been set yet (not 100% sure this will work)
            //if (currentParticleSphere.GetComponent<Renderer>().sharedMaterial.GetFloatArray("_SphereRadii")[0] == 0)
            //{
            //    if (currentParticle.GetBubble() != null)
            //    {
            //        float radius = currentParticle.GetBubble().ComputeUnitySphereRadius();
            //        if (radius >= macroscopicThreshold)
            //        {
            //            Material mat = Resources.Load(defaultMaterial, typeof(Material)) as Material;
            //            if (fffMaterial)
            //            {
            //                mat = Resources.Load(voronoiMaterial, typeof(Material)) as Material;
            //            }
            //            currentParticleSphere.GetComponent<MeshRenderer>().material = mat;
            //            currentParticleSphere.GetComponent<Renderer>().material = mat;
            //            currentParticleSphere.GetComponent<Renderer>().sharedMaterial = mat;
            //        }
            //        currentParticleSphere.transform.localScale = new Vector3(radius, radius, radius);
            //    }
            //}
            if (currentParticle.GetBubble() != null)
            {
                float radius = currentParticle.GetBubble().ComputeUnitySphereRadius();
                //if (whiteMaterial)
                //{
                //    // probably not necessary
                //    macroscopicThreshold = 0.3;
                //}
                if (radius >= macroscopicThreshold)
                {
                    Material mat = Resources.Load(defaultMaterial, typeof(Material)) as Material;
                    if (fffMaterial)
                    {
                        mat = Resources.Load(voronoiMaterial, typeof(Material)) as Material;
                    }
                    if (whiteMaterial)
                    {
                        mat = Resources.Load(whiteFoamMaterial, typeof(Material)) as Material;
                    }

                    //currentParticleSphere.GetComponent<MeshRenderer>().material = mat;
                    currentParticleSphere.GetComponent<Renderer>().material = mat;
                    currentParticleSphere.GetComponent<Renderer>().sharedMaterial = mat;
                    //if (currentParticleSphere.GetComponent<Renderer>().sharedMaterial.GetFloatArray("_SphereRadii")[0] == 0 || !fffMaterial)
                    //{
                    //    currentParticleSphere.GetComponent<MeshRenderer>().material = mat;
                    //    currentParticleSphere.GetComponent<Renderer>().material = mat;
                    //    currentParticleSphere.GetComponent<Renderer>().sharedMaterial = mat;
                    //}
                }
                currentParticleSphere.transform.localScale = new Vector3(radius, radius, radius);
            }
        }
        // TODO This is a bunch of debug test code that I need to remove
        Material cubeMat = Resources.Load("AnotherTestMat", typeof(Material)) as Material;
        GameObject cube = GameObject.Find("Cube");
        cube.GetComponent<MeshRenderer>().material = cubeMat;
        cube.GetComponent<Renderer>().material = cubeMat;
        cube.GetComponent<Renderer>().sharedMaterial = cubeMat;
        GameObject testingSphere1 = GameObject.Find("TestingSphere1");
        GameObject testingSphere2 = GameObject.Find("TestingSphere2");
        Material sphereMat = Resources.Load(voronoiMaterial, typeof(Material)) as Material;
        testingSphere1.GetComponent<MeshRenderer>().material = sphereMat;
        testingSphere1.GetComponent<Renderer>().material = sphereMat;
        testingSphere1.GetComponent<Renderer>().sharedMaterial = sphereMat;

        testingSphere2.GetComponent<MeshRenderer>().material = sphereMat;
        testingSphere2.GetComponent<Renderer>().material = sphereMat;
        testingSphere2.GetComponent<Renderer>().sharedMaterial = sphereMat;

        Material myTest = Resources.Load("Test_FFF", typeof(Material)) as Material;
        GameObject thereIsAnother = GameObject.Find("ThereIsAnother");
        thereIsAnother.GetComponent<Renderer>().sharedMaterial = myTest;
        GameObject aCapsule = GameObject.Find("Capsule");
        //aCapsule.GetComponent<Renderer>().sharedMaterial = myTest;
        //aCapsule.GetComponent<Renderer>().material = myTest;

        aCapsule.GetComponent<Renderer>().sharedMaterial = sphereMat;
        aCapsule.GetComponent<Renderer>().material = sphereMat;
    }

    public void RemoveParticlesFromScene()
    {
        NukeAllParticles();
        particleSphereList = new GameObject[0];
    }

    public GameObject[] GetParticleSphereList()
    {
        return particleSphereList;
    }
}
