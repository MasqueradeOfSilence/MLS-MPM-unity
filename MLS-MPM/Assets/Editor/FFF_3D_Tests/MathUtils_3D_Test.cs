using NUnit.Framework;
using System.Collections.Generic;
using Unity.Mathematics;

[TestFixture]
public class MathUtils_3D_Test 
{
    [Test]
    public void ParticlePositionToCellPositionShouldCorrectlyConvert()
    {
        double3 particlePos = new(3.2, 6, 5.1);
        int3 expectedCellPos = new(3, 6, 5);
        int3 actualCellPos = MathUtils_3D.ParticlePositionToCellPosition(particlePos);
        Assert.AreEqual(expectedCellPos, actualCellPos);
    }

    [Test]
    public void ComputeDistanceFromParticleToCellShouldCorrectlyComputeDistance()
    {

    }

    [Test]
    public void ComputeWeightShouldCalculateTheFinalWeight()
    {
        double expectedWeight = 0.128;
        double3 weight0 = new(0.32, 0.5, 0.75);
        double3 weight1 = new(0.66, 0.5, 0.12);
        double3 weight2 = new(0.02, 0, 0.8);
        List<double3> weights = new() { 
            weight0, weight1, weight2 
        };
        int nx = 0;
        int ny = 1;
        int nz = 2;
        double actualWeight = MathUtils_3D.ComputeWeight(weights, nx, ny, nz);
        Assert.AreEqual(expectedWeight, actualWeight);
    }

    [Test]
    public void ComputeNeighborPositionShouldGetNeighboringCell()
    {
        int nx = 0;
        int ny = 0;
        int nz = 0;
        int3 cellPosition = new(3, 6, 9);
        int3 expectedNeighborPosition = new(2, 5, 8);
        int3 actualNeighborPosition = MathUtils_3D.ComputeNeighborPosition(cellPosition, nx, ny, nz);
        Assert.AreEqual(expectedNeighborPosition, actualNeighborPosition);
    }
}
