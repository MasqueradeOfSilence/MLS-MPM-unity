using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

public class VolumeFractionCalculatorTests
{
    private Particle CreateParticleInUnity()
    {
        return ScriptableObject.CreateInstance("Particle") as Particle;
    }

    private Particle CreateParticleWithGivenPosition(double2 particlePosition)
    {
        // These need to be initialized, but their value won't affect much here 
        double2 initialVelocity = new(0, 0);
        double initialMass = 1;
        double2x2 initialC = new double2x2(0, 0, 0, 0);
        Particle particle = CreateParticleInUnity();
        particle.InitParticle(particlePosition, initialVelocity, initialMass, initialC);
        return particle;
    }

    [Test]
    public void FindParticlesThatIntersectWithCellShouldFindAllParticlesWithinACell()
    {
        // It will cast the position to an integer, and if they're equal, it's inside the cell. 
        // Note that a GridCell object does *not* have a position, as it's implicit from its location within the array, so we will pass it into the method as a double2 for now.
        int2 gridCellPosition = new(2, 12);
        // Positions that should be true (inside the cell)
        double2 particlePositionInside1 = new(2.1, 12);
        double2 particlePositionInside2 = new(2.5, 12);
        double2 particlePositionInside3 = new(2.9, 12);
        double2 particlePositionInside4 = new(2, 12);
        double2 particlePositionInside5 = new(2, 12.2);
        double2 particlePositionInside6 = new(2, 12.5);
        double2 particlePositionInside7 = new(2, 12.9);
        // Positions that should be false (outside the cell)
        double2 particlePositionOutside1 = new(3, 12);
        double2 particlePositionOutside2 = new(1, 12);
        double2 particlePositionOutside3 = new(2, 13);
        double2 particlePositionOutside4 = new(2, 11);

        Particle insideParticle1 = CreateParticleWithGivenPosition(particlePositionInside1);
        Particle insideParticle2 = CreateParticleWithGivenPosition(particlePositionInside2);
        Particle insideParticle3 = CreateParticleWithGivenPosition(particlePositionInside3);
        Particle insideParticle4 = CreateParticleWithGivenPosition(particlePositionInside4);
        Particle insideParticle5 = CreateParticleWithGivenPosition(particlePositionInside5);
        Particle insideParticle6 = CreateParticleWithGivenPosition(particlePositionInside6);
        Particle insideParticle7 = CreateParticleWithGivenPosition(particlePositionInside7);
        Particle outsideParticle1 = CreateParticleWithGivenPosition(particlePositionOutside1);
        Particle outsideParticle2 = CreateParticleWithGivenPosition(particlePositionOutside2);
        Particle outsideParticle3 = CreateParticleWithGivenPosition(particlePositionOutside3);
        Particle outsideParticle4 = CreateParticleWithGivenPosition(particlePositionOutside4);

        // It's not necessary to init a grid cell, since IsParticleInsideCell only needs a position, and we are not currently storing that as a data member within the cell.

        // Calculating booleans
        bool insideParticle1ShouldBeInsideTheCell = VolumeFractionCalculator.IsParticleInsideCell(insideParticle1, gridCellPosition);
        bool insideParticle2ShouldBeInsideTheCell = VolumeFractionCalculator.IsParticleInsideCell(insideParticle2, gridCellPosition);
        bool insideParticle3ShouldBeInsideTheCell = VolumeFractionCalculator.IsParticleInsideCell(insideParticle3, gridCellPosition);
        bool insideParticle4ShouldBeInsideTheCell = VolumeFractionCalculator.IsParticleInsideCell(insideParticle4, gridCellPosition);
        bool insideParticle5ShouldBeInsideTheCell = VolumeFractionCalculator.IsParticleInsideCell(insideParticle5, gridCellPosition);
        bool insideParticle6ShouldBeInsideTheCell = VolumeFractionCalculator.IsParticleInsideCell(insideParticle6, gridCellPosition);
        bool insideParticle7ShouldBeInsideTheCell = VolumeFractionCalculator.IsParticleInsideCell(insideParticle7, gridCellPosition);

        bool outsideParticle1ShouldBeOutsideTheCell = VolumeFractionCalculator.IsParticleInsideCell(outsideParticle1, gridCellPosition);
        bool outsideParticle2ShouldBeOutsideTheCell = VolumeFractionCalculator.IsParticleInsideCell(outsideParticle2, gridCellPosition);
        bool outsideParticle3ShouldBeOutsideTheCell = VolumeFractionCalculator.IsParticleInsideCell(outsideParticle3, gridCellPosition);
        bool outsideParticle4ShouldBeOutsideTheCell = VolumeFractionCalculator.IsParticleInsideCell(outsideParticle4, gridCellPosition);

        // Assertions
        Assert.IsTrue(insideParticle1ShouldBeInsideTheCell);
        Assert.IsTrue(insideParticle2ShouldBeInsideTheCell);
        Assert.IsTrue(insideParticle3ShouldBeInsideTheCell);
        Assert.IsTrue(insideParticle4ShouldBeInsideTheCell);
        Assert.IsTrue(insideParticle5ShouldBeInsideTheCell);
        Assert.IsTrue(insideParticle6ShouldBeInsideTheCell);
        Assert.IsTrue(insideParticle7ShouldBeInsideTheCell);

        Assert.IsFalse(outsideParticle1ShouldBeOutsideTheCell);
        Assert.IsFalse(outsideParticle2ShouldBeOutsideTheCell);
        Assert.IsFalse(outsideParticle3ShouldBeOutsideTheCell);
        Assert.IsFalse(outsideParticle4ShouldBeOutsideTheCell);
    }

