using NUnit.Framework;
using TriangleNet.Geometry;
using Unity.Mathematics;
using UnityEngine;

[TestFixture]
public class WaterSurfacer_3D_Test
{
    private Particle_3D[][][] BuildParticleListWithZAtZero()
    {
        Particle_3D[][][] particles = new Particle_3D[2][][]; // 2 x 3 x 1
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i] = new Particle_3D[3][];
            for (int j = 0; j < particles[i].Length; j++)
            {
                particles[i][j] = new Particle_3D[1];
            }
        }
        FluidParticle_3D p1 = ScriptableObject.CreateInstance("FluidParticle_3D") as FluidParticle_3D;
        particles[0][0][0] = p1;
        double3 defaultAtZero = new(0);
        double3x3 defaultCAtZero = new(0);
        p1.Init(defaultAtZero, defaultAtZero, defaultCAtZero);
        FluidParticle_3D p2 = ScriptableObject.CreateInstance("FluidParticle_3D") as FluidParticle_3D;
        FluidParticle_3D p3 = ScriptableObject.CreateInstance("FluidParticle_3D") as FluidParticle_3D;
        FluidParticle_3D p4 = ScriptableObject.CreateInstance("FluidParticle_3D") as FluidParticle_3D;
        FluidParticle_3D p5 = ScriptableObject.CreateInstance("FluidParticle_3D") as FluidParticle_3D;
        FluidParticle_3D p6 = ScriptableObject.CreateInstance("FluidParticle_3D") as FluidParticle_3D;
        p2.Init(new double3(0, 1, 0), defaultAtZero, defaultCAtZero);
        p3.Init(new double3(1, 2, 0), defaultAtZero, defaultCAtZero);
        p4.Init(new double3(2, 1, 0), defaultAtZero, defaultCAtZero);
        p5.Init(new double3(1, 3, 0), defaultAtZero, defaultCAtZero);
        p6.Init(new double3(3, 3, 0), defaultAtZero, defaultCAtZero);
        particles[0][1][0] = p2;
        particles[0][2][0] = p3;
        particles[1][0][0] = p4;
        particles[1][1][0] = p5;
        particles[1][2][0] = p6;
        return particles;
    }

    [Test]
    public void InitializePolygonShouldCreateAPolygonFromASetOfPoints()
    {
        int expectedNumPoints = 4096;
        FFF_3D fffSim = GameObject.Find("ExampleGeo").AddComponent<FFF_3D>();
        //fffSim.InitializeFoamSimulator();
        //fffSim.InitializeParticlesWithFluidAtBottom();
        //WaterSurfacer_3D fluidSurfacer = GameObject.Find("ExampleGeo").AddComponent<WaterSurfacer_3D>();
        //Polygon polygon = fluidSurfacer.InitializePolygon(fffSim.GetParticles());
        //Assert.IsNotNull(polygon);
    }
}
