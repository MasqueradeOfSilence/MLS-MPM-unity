using NUnit.Framework;
using UnityEngine;
using Unity.Mathematics;
/*
 * Testing the GeometryCreator, which enables our math for MLS-MPM to integrate with the Unity game engine.
 */
[TestFixture]
public class GeometryCreatorTests
{
    [Test]
    public void BaseSphereShouldExist()
    {
        GameObject baseSphere = GameObject.Find("BaseSphere");
        Assert.IsNotNull(baseSphere);
    }

    [Test]
    public void BaseSphereShouldBeSmall()
    {
        GameObject baseSphere = GameObject.Find("BaseSphere");
        // local vs. lossy scale shouldn't matter right now
        Vector3 actualScale = baseSphere.transform.localScale;
        Vector3 expectedScale = new Vector3(0.1f, 0.1f, 0.1f);
        Assert.AreEqual(expectedScale, actualScale);
    }

    [Test]
    public void SpawnParticleSphereShouldCreateANewSphere()
    {
        // 2D vectors for now, which we convert to 3D, just for the system of spheres only right now. 
        GameObject newSphere = GeometryCreator.SpawnParticleSphere_2DVersion(new Vector2(0.1f, 0.2f));
        Assert.IsNotNull(newSphere);
        Assert.AreEqual(newSphere.transform.position, new Vector3(0.1f, 0.2f, 0f));
        Assert.AreEqual(newSphere.transform.localScale, new Vector3(0.1f, 0.1f, 0.1f));
        Assert.IsTrue(newSphere.name.Contains("Sphere"));
    }

    [Test]
    public void SphereIDShouldIncrementWhenANewSphereIsCreated()
    {
        GeometryCreator.ResetSphereID();
        int expectedID = 0;
        Assert.AreEqual(GeometryCreator.GetSphereID(), expectedID);
        GameObject newSphere = GeometryCreator.SpawnParticleSphere_2DVersion(new Vector2(0.1f, 0.2f));
        expectedID = 1;
        Assert.AreEqual(GeometryCreator.GetSphereID(), expectedID);
        GeometryCreator.ResetSphereID();
    }

    [Test]
    public void SpawnFinalSpheresShouldConvertParticlesIntoSphericalGameObjects()
    {
        Vector2 testPosition = new Vector2(0, 0);
        Vector2 testVelocity = new Vector2(0, 1);
        double testMass = 1;
        double2x2 testC = new double2x2();
        Particle p1 = GeometryCreator.CreateNewParticle(testPosition, testVelocity, testMass, testC);
        Particle p2 = GeometryCreator.CreateNewParticle(testPosition, testVelocity, testMass, testC);
        Particle[] particles = new Particle[] { p1, p2 };
        GameObject[] finalParticleSpheres = GeometryCreator.SpawnFinalParticleSpheres(particles);
        Assert.AreEqual(2, finalParticleSpheres.Length);
        GameObject particleSphere0 = finalParticleSpheres[0];
        GameObject particleSphere1 = finalParticleSpheres[1];
        Assert.IsTrue(particleSphere0.name.Contains("Sphere"));
        Assert.IsTrue(particleSphere1.name.Contains("Sphere"));
    }

    [Test]
    public void CreateNewParticleShouldCreateAParticleObject()
    {
        // Particle objects should have spheres underneath the hood. Abstracted out. 
        // There should be a few particle tests (possibly only one for the correct data members) but not in this file.
        Vector2 position = new Vector2(1, 2);
        Vector2 velocity = new Vector2(2, 3);
        double mass = 1;
        double2x2 c = new double2x2();
        Particle p = GeometryCreator.CreateNewParticle(position, velocity, mass, c);
        Assert.IsNotNull(p);
        Assert.AreEqual(position, p.GetPosition());
        Assert.AreEqual(velocity, p.GetVelocity());
        Assert.AreEqual(mass, p.GetMass());
        Assert.AreEqual(c, p.GetAffineMomentumMatrix());
    }

    // possible test here for adding particles into the actual scene

    // test for moving particles around the scene should have its own class
}
