using UnityEngine;

public class GameInterface_3D : MonoBehaviour
{
    /**
     * Data members
     */
    private GameObject[] particleSphereList;
    private const double macroscopicThreshold = 0.8;
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
        for (int i = 0; i < particleSphereList.Length; i++)
        {
            GameObject currentParticleSphere = particleSphereList[i];
            Particle_3D currentParticle = particles[i];
            currentParticleSphere.transform.position = new Vector3((float)currentParticle.GetPosition().x, (float)currentParticle.GetPosition().y, (float)currentParticle.GetPosition().z);
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
                    currentParticleSphere.GetComponent<MeshRenderer>().material = mat;
                    currentParticleSphere.GetComponent<Renderer>().material = mat;
                    currentParticleSphere.GetComponent<Renderer>().sharedMaterial = mat;
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
