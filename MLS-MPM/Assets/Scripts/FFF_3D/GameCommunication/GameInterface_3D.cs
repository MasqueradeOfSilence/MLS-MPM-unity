using UnityEngine;

public class GameInterface_3D : MonoBehaviour
{
    /**
     * Data members
     */
    private GameObject[] particleSphereList;
    private const double macroscopicThreshold = 0.7;
    private const string defaultMaterial = "ClearBubbleTest";
    private const string voronoiMaterial = "FFFBubbles";

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

    public void DumpParticlesIntoScene(Particle_3D[] particles, bool shouldUseFFFShader = false)
    {
        GameObject[] particleSpheres = GeometryCreator_3D.SpawnFinalParticleSpheres(particles, shouldUseFFFShader);
        particleSphereList = particleSpheres;
        AddAllParticles();
    }

    public void UpdateParticles(Particle_3D[] particles, bool fffMaterial = false)
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
                if (radius >= macroscopicThreshold)
                {
                    Material mat = Resources.Load(defaultMaterial, typeof(Material)) as Material;
                    if (fffMaterial)
                    {
                        mat = Resources.Load(voronoiMaterial, typeof(Material)) as Material;
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
        // Try again
        Material cubeMat = Resources.Load("AnotherTestMat", typeof(Material)) as Material;
        GameObject cube = GameObject.Find("Cube");
        // not sure if this will repro the issue since it already has that assigned, but let's try it
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
        // It still turns cyan! Whaaaat (Note: This only turns cyan if the material is not originally assigned)
        GameObject thereIsAnother = GameObject.Find("ThereIsAnother");
        thereIsAnother.GetComponent<Renderer>().sharedMaterial = myTest;
        // Try: Commenting this out and assigning it initially
        // Yes, if I assign it initially, it does not turn cyan. Let's see if we can turn it a different color instead. Like basically this could be resetting it
        // but idk why it didn't mess up the other 2D one?

        /*
         * So basically, the above works properly IF I assign it the material initially, and then do NOT reassign the sharedMaterial here. 
         * 
         * However, if I just remove the reassignment with FFF, it breaks *and* everything still is cyan. Whereas in 2D, it is all pink as expected.
         * 
         * What if we assign it first and ALSO uncomment it? It still works. So actually, the thing is that the material is assigned and doesn't stick, possibly. 
         * When we assign it beforehand, it is totally fine. 
         * What do the 2D particles get initialized with?
         */

        // didn't assign it initially here and it just worked...IF the mat is myTest
        // If it's sphereMat, we absolutely get a cyan capsule
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
