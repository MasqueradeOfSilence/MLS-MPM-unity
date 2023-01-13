using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

/*
 * Game Interface class: inserts particles directly into the Unity scene, as spherical GameObjects
 */

public class GameInterface: MonoBehaviour
{
    GameObject[] listOfParticleSpheres;
    int numIterations = 0;
    void Update()
    {
        // add: test for listOfPS being not null
        if (numIterations == 0)
        {
            numIterations++;
            Vector2 testPosition = new Vector2(0, 0);
            Vector2 testVelocity = new Vector2(0, 1);
            double testMass = 1;
            double2x2 testC = new double2x2();
            Particle p1 = GeometryCreator.CreateNewParticle(testPosition, testVelocity, testMass, testC);
            listOfParticleSpheres = new GameObject[] { p1.ConstructSphereFromParticle() };
            foreach (GameObject p in listOfParticleSpheres)
            {
                Instantiate(p);
            }
        }
    }
    public void DumpParticlesIntoScene(Particle[] particles)
    {
        // note: change the test for this. this is getting into actual playtime tests...
        GameObject[] particleSpheres = GeometryCreator.SpawnFinalParticleSpheres(particles);
        listOfParticleSpheres = particleSpheres;
        foreach (GameObject particleSphere in particleSpheres)
        {
            //Instantiate(particleSphere);
        }
    }
}
