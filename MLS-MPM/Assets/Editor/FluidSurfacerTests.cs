using NUnit.Framework;
using TriangleNet.Geometry;
using UnityEngine;

[TestFixture]
public class FluidSurfacerTests
{
    [Test]
    public void InitializePolygonShouldCreateAPolygonBasedOnASetOfPoints()
    {
        FoamSimulator foamSimulator = GameObject.Find("ExampleGeo").AddComponent<FoamSimulator>();
        foamSimulator.InitializeFoamSimulator();
        foamSimulator.InitializeParticlesWithFluidAtBottom();
        Polygon polygon = FluidSurfacer.InitializePolygon(foamSimulator.GetParticles());
        Assert.IsNotNull(polygon);
    }
}
