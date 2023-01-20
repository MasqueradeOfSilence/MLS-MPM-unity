using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

[TestFixture]
public class GeneralMathUtilsTests
{
    // Try to do in-function-call conversions as little as possible
    // And move all of the common functions OUT of P2G1

    [Test]
    public void DeepEqualsShouldCompareDouble2sForEquality()
    {
        double2 a = new(0.01, 0.02);
        double2 b = new(0.01, 0.02);
        double2 c = new(0.01, 0.03);
        Assert.IsTrue(GeneralMathUtils.DeepEquals(a, b));
        Assert.IsFalse(GeneralMathUtils.DeepEquals(b, c));
    }

    [Test]
    public void DeepEqualsShouldAlsoCompareDouble2x2sForEquality()
    {
        double2x2 a = new(0.01, 0.02, 0.03, 0.04);
        double2x2 b = new(0.01, 0.02, 0.03, 0.04);
        double2x2 c = new(0.01, 0.01, 0.03, 0.04);
        Assert.IsTrue(GeneralMathUtils.DeepEquals(a, b));
        Assert.IsFalse(GeneralMathUtils.DeepEquals(b, c));
    }

    [Test]
    public void ParticlePositionToCellPositionShouldCorrectlyConvert()
    {
        double[] particlePosition = { 3.2, 6 };
        int[] expectedCorrespondingCellPosition = { 3, 6 };
        int[] actualCorrespondingCellPosition = GeneralMathUtils.ParticlePositionToCellPosition(particlePosition);
        CollectionAssert.AreEqual(actualCorrespondingCellPosition, expectedCorrespondingCellPosition);
        // checking false case
        int[] unequal1 = { 0, 2 };
        int[] unequal2 = { 1, 2 };
        CollectionAssert.AreNotEqual(unequal1, unequal2);
    }

    [Test]
    public void ComputeDistanceFromParticleToCellShouldCorrectlyCalculateDistance()
    {
        double[] particlePosition = { 3.2, 6 };
        int[] correspondingCellPosition = { 3, 6 };
        double[] expectedDistance = { -0.3, -0.5 };
        double[] distanceFromParticleToCell = GeneralMathUtils.ComputeDistanceFromParticleToCell(particlePosition, correspondingCellPosition);
        Assert.That(expectedDistance, Is.EqualTo(distanceFromParticleToCell).Within(0.01));
    }

    [Test]
    public void ComputeWeightShouldUseWeights0Through2ToCalculateTheFinalWeight()
    {
        double expectedWeight = 0.16;
        double[] weight0 = { 0.32, 0.5 };
        double[] weight1 = { 0.66, 0.5 };
        double[] weight2 = { 0.02, 0 };
        double[][] weights = { weight0, weight1, weight2 };
        int nx = 0;
        int ny = 0;
        double actualWeight = GeneralMathUtils.ComputeWeight(weights, nx, ny);
        Assert.That(expectedWeight, Is.EqualTo(actualWeight).Within(0.01));
    }

    [Test]
    public void ComputeCurrentNeighborPositionShouldCorrectlyCalculateTheNeighborPosition()
    {
        int nx = 0;
        int ny = 0;
        int[] correspondingCellPosition = { 3, 6 };
        int[] expectedNeighborPosition = { 2, 5 };
        int[] actualNeighborPosition = GeneralMathUtils.ComputeNeighborPosition(correspondingCellPosition, nx, ny);
        Assert.That(expectedNeighborPosition, Is.EqualTo(actualNeighborPosition).Within(0.01));
    }

}