    [Test]
    public void CalculateGridCellForParticleShouldCastParticlePositionsIntoCells()
    {
        int2 expectedCellPosition1 = new(2, 12);
        int2 expectedCellPosition2 = new(3, 12);
        double2 particlePosition1 = new(2.1, 12);
        double2 particlePosition2 = new(2, 12);
        double2 particlePosition3 = new(2.9, 12);
        double2 particlePosition4 = new(3, 12);
        double2 particlePosition5 = new(2, 12.1);
        double2 particlePosition6 = new(2, 12.9);

        Particle insideParticle1 = CreateParticleWithGivenPosition(particlePosition1);
        Particle insideParticle2 = CreateParticleWithGivenPosition(particlePosition2);
        Particle insideParticle3 = CreateParticleWithGivenPosition(particlePosition3);
        Particle insideParticle4 = CreateParticleWithGivenPosition(particlePosition4);
        Particle insideParticle5 = CreateParticleWithGivenPosition(particlePosition5);
        Particle insideParticle6 = CreateParticleWithGivenPosition(particlePosition6);

        int2 computedCellPosition1 = VolumeFractionCalculator.CalculateGridCellForParticle(insideParticle1);
        int2 computedCellPosition2 = VolumeFractionCalculator.CalculateGridCellForParticle(insideParticle2);
        int2 computedCellPosition3 = VolumeFractionCalculator.CalculateGridCellForParticle(insideParticle3);
        int2 computedCellPosition4 = VolumeFractionCalculator.CalculateGridCellForParticle(insideParticle4);
        int2 computedCellPosition5 = VolumeFractionCalculator.CalculateGridCellForParticle(insideParticle5);
        int2 computedCellPosition6 = VolumeFractionCalculator.CalculateGridCellForParticle(insideParticle6);

        Assert.AreEqual(computedCellPosition1, expectedCellPosition1);
        Assert.AreEqual(computedCellPosition2, expectedCellPosition1);
        Assert.AreEqual(computedCellPosition3, expectedCellPosition1);
        Assert.AreEqual(computedCellPosition4, expectedCellPosition2);
        Assert.AreEqual(computedCellPosition5, expectedCellPosition1);
        Assert.AreEqual(computedCellPosition6, expectedCellPosition1);
    }

    [Test]
    public void CalculateNineNeighborhoodOfParticleShouldCastAndCalculateThe3x3Neighborhood()
    {
        double2 testParticlePosition = new(2.5, 12.7);
        Particle particle = CreateParticleWithGivenPosition(testParticlePosition);
        NineNeighborhood nineNeighborhood = VolumeFractionCalculator.CalculateNineNeighborhoodOfParticle(particle);
        int2 expectedUpperLeft = new(1, 11);
        int2 expectedUpper = new(1, 12);
        int2 expectedUpperRight = new(1, 13);
        int2 expectedLeft = new(2, 11);
        int2 expectedCenter = new(2, 12);
        int2 expectedRight = new(2, 13);
        int2 expectedLowerLeft = new(3, 11);
        int2 expectedLower = new(3, 12);
        int2 expectedLowerRight = new(3, 13);
        Assert.AreEqual(nineNeighborhood.GetUpperLeft(), expectedUpperLeft);
        Assert.AreEqual(nineNeighborhood.GetUpper(), expectedUpper);
        Assert.AreEqual(nineNeighborhood.GetUpperRight(), expectedUpperRight);
        Assert.AreEqual(nineNeighborhood.GetLeft(), expectedLeft);
        Assert.AreEqual(nineNeighborhood.GetCenter(), expectedCenter);
        Assert.AreEqual(nineNeighborhood.GetRight(), expectedRight);
        Assert.AreEqual(nineNeighborhood.GetLowerLeft(), expectedLowerLeft);
        Assert.AreEqual(nineNeighborhood.GetLower(), expectedLower);
        Assert.AreEqual(nineNeighborhood.GetLowerRight(), expectedLowerRight);
    }

