using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Mathematics;

[TestFixture]
public class MathUtils_3D_Test 
{
    /**
     * Utility
     */
    public void AssertApproximatelyEqual(double3 a, double3 b)
    {
        Assert.That((float3)a, Is.EqualTo((float3)b).Within(0.01));
    }

    public void AssertApproximatelyEqual(List<double3> a, List<double3> b)
    {
        Assert.IsTrue(a.Count == b.Count);
        for (int i = 0; i < a.Count; i++) 
        {
            double3 current = a[i];
            double3 current2 = b[i];
            AssertApproximatelyEqual(current, current2);
        }
    }

    /**
     * Common
     */

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
        double3 particlePosition = new(3.2, 6, 8.7);
        int3 cellPosition = new(3, 6, 9);
        double3 expectedDistance = new(-0.3, -0.5, -0.8);
        double3 actualDistance = MathUtils_3D.ComputeDistanceFromParticleToCell(particlePosition, cellPosition);
        AssertApproximatelyEqual(expectedDistance, actualDistance);
    }

    [Test]
    public void ComputeWeight0ShouldComputeCorrectly()
    {
        double3 distanceFromParticleToCell = new(-0.3, -0.5, -0.1);
        double3 expectedWeight0 = new(0.32, 0.5, 0.18);
        double3 actualWeight0 = MathUtils_3D.ComputeWeight0(distanceFromParticleToCell);
        AssertApproximatelyEqual(expectedWeight0, actualWeight0);
    }

    [Test]
    public void ComputeWeight1ShouldComputeCorrectly() 
    {
        double3 distanceFromParticleToCell = new(-0.3, -0.5, -0.1);
        double3 expectedWeight1 = new(0.66, 0.5, 0.74);
        double3 actualWeight1 = MathUtils_3D.ComputeWeight1(distanceFromParticleToCell);
        AssertApproximatelyEqual(expectedWeight1, actualWeight1);
    }

    [Test]
    public void ComputeWeight2ShouldComputeCorrectly()
    {
        double3 distanceFromParticleToCell = new(-0.3, -0.5, -0.1);
        double3 expectedWeight2 = new(0.02, 0, 0.08);
        double3 actualWeight2 = MathUtils_3D.ComputeWeight2(distanceFromParticleToCell);
        AssertApproximatelyEqual(expectedWeight2, actualWeight2);
    }

    [Test]
    public void ComputeAllWeightsShouldComputeWeights0Through2()
    {
        double3 distanceFromParticleToCell = new(-0.3, -0.5, -0.1);
        double3 weight0 = new(0.32, 0.5, 0.18);
        double3 weight1 = new(0.66, 0.5, 0.74);
        double3 weight2 = new(0.02, 0, 0.08);
        List<double3> expectedWeights = new()
        {
            weight0, weight1, weight2
        };
        List<double3> actualWeights = MathUtils_3D.ComputeAllWeights(distanceFromParticleToCell);
        AssertApproximatelyEqual(expectedWeights, actualWeights);
        // Printing values
        foreach(double3 d in actualWeights)
        {
            TestContext.WriteLine(d);
        }
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

    [Test]
    public void ComputeDistanceFromParticleToNeighborShouldComputeCorrectly()
    {
        int3 neighbor = new(2, 5, 1);
        double3 particle = new(3.2, 6, 1.4);
        double3 expectedDistance = new(-0.7, -0.5, 0.1);
        double3 actualDistance = MathUtils_3D.ComputeDistanceFromParticleToNeighbor(neighbor, particle);
        AssertApproximatelyEqual(expectedDistance, actualDistance);
    }

    /**
     * P2G1
     */
    [Test]
    public void ComputeQShouldComputeCorrectly()
    {
        double3x3 C = new(0.1, 1, 0.2, 2, 2, 0.2, 1, 0.7, 0.8);
        double3 distance = new(-0.7, -0.5, 0.1);
        double3 expectedQ = new(-0.55, -2.38, -0.97);
        double3 actualQ = MathUtils_3D.ComputeQ(C, distance);
        AssertApproximatelyEqual(expectedQ, actualQ);
        TestContext.WriteLine(actualQ);
    }

    [Test]
    public void ComputeMassContributionShouldComputeCorrectly()
    {
        double expectedMassContribution = 0.16;
        double particleMass = 1;
        double weight = 0.16;
        double actualMassContribution = MathUtils_3D.ComputeMassContribution(weight, particleMass);
        AssertApproximatelyEqual(expectedMassContribution, actualMassContribution);
    }

    [Test]
    public void UpdateMassShouldAddContributionToMass()
    {
        double initialCellMass = 1;
        double massContribution = 0.16;
        double expectedUpdatedCellMass = 1.16;
        double actualUpdatedCellMass = MathUtils_3D.UpdateMass(initialCellMass, massContribution);
        AssertApproximatelyEqual(expectedUpdatedCellMass, actualUpdatedCellMass);
    }

    [Test]
    public void UpdateVelocityShouldModifyVelocity()
    {
        double massContribution = 0.16;
        double3 particleVelocity = new(2, 2, 2);
        double3 oldCellVelocity = new(3, 3, 3);
        double3 Q = new(-0.55, -2.38, -0.97);
        double3 expectedNewVelocity = new(3.232, 2.9392, 3.1648);
        double3 actualNewVelocity = MathUtils_3D.UpdateVelocity(massContribution, particleVelocity, Q, oldCellVelocity);
        AssertApproximatelyEqual(expectedNewVelocity, actualNewVelocity);
    }

    /**
     * P2G2
     */

    [Test]
    public void UpdateDensityShouldIncorporateMassAndWeight()
    {

    }
}
