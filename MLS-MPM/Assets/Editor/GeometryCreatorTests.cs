using NUnit.Framework;
using UnityEngine;

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
    public void CreateParticleShouldCreateAParticleObject()
    {
        // Particle objects should have spheres underneath the hood. Abstracted out. 
        // There should be a few particle tests (possibly only one for the correct data members) but not in this file.
    }

    [Test]
    public void SpawnParticleSphereShouldAddToAListOfParticles()
    {
        // should they be a list of spheres? or particle objects...?
        // we could also make a wrapper to give us back particles

    }
}
