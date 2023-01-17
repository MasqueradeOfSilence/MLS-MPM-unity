using UnityEngine;
using System;

/*
 * Game Interface class: inserts particles directly into the Unity scene, as spherical GameObjects
 */

public class GameInterface: MonoBehaviour
{
    private GameObject[] listOfParticleSpheres;
    private bool weNeedToAddAllTheParticles = false;
    private bool weNeedToNukeAllTheParticles = false;

    void Update()
    {
        // Do not remove particles from scene on the last iteration
        //if (weNeedToNukeAllTheParticles)
        //{
        //    print("Removing particles from scene");
        //    weNeedToNukeAllTheParticles = false;
        //    NukeAllParticles();
        //    Array.Clear(listOfParticleSpheres, 0, listOfParticleSpheres.Length);
        //}
        //else if (weNeedToAddAllTheParticles)
        //{
        //    print("Adding particles into scene");
        //    weNeedToAddAllTheParticles = false;
        //    AddAllParticles();
        //}
    }

    public void NukeAllParticles()
    {
        foreach (GameObject p in listOfParticleSpheres)
        {
            Destroy(p);
        }
        listOfParticleSpheres = new GameObject[0];
        Debug.Log("Just deleted all particles, there are: " + listOfParticleSpheres.Length);
    }

    public void AddAllParticles()
    {
        Debug.Log("Add all particles, there are: " + listOfParticleSpheres.Length);
        foreach (GameObject p in listOfParticleSpheres)
        {
            Instantiate(p);
        }
    }


    public void DumpParticlesIntoScene(Particle[] particles)
    {
        GameObject[] particleSpheres = GeometryCreator.SpawnFinalParticleSpheres(particles);
        listOfParticleSpheres = particleSpheres;
        weNeedToAddAllTheParticles = true;
        AddAllParticles();
    }

    public void RemoveParticlesFromScene()
    {
        weNeedToNukeAllTheParticles = true;
    }

    public bool GetIfWeNeedToAddAllTheParticles()
    {
        return weNeedToAddAllTheParticles;
    }

    public bool GetIfWeNeedToNukeAllTheParticles()
    {
        return weNeedToNukeAllTheParticles;
    }

    public GameObject[] GetListOfParticleSpheres()
    {
        return listOfParticleSpheres;
    }
}
