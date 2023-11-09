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
    }

    [Test]
    public void CreatePowerDiagramShouldBuildAnAdditivelyWeightedVoronoiDiagram()
    {

    }
}
