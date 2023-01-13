using UnityEngine;
using NUnit.Framework;
using Unity.Mathematics;

[TestFixture]
public class GameInterfaceTests : MonoBehaviour
{
    // no
    private GameInterface gameInterface = new GameInterface();
    [Test]
    public void DumpParticlesIntoSceneShouldAddAListOfGameObjectsToTheCurrentScene()
    {
        Vector2 testPosition = new Vector2(0, 0);
        Vector2 testVelocity = new Vector2(0, 1);
        double testMass = 1;
        double2x2 testC = new double2x2();
        Particle p1 = GeometryCreator.CreateNewParticle(testPosition, testVelocity, testMass, testC);
        Particle p2 = GeometryCreator.CreateNewParticle(testPosition, testVelocity, testMass, testC);
        Particle[] particles = new Particle[] { p1, p2 };
        gameInterface.DumpParticlesIntoScene(particles);
        GameObject[] gameObjects = (GameObject[])FindObjectsOfType(typeof(GameObject));
        int actualNumberOfParticlesInScene = 0;
        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i].name.Contains("Sphere"))
            {
                print(gameObjects[i].name);
                actualNumberOfParticlesInScene++;
            }
        }
        int expectedNumberOfParticlesInScene = 2;
        // (expected, actual) format
        Assert.AreEqual(expectedNumberOfParticlesInScene, actualNumberOfParticlesInScene);
    }

    [Test]
    public void RemoveParticlesFromSceneShouldRemoveAllParticlesFromTheScene()
    {

    }
}
