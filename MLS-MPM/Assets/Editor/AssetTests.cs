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
        GameObject newSphere = GeometryCreator.SpawnParticleSphere(new Vector2(0.1f, 0.2f));
        Assert.IsNotNull(newSphere);
        //GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Assert.IsInstanceOf(typeof(PrimitiveType.Sphere), newSphere);
    }
}
