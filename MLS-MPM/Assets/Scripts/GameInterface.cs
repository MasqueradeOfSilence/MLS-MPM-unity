using UnityEngine;

/*
 * Game Interface class: inserts particles directly into the Unity scene, as spherical GameObjects
 */

public class GameInterface: MonoBehaviour
{
    private GameObject[] listOfParticleSpheres;

    // Use this one to get rid of unwanted cloned spheres
    public void NukeClones()
    {
        foreach (GameObject p in listOfParticleSpheres)
        {
            Destroy(GameObject.Find(p.name + "(Clone)"));
        }
    }

    private void NukeAllParticles()
    {
        foreach (GameObject p in listOfParticleSpheres)
        {
            Destroy(GameObject.Find(p.name + "(Clone)"));
            Destroy(GameObject.Find(p.name));
        }
    }

    private void AddAllParticles()
    {
        foreach (GameObject p in listOfParticleSpheres)
        {
            Instantiate(p);
        }
    }

    public void DumpParticlesIntoScene(Particle[] particles, bool shouldUseFFFShader = false)
    {
        GameObject[] particleSpheres = GeometryCreator.SpawnFinalParticleSpheres(particles, shouldUseFFFShader);
        listOfParticleSpheres = particleSpheres;
        AddAllParticles();
    }

    public void UpdateParticles(Particle[] particles, bool fffMaterial = false)
    {
        // should be equal lengths
        for (int i = 0; i < listOfParticleSpheres.Length; i++)
        {
            GameObject currentParticleSphere = listOfParticleSpheres[i];
            Particle currentParticle = particles[i];
            currentParticleSphere.transform.position = new Vector3((float)currentParticle.GetPosition().x, (float)currentParticle.GetPosition().y, 0);
            if (currentParticle.GetBubble() != null)
            {
                float radius = currentParticle.GetBubble().ComputeUnitySphereRadius();
                double macroscopicThreshold = 0.8;
                if (radius >= macroscopicThreshold)
                {
                    Material materialForSphere = Resources.Load("ClearBubbleTest", typeof(Material)) as Material;
                    if (fffMaterial)
                    {
                        materialForSphere = Resources.Load("FFFBubbles", typeof(Material)) as Material;
                    }
                    currentParticleSphere.GetComponent<MeshRenderer>().material = materialForSphere;
                    currentParticleSphere.GetComponent<Renderer>().material = materialForSphere;
                    currentParticleSphere.GetComponent<Renderer>().sharedMaterial = materialForSphere;
                }
                currentParticleSphere.transform.localScale = new Vector3(radius, radius, radius);
            }
        }
    }

    public void RemoveParticlesFromScene()
    {
        NukeAllParticles();
        listOfParticleSpheres = new GameObject[0];
    }

    public GameObject[] GetListOfParticleSpheres()
    {
        return listOfParticleSpheres;
    }
}
