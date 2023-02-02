using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class FluidSimulatorTestsForSphereMovement
{
    [OneTimeSetUp]
    public void LoadScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator FluidSimulatorShouldMoveAroundParticlesWithEachUpdate()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
        GameObject sphere0 = GameObject.Find("Sphere0");
        GameObject sphere4095 = GameObject.Find("Sphere4095");
        Assert.IsNotNull(sphere0);
        Assert.IsNotNull(sphere4095);
        Vector3 position0Initial = sphere0.transform.position;
        Vector3 position4095Initial = sphere4095.transform.position;
        yield return null;
        sphere0 = GameObject.Find("Sphere0");
        sphere4095 = GameObject.Find("Sphere4095");
        Vector3 position0Updated = sphere0.transform.position;
        Vector3 position4095Updated = sphere4095.transform.position;
        Assert.AreNotEqual(position0Initial, position0Updated);
        Assert.AreNotEqual(position4095Initial, position4095Updated);
    }
}
