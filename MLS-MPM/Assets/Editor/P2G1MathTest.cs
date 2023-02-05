using NUnit.Framework;
using Unity.Mathematics;

[TestFixture]
public class P2G1MathTest
{
    [Test]
    public void ComputeWeight0ShouldCorrectlyCalculateWeight0()
    {
        double[] expectedWeight0 = { 0.32, 0.5 };
        double[] distanceFromParticleToCell = { -0.3, -0.5 };
        double[] actualWeight0 = P2G1Math.ComputeWeight0(distanceFromParticleToCell);
        Assert.That(expectedWeight0, Is.EqualTo(actualWeight0).Within(0.01));
    }

    [Test]
    public void ComputeWeight1ShouldCorrectlyCalculateWeight1()
    {
        double[] expectedWeight1 = { 0.66, 0.5 };
        double[] distanceFromParticleToCell = { -0.3, -0.5 };
        double[] actualWeight1 = P2G1Math.ComputeWeight1(distanceFromParticleToCell);
        Assert.That(expectedWeight1, Is.EqualTo(actualWeight1).Within(0.01));
    }

    [Test]
    public void ComputeWeight2ShouldCorrectlyCalculateWeight2()
    {
        double[] expectedWeight2 = { 0.02, 0 };
        double[] distanceFromParticleToCell = { -0.3, -0.5 };
        double[] actualWeight2 = P2G1Math.ComputeWeight2(distanceFromParticleToCell);
        Assert.That(expectedWeight2, Is.EqualTo(actualWeight2).Within(0.01));
    }

    [Test]
    public void ComputeAllWeightsShoulProperlyComputeAllWeights()
    {
        double[] weight0 = { 0.32, 0.5 };
        double[] weight1 = { 0.66, 0.5 };
        double[] weight2 = { 0.02, 0 };
        double[][] expectedWeights = { weight0, weight1, weight2 };
        double[] distanceFromParticleToCell = { -0.3, -0.5 };
        double[][] actualWeights = P2G1Math.ComputeAllWeights(distanceFromParticleToCell);
        Assert.That(expectedWeights, Is.EqualTo(actualWeights).Within(0.01));
    }

    [Test]
    public void ComputeDistanceFromCurrentParticleToCurrentNeighborShouldCorrectlyCalculateDistance()
    {
        double[] expectedDistance = { -0.7, -0.5 };
        int[] neighborPosition = { 2, 5 };
        double[] particlePosition = { 3.2, 6 };
        double[] actualDistance = P2G1Math.ComputeDistanceFromCurrentParticleToCurrentNeighbor(neighborPosition, particlePosition);
        Assert.That(expectedDistance, Is.EqualTo(actualDistance).Within(0.01));
    }

    [Test]
    public void ComputeQShouldCorrectlyCalculateTheQ2x2Matrix()
    {
        double2x2 C = new double2x2(0.1, 1, 0.2, 2);
        double2 distanceFromCurrentParticleToCurrentNeighbor = new double2(-0.7, -0.5);
        double2 Q = P2G1Math.ComputeQ(C, distanceFromCurrentParticleToCurrentNeighbor);
        double2 expectedQ = new(-0.57, -1.14);
        Assert.That(Q, Is.EqualTo(expectedQ).Within(0.01));
    }

    [Test]
    public void ComputeMassContributionShouldCorrectlyCalculateTheMassContribution()
    {
        double expectedMassContribution = 0.16;
        double particleMass = 1;
        double weight = 0.16;
        double actualMassContribution = P2G1Math.ComputeMassContribution(weight, particleMass);
        Assert.That(expectedMassContribution, Is.EqualTo(actualMassContribution));
    }

    [Test]
    public void RecomputeCellMassAndReturnItShouldReturnAnUpdatedCellMass()
    {
        double initialCellMass = 1;
        double massContribution = 0.16;
        double expectedUpdatedCellMass = 1.16;
        // recompute cell mass because it just returns the updated value, which we will then update inside of a Cell object
        double actualUpdatedCellMass = P2G1Math.RecomputeCellMassAndReturnIt(initialCellMass, massContribution);
        Assert.That(expectedUpdatedCellMass, Is.EqualTo(actualUpdatedCellMass).Within(0.01));
    }

    [Test]
    public void RecomputeCellVelocityAndReturnItShouldReturnAnUpdatedCellVelocity()
    {
        // Add momentum to update the cell velocity.
        // Momentum is computed as mass contribution * (particle velocity + Q)
        double massContribution = 0.16;
        double2 particleVelocity = new(2, 2);
        double2 expectedNewCellVelocity = new(1.2288, 1.1376);
        double2 Q = new(-0.57, -1.14);
        double2 oldCellVelocity = new(1, 1);
        double2 actualNewCellVelocity = P2G1Math.RecomputeCellVelocityAndReturnIt(massContribution, particleVelocity, Q, oldCellVelocity);
        Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedNewCellVelocity, actualNewCellVelocity));
    }
}
