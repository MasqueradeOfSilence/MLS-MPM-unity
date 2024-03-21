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
