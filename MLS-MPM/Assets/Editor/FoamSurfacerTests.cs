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
        VoronoiDiagram<Color> unweightedVoronoiDiagram = foamSurfacer.CreateUnweightedVoronoiDiagram(particles);
        Assert.IsNotNull(unweightedVoronoiDiagram);
        // TODO Also assert that it has vertices
    }

    [Test]
    public void BoundariesShouldBeComputedProperly()
    {
        Vector2 testPoint = new(15, 17);
        Rect testRect1 = new(0f, 0f, (float)32.53613, (float)32.15869);
        Assert.IsTrue(testRect1.Contains(testPoint));
        Rect testRect2 = new(15, (float)14.9450023777288, (float)32.53613, (float)32.15869);
        Assert.IsTrue(testRect2.Contains(testPoint));
    }

    [Test]
    public void CreateWeightedVoronoiDiagramShouldBuildAnAdditivelyWeightedVoronoiDiagram()
    {
        FoamSurfacer foamSurfacer = GameObject.Find("ExampleGeo").AddComponent<FoamSurfacer>();

        // Now let's build some particles
        FoamSimulator foamSimulator = GameObject.Find("ExampleGeo").AddComponent<FoamSimulator>();
        foamSimulator.InitializeFoamSimulator();
        foamSimulator.InitializeParticlesWithFluidAtBottom();
        Particle[,] particles = foamSimulator.GetParticles();
        VoronoiDiagram<Color> weightedVD = foamSurfacer.CreateWeightedVoronoiDiagram(particles);
        Assert.IsNotNull(weightedVD);
        // TODO assert it has vertices and is not equivalent to unweighted
    }
}
