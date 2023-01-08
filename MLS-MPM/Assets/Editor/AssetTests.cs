using NUnit.Framework;
using UnityEngine;

/*
 * Asset Tests: Tests for our various game objects
 * We will probably build classes on top of these...
 */
[TestFixture]
public class AssetTests
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
        GameObject newSphere = GeometryCreator.SpawnParticleSphere(new Vector2(0.1f, 0.2f));
        Assert.IsNotNull(newSphere);
        // test its basic sphere attributes: 3D position, name, etc.
        // double check on your numbering system
    }
}
