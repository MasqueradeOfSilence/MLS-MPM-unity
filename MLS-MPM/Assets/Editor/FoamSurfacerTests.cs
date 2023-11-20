using NUnit.Framework;
using PixelsForGlory.VoronoiDiagram;
using UnityEngine;


[TestFixture]
public class FoamSurfacerTests 
{
    [Test]
    public void CreateUnweightedVoronoiDiagramShouldBuildAVoronoiDiagramUsingThePixelsForGloryLibrary()
    {
        FoamSurfacer foamSurfacer = GameObject.Find("ExampleGeo").AddComponent<FoamSurfacer>();

        // Now let's build some particles
        FoamSimulator foamSimulator = GameObject.Find("ExampleGeo").AddComponent<FoamSimulator>();
        foamSimulator.InitializeFoamSimulator();
        foamSimulator.InitializeParticlesWithFluidAtBottom();
        Particle[,] particles = foamSimulator.GetParticles();
        // grid resolution of 64, TODO do not hardcode me
        VoronoiDiagram<Color> unweightedVoronoiDiagram = foamSurfacer.CreateUnweightedVoronoiDiagram(particles, 64);
        Assert.IsNotNull(unweightedVoronoiDiagram);
        // TODO Also assert that it has vertices
    }

    [Test]
    public void BoundariesShouldBeComputedProperly()
    {
        /*
         * To fix the error
         * point (15, 17) out of diagram bounds (32.53613, 32.15869)
         */
        Vector2 testPoint = new(15, 17);
        Rect testRect1 = new(0f, 0f, (float)32.53613, (float)32.15869);
        // the problem is that it probably isn't starting at 0f, 0f
        Assert.IsTrue(testRect1.Contains(testPoint));
        // I bet it's going to have a different start/end point, let's see
        // lowest x: 15.4837641507968
        // lowest y: 14.9450023777288
        Rect testRect2 = new((float)15.4837641507968, (float)14.9450023777288, (float)32.53613, (float)32.15869);
        testRect2 = new((float)15, (float)14.9450023777288, (float)32.53613, (float)32.15869);
        Assert.IsTrue(testRect2.Contains(testPoint));
    }

    [Test]
    public void CreatePowerDiagramShouldBuildAnAdditivelyWeightedVoronoiDiagram()
    {

    }
}
