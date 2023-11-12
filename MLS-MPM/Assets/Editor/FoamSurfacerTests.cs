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
    public void CreatePowerDiagramShouldBuildAnAdditivelyWeightedVoronoiDiagram()
    {

    }
}
