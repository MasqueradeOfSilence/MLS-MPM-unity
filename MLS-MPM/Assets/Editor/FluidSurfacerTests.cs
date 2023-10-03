using NUnit.Framework;
using TriangleNet.Geometry;
using UnityEngine;

[TestFixture]
public class FluidSurfacerTests
{
    [Test]
    public void InitializePolygonShouldCreateAPolygonBasedOnASetOfPoints()
    {
        int expectedNumPoints = 4096;
        FoamSimulator foamSimulator = GameObject.Find("ExampleGeo").AddComponent<FoamSimulator>();
        foamSimulator.InitializeFoamSimulator();
        foamSimulator.InitializeParticlesWithFluidAtBottom();
        Polygon polygon = FluidSurfacer.InitializePolygon(foamSimulator.GetParticles());
        Assert.IsNotNull(polygon);
        Assert.AreEqual(polygon.Points.Count, expectedNumPoints);
    }
}
