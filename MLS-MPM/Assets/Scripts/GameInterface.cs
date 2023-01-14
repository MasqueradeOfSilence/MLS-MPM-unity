using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
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
        // Need to make PlayMode tests for these
        if (weNeedToNukeAllTheParticles)
        {
            weNeedToNukeAllTheParticles = false;
            NukeAllParticles();
        }
        else if (weNeedToAddAllTheParticles)
        {
            weNeedToAddAllTheParticles = false;
            AddAllParticles();
        }
    }

    private void NukeAllParticles()
    {
        foreach (GameObject p in listOfParticleSpheres)
        {
            Destroy(p);
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
        weNeedToAddAllTheParticles = true;
    }

    public void RemoveParticlesFromScene()
    {
        Array.Clear(listOfParticleSpheres, 0, listOfParticleSpheres.Length);
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
