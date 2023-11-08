using NUnit.Framework;
using TriangleNet.Geometry;
using Unity.Mathematics;
using UnityEngine;

[TestFixture]
public class FluidSurfacerTests
{
    private Particle[,] BuildParticleList()
    {
        Particle[,] particles = new Particle[2, 3];
        FluidParticle p1 = ScriptableObject.CreateInstance("FluidParticle") as FluidParticle;
        particles[0, 0] = p1;
        p1.InitParticle(new double2(0, 0), new double2(0, 0), new double2x2(0, 0));
        FluidParticle p2 = ScriptableObject.CreateInstance("FluidParticle") as FluidParticle;
        FluidParticle p3 = ScriptableObject.CreateInstance("FluidParticle") as FluidParticle;
        FluidParticle p4 = ScriptableObject.CreateInstance("FluidParticle") as FluidParticle;
        FluidParticle p5 = ScriptableObject.CreateInstance("FluidParticle") as FluidParticle;
        FluidParticle p6 = ScriptableObject.CreateInstance("FluidParticle") as FluidParticle;
        p2.InitParticle(new double2(0, 1), new double2(0, 0), new double2x2(0, 0));
        p3.InitParticle(new double2(1, 2), new double2(0, 0), new double2x2(0, 0));
        p4.InitParticle(new double2(2, 1), new double2(0, 0), new double2x2(0, 0));
        p5.InitParticle(new double2(1, 3), new double2(0, 0), new double2x2(0, 0));
        p6.InitParticle(new double2(3, 3), new double2(0, 0), new double2x2(0, 0));
        particles[0, 1] = p2;
        particles[0, 2] = p3;
        particles[1, 0] = p4;
        particles[1, 1] = p5;
        particles[1, 2] = p6;
        return particles;
    }

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
    public void InitializeFluidSurfaceShouldSetUpTheFluidMesh()
    {
        FluidSurfacer fluidSurfacer = GameObject.Find("ExampleGeo").AddComponent<FluidSurfacer>();
        Particle[,] particles = BuildParticleList();
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "TestPlane";
        fluidSurfacer.SetPlane(plane);
        fluidSurfacer.InitializeFluidSurface(particles, true, false);
        Assert.IsNotNull(fluidSurfacer.GetFluidSurface());
    }
}
