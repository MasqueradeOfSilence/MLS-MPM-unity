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

    public void DumpParticlesIntoScene(Particle[] particles)
    {
        GameObject[] particleSpheres = GeometryCreator.SpawnFinalParticleSpheres(particles);
        listOfParticleSpheres = particleSpheres;
        AddAllParticles();
    }

    public void UpdateParticles(Particle[] particles)
    {
        // should be equal lengths
        for (int i = 0; i < listOfParticleSpheres.Length; i++)
        {
            GameObject currentParticleSphere = listOfParticleSpheres[i];
            Particle currentParticle = particles[i];
            currentParticleSphere.transform.position = new Vector3((float)currentParticle.GetPosition().x, (float)currentParticle.GetPosition().y, 0);
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
