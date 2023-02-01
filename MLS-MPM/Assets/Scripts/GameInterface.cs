using UnityEngine;

/*
 * Game Interface class: inserts particles directly into the Unity scene, as spherical GameObjects
 */

public class GameInterface: MonoBehaviour
{
    private GameObject[] listOfParticleSpheres;
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
