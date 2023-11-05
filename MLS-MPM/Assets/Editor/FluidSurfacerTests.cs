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
        FluidSurfacer fluidSurfacer = GameObject.Find("ExampleGeo").AddComponent<FluidSurfacer>();
        Polygon polygon = fluidSurfacer.InitializePolygon(foamSimulator.GetParticles());
        Assert.IsNotNull(polygon);
        Assert.AreEqual(polygon.Points.Count, expectedNumPoints);
    }

    [Test]
    public void CreateMeshShouldTriangulateThePointsAndReturnAMesh()
    {
        int numParticles = 4096;
        FoamSimulator foamSimulator = GameObject.Find("ExampleGeo").AddComponent<FoamSimulator>();
        foamSimulator.InitializeFoamSimulator();
        foamSimulator.InitializeParticlesWithFluidAtBottom();
        FluidSurfacer fluidSurfacer = GameObject.Find("ExampleGeo").AddComponent<FluidSurfacer>();
        Polygon polygon = fluidSurfacer.InitializePolygon(foamSimulator.GetParticles());
        TriangleNet.TriangleNetMesh mesh = fluidSurfacer.CreateMesh(polygon);
        Assert.IsNotNull(mesh);
        // Should be quite a few more edges than particles, but not sure if this is the optimal test
        Assert.IsTrue(mesh.NumberOfEdges > numParticles * 2);
    }

    [Test]
    public void InstantiateMeshInUnityShouldCreateGameObjectsToDisplayMesh()
    {
        // this will include the actual game object instantation, so we will be able to see a thin layer of fluid underneath the bubbles
        // TODO I have not unit tested this yet :)
    }
}
