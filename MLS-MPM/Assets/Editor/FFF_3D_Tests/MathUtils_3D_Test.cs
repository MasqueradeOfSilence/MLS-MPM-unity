using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
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

    public void AssertMatricesAreApproximatelyEqual(double3x3 a, double3x3 b)
    {
        AssertApproximatelyEqual(a.c0, b.c0);
        AssertApproximatelyEqual(a.c1, b.c1);
        AssertApproximatelyEqual(a.c2, b.c2);
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
        double gridCellMass = 3;
        double weight = 1.5376;
        double initialDensity = 0;
        double expectedUpdatedDensity = 4.6128;
        double actualUpdatedDensity = MathUtils_3D.UpdateDensity(weight, gridCellMass, initialDensity);
        AssertApproximatelyEqual(expectedUpdatedDensity, actualUpdatedDensity);
    }

    [Test]
    public void ComputeVolumeShouldCorrectlyCompute()
    {
        double particleMass = 2;
        double density = 4.6128;
        double expectedVolume = 0.433576136;
        double actualVolume = MathUtils_3D.ComputeVolume(particleMass, density);
        AssertApproximatelyEqual(expectedVolume, actualVolume);
    }

    [Test]
    public void ComputeTraceShouldCorrectlyCompute()
    {
        double3x3 strain = new(2, 3, 4, 1, 5, 8, 9, 5, 4);
        double expectedTrace = 11;
        double actualTrace = MathUtils_3D.ComputeTrace(strain);
        AssertApproximatelyEqual(expectedTrace, actualTrace);
    }

    [Test]
    public void ComputePressureShouldCorrectlyCompute()
    {
        double expectedPressure = 7.685549;
        double eosStiffness = 10;
        double density = 4.6128;
        double restDensity = 4;
        double eosPower = 4;
        double actualPressure = MathUtils_3D.ComputePressure(eosStiffness, density, restDensity, eosPower);
        AssertApproximatelyEqual(expectedPressure, actualPressure);
    }

    [Test]
    public void CreateStressMatrixShouldCorrectlyBuild()
    {
        double pressure = 7.6855;
        double3x3 expectedStress = new(-pressure, 0, 0, 0, -pressure, 0, 0, 0, -pressure);
        double3x3 actualStress = MathUtils_3D.CreateStressMatrix(pressure);
        AssertMatricesAreApproximatelyEqual(expectedStress, actualStress);
    }

    [Test]
    public void ComputeHerschelBulkleyStressShouldCorrectlyCompute()
    {
        double yieldStress_T0 = 31.9;
        double3x3 strain = new(2, 0, 0, 0, 2, 0, 0, 0, 2);
        double viscosity_mu = 27.2;
        double flowIndex_n = 0.22;
        double eosStiffness = 109;
        double density = 90;
        double restDensity = 77.7;
        int eosPower = 7;
        double3x3 expectedHBStress = new(-132.3329425379, 31.9, 31.9, 31.9, -132.3329425379, 31.9, 31.9, 31.9, -132.3329425379);
        double3x3 actualHBStress = MathUtils_3D.ComputeHerschelBulkleyStress(yieldStress_T0, strain, viscosity_mu, flowIndex_n, eosStiffness, density, restDensity, eosPower);
        Debug.Log(actualHBStress);
        AssertMatricesAreApproximatelyEqual(expectedHBStress, actualHBStress);
    }

    [Test]
    public void ComputeEquation16Term0ShouldCorrectlyCompute()
    {
        double3x3 stress = new(-7.4855, 0.7, 0.7, 0.7, -7.4855, 0.7, 0.7, 0.7, -7.4855);
        double volume = 1;
        double dt = 0.2;
        double3x3 expectedTerm = new(4.4913, -0.42, -0.42,
            -0.42, 4.4913, -0.42,
            -0.42, -0.42, 4.4913);
        double3x3 actualTerm = MathUtils_3D.ComputeEquation16Term0(stress, volume, dt);
        AssertMatricesAreApproximatelyEqual(expectedTerm, actualTerm);
    }

    [Test]
    public void ComputeMomentumShouldCorrectlyCompute()
    {
        double3x3 eq16Term0 = new(4.4913, -0.42, -0.42,
            -0.42, 4.4913, -0.42,
            -0.42, -0.42, 4.4913);
        double weight = 1.5376;
        //Debug.Log("First factor: " + eq16Term0 * weight);
        double3 distanceParticleToNeighbor = new(-0.5);
        double3 expectedMomentum = new(-2.80711944, -2.80711944, -2.80711944);
        double3 actualMomentum = MathUtils_3D.ComputeMomentum(eq16Term0, weight, distanceParticleToNeighbor);
        Assert.AreEqual(expectedMomentum, actualMomentum);
    }

    [Test]
    public void AddMomentumToVelocityShouldCorrectlyUpdate()
    {
        double3 initialCellVelocity = new(0, 1, 0);
        double3 momentum = new(3);
        double3 expectedUpdatedVelocity = new(3, 4, 3);
        double3 actualUpdatedVelocity = MathUtils_3D.AddMomentumToVelocity(momentum, initialCellVelocity);
        Assert.AreEqual(expectedUpdatedVelocity, actualUpdatedVelocity);
    }

    /**
     * G2P
     */
    [Test]
    public void ComputeWeightedVelocityShouldCorrectlyCompute()
    {
        double3 neighborVelocity = new(0, 1, 1);
        double weight = 0.16;
        double3 expectedWeightedVelocity = new(0, 0.16, 0.16);
        double3 actualWeightedVelocity = MathUtils_3D.ComputeWeightedVelocity(neighborVelocity, weight);
        Assert.AreEqual(expectedWeightedVelocity, actualWeightedVelocity);
    }

    [Test]
    public void ComputeTermShouldCorrectlyCompute()
    {
        double3 weightedVelocity = new(1, 2, 4);
        double3 distanceParticleToNeighbor = new(2, 3, 6);
        double3x3 expectedTerm = new(2, 4, 8,
            3, 6, 12,
            6, 12, 24);
        double3x3 actualTerm = MathUtils_3D.ComputeTerm(weightedVelocity, distanceParticleToNeighbor);
        Assert.AreEqual(expectedTerm, actualTerm);
    }

    [Test]
    public void UpdateBShouldCorrectlyUpdate()
    {
        double3x3 B = new(1);
        double3x3 term = new(4);
        double3x3 expectedB = new(5);
        double3x3 actualB = MathUtils_3D.UpdateB(B, term);
        Assert.AreEqual(expectedB, actualB);
    }

    [Test]
    public void AddWeightedVelocityShouldUpdateParticleVelocity()
    {
        double3 initialVelocity = new(2);
        double3 weightedVelocity = new(4);
        double3 expectedUpdatedVelocity = new(6);
        double3 actualUpdatedVelocity = MathUtils_3D.AddWeightedVelocity(initialVelocity, weightedVelocity);
        Assert.AreEqual(expectedUpdatedVelocity, actualUpdatedVelocity);
    }

    [Test]
    public void RecomputeCMatrixShouldCorrectlyCalculate()
    {
        double3x3 B = new(4);
        double3x3 expectedC = new(12); // may be 16 if *4
        double3x3 actualC = MathUtils_3D.RecomputeCMatrix(B);
        Assert.AreEqual(expectedC, actualC);
    }

    [Test]
    public void AdvectParticleShouldCorrectlyComputeNewPosition()
    {
        double3 initialPosition = new(2);
        double3 particleVelocity = new(10);
        double dt = 0.2;
        double3 expectedAdvectionPosition = new(4);
        double3 actualAdvectionPosition = MathUtils_3D.AdvectParticle(initialPosition, particleVelocity, dt);
        Assert.AreEqual(expectedAdvectionPosition, actualAdvectionPosition);
    }

    [Test]
    public void IsAirShouldReturnTrueForAirAndFalseForFluid()
    {
        double3 position = new(0);
        double3 velocity = new(0);
        double3x3 c = new(0);
        FluidParticle_3D pF = GeometryCreator_3D.CreateNewFluidParticle(position, velocity, c);
        AirParticle_3D pA = GeometryCreator_3D.CreateNewAirParticle(position, velocity, c);
        bool isFluidAir = MathUtils_3D.IsAir(pF);
        bool isAirAir = MathUtils_3D.IsAir(pA);
        Assert.IsFalse(isFluidAir);
        Assert.IsTrue(isAirAir);
    }
}
