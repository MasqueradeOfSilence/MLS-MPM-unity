using UnityEngine;
using NUnit.Framework;
using Unity.Mathematics;

[TestFixture]
public class GameInterfaceTests : MonoBehaviour
{
    private GameInterface gameInterface;

    // Most of these tests should be completed on PlayMode, not EditMode.
    [Test]
    public void DumpParticlesIntoSceneShouldTellUpdateToAddParticles()
    {
        gameInterface = GameObject.Find("ExampleGeo").AddComponent<GameInterface>();
        Vector2 testPosition = new Vector2(0, 0);
        Vector2 testVelocity = new Vector2(0, 1);
        double testMass = 1;
        double2x2 testC = new double2x2();
        Particle p1 = GeometryCreator.CreateNewParticle(testPosition, testVelocity, testMass, testC);
        Particle p2 = GeometryCreator.CreateNewParticle(testPosition, testVelocity, testMass, testC);
        Particle[] particles = new Particle[] { p1, p2 };
        gameInterface.DumpParticlesIntoScene(particles);
        Assert.IsTrue(gameInterface.GetIfWeNeedToAddAllTheParticles());
    }

    [Test]
    public void RemoveParticlesFromSceneShouldTellUpdateToRemoveParticles()
    {
        gameInterface.RemoveParticlesFromScene();
        Assert.IsTrue(gameInterface.GetIfWeNeedToNukeAllTheParticles());
    }

}
