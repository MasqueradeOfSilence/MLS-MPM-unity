using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/**
 * This will be where we test each phase of the sim in-depth
 */
[TestFixture]
public class FFFOptimized_3D_Test 
{
    [Test]
    public void InitShouldSetUpGridAndParticles()
    {
        FFF_Optimized_3D fff = GameObject.Find("ExampleGeo").AddComponent<FFF_Optimized_3D>();
        fff.Init();
        int3 expectedGridSize = new(16);
        int expectedNumParticles = 4096;
        int3 actualGridSize = fff.GetGrid().GetSize();
        Assert.AreEqual(expectedGridSize, actualGridSize);
        int actualNumParticles = fff.GetParticles().Length;
        Assert.AreEqual(expectedNumParticles, actualNumParticles);
    }

    [Test]
    public void InitTempGridShouldInitializeParticlesInTheCenter()
    {
        FFF_Optimized_3D fff = GameObject.Find("ExampleGeo").AddComponent<FFF_Optimized_3D>();
        double3[] tempGrid = fff.InitTempGrid();
        double3 expectedFirstPosition = new(4);
        double3 expectedLastPosition = new(11.5);
        double3 actualFirstPosition = tempGrid[0];
        double3 actualLastPosition = tempGrid[^1];
        Assert.AreEqual(expectedFirstPosition, actualFirstPosition);
        Assert.AreEqual(expectedLastPosition, actualLastPosition);
        double expectedLength = 4096;
        double actualLength = tempGrid.Length;
        Assert.AreEqual(expectedLength, actualLength);
    }

    [Test]
    public void ClearGridShouldResetScratchpadAndMakeEverythingZero()
    {
        double3 expectedFirstCellVelocity = new(0);
        double3 expectedLastCellVelocity = new(0);
        double expectedFirstCellMass = 0;
        double expectedLastCellMass = 0;
        int3 firstCellCoordinate = new(0);
        int3 lastCellCoordinate = new(15);
        FFF_Optimized_3D fff = GameObject.Find("ExampleGeo").AddComponent<FFF_Optimized_3D>();
        fff.ClearGrid();
        Cell_3D firstCell = fff.GetGrid().At(firstCellCoordinate);
        Cell_3D lastCell = fff.GetGrid().At(lastCellCoordinate);
        double actualFirstCellMass = firstCell.GetMass();
        double actualLastCellMass = lastCell.GetMass();
        Assert.AreEqual(expectedFirstCellMass, actualFirstCellMass);
        Assert.AreEqual(expectedLastCellMass, actualLastCellMass);
        double3 actualFirstCellVelocity = firstCell.GetVelocity();
        double3 actualLastCellVelocity = lastCell.GetVelocity();
        Assert.AreEqual(expectedFirstCellVelocity, actualFirstCellVelocity);
        Assert.AreEqual(expectedLastCellVelocity, actualLastCellVelocity);
    }

    [Test]
    public void P2G1ShouldModifyEachGridCellUsingParticleAttributes()
    {
        // TODO no neighbor logic, this is not actually testing the full functionality!

        FFF_Optimized_3D fff = GameObject.Find("ExampleGeo").AddComponent<FFF_Optimized_3D>();
        int neighborDim = 1;
        fff.SetNeighborDimension(neighborDim);
        Particle_3D[] particles = new Particle_3D[1];
        double3 placeholder = new(0);
        double mass = 2;
        double3x3 c = new(0);
        Particle_3D p = GeometryCreator_3D.CreateNewParticle(placeholder, placeholder, mass, c);
        particles[0] = p;
        fff.SetParticles(particles);
        double expectedNewMass = 0;
        double3 expectedNewVelocity = new(0);
        fff.ParticleToGridStep1();
        Cell_3D firstCell = fff.GetGrid().At(0);
        double actualNewMass = firstCell.GetMass();
        double3 actualNewVelocity = firstCell.GetVelocity();
        Assert.AreEqual(expectedNewMass, actualNewMass);
        Assert.AreEqual(expectedNewVelocity, actualNewVelocity);
    }
}