    [Test]
    public void FindNeighborsOfParticleShouldFindNeighboringParticlesGivenAParticle()
    {
        /*
         * Calculate the 9-neighborhood and find all particles within each of the cells
         * in said neighborhood. 
         */
        double2 testParticlePosition = new(2.5, 12.7);
        Particle particle = CreateParticleWithGivenPosition(testParticlePosition);
        /*
         * 1, 11
         * 1, 12
         * 1, 13
         * 2, 11
         * 2, 12
         * 2, 13
         * 3, 11
         * 3, 12
         * 3, 13
         * 
         * Neighboring particle positions:
         */
        double2 neighbor1Position = new(1.3, 11.1);
        double2 neighbor2Position = new(1.9, 12.7);
        double2 neighbor3Position = new(1, 13);
        double2 neighbor4Position = new(2.5, 11.5);
        double2 neighbor5Position = new(2.3, 12.9);
        double2 neighbor6Position = new(2.01, 13.11);
        double2 neighbor7Position = new(3.1, 11.2);
        double2 neighbor8Position = new(3.14, 12.8);
        double2 neighbor9Position = new(3, 13.01);
        bool hasNeighbor1 = false;
        bool hasNeighbor2 = false;
        bool hasNeighbor3 = false;
        bool hasNeighbor4 = false;
        bool hasNeighbor5 = false;
        bool hasNeighbor6 = false;
        bool hasNeighbor7 = false;
        bool hasNeighbor8 = false;
        bool hasNeighbor9 = false;

        bool allNeighborsContained = false;

        // Non-neighbors:
        double2 nonNeighbor1Position = new(3, 14);
        double2 nonNeighbor2Position = new(1, 10.45);
        bool hasNonNeighbor1 = false;
        bool hasNonNeighbor2 = false;
        bool nonNeighborsContained = false;
        Particle neighborParticle1 = CreateParticleWithGivenPosition(neighbor1Position);
        Particle neighborParticle2 = CreateParticleWithGivenPosition(neighbor2Position);
        Particle neighborParticle3 = CreateParticleWithGivenPosition(neighbor3Position);
        Particle neighborParticle4 = CreateParticleWithGivenPosition(neighbor4Position);
        Particle neighborParticle5 = CreateParticleWithGivenPosition(neighbor5Position);
        Particle neighborParticle6 = CreateParticleWithGivenPosition(neighbor6Position);
        Particle neighborParticle7 = CreateParticleWithGivenPosition(neighbor7Position);
        Particle neighborParticle8 = CreateParticleWithGivenPosition(neighbor8Position);
        Particle neighborParticle9 = CreateParticleWithGivenPosition(neighbor9Position);
        Particle nonNeighborParticle1 = CreateParticleWithGivenPosition(neighbor1Position);
        Particle nonNeighborParticle2 = CreateParticleWithGivenPosition(neighbor2Position);
        List<Particle> allParticles = new List<Particle> { neighborParticle1, neighborParticle2,
            neighborParticle3, neighborParticle4, neighborParticle5, neighborParticle6, neighborParticle7,
            neighborParticle8, neighborParticle9, nonNeighborParticle1, nonNeighborParticle2};


        List<Particle> allNeighbors = VolumeFractionCalculator.FindNeighborsOfParticle(particle, allParticles);

        foreach (Particle neighbor in allNeighbors)
        {
            double2 position = neighbor.GetPosition();
            if (GeneralMathUtils.DeepEquals(position, neighbor1Position))
            {
                hasNeighbor1 = true;
            }
            else if (GeneralMathUtils.DeepEquals(position, neighbor2Position))
            {
                hasNeighbor2 = true;
            }
            else if (GeneralMathUtils.DeepEquals(position, neighbor3Position))
            {
                hasNeighbor3 = true;
            }
            else if (GeneralMathUtils.DeepEquals(position, neighbor4Position))
            {
                hasNeighbor4 = true;
            }
            else if (GeneralMathUtils.DeepEquals(position, neighbor5Position))
            {
                hasNeighbor5 = true;
            }
            else if (GeneralMathUtils.DeepEquals(position, neighbor6Position))
            {
                hasNeighbor6 = true;
            }
            else if (GeneralMathUtils.DeepEquals(position, neighbor7Position))
            {
                hasNeighbor7 = true;
            }
            else if (GeneralMathUtils.DeepEquals(position, neighbor8Position))
            {
                hasNeighbor8 = true;
            }
            else if (GeneralMathUtils.DeepEquals(position, neighbor9Position))
            {
                hasNeighbor9 = true;
            }
            else if (GeneralMathUtils.DeepEquals(position, nonNeighbor1Position))
            {
                hasNonNeighbor1 = true;
            }
            else if (GeneralMathUtils.DeepEquals(position, nonNeighbor2Position))
            {
                hasNonNeighbor2 = true;
            }
        }

        allNeighborsContained = hasNeighbor1 && hasNeighbor2 && hasNeighbor3 &&
            hasNeighbor4 && hasNeighbor5 && hasNeighbor6 && hasNeighbor7 &&
            hasNeighbor8 && hasNeighbor9;

        nonNeighborsContained = hasNonNeighbor1 || hasNonNeighbor2;

        Assert.IsTrue(allNeighborsContained);
        Assert.IsFalse(nonNeighborsContained);
    }
}