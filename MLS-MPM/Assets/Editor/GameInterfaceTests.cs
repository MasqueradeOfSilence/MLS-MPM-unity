using UnityEngine;
using NUnit.Framework;
using Unity.Mathematics;

[TestFixture]
public class GameInterfaceTests
{
    // Most of these tests should be completed on PlayMode, not EditMode.
    [Test]
    public void DumpParticlesIntoSceneShouldAddParticles()
    {
        GameInterface gameInterface = GameObject.Find("ExampleGeo").AddComponent<GameInterface>();
        double2 testPosition = new(0, 0);
        double2 testVelocity = new(0, 1);
        double testMass = 1;
        double2x2 testC = new double2x2();
        Particle p1 = GeometryCreator.CreateNewParticle(testPosition, testVelocity, testMass, testC);
        Particle p2 = GeometryCreator.CreateNewParticle(testPosition, testVelocity, testMass, testC);
        Particle[] particles = new Particle[] { p1, p2 };
        gameInterface.DumpParticlesIntoScene(particles);
        Assert.IsTrue(gameInterface.GetListOfParticleSpheres().Length > 0);
    }

    // Removing particles cannot be tested as-is on Edit Mode.

}
