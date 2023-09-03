using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class VolumeFractionCalculatorTests
{
    private Particle CreateParticleInUnity()
    {
        return ScriptableObject.CreateInstance("Particle") as Particle;
    }

    private AirParticle CreateAirParticleInUnity()
    {
        return ScriptableObject.CreateInstance("AirParticle") as AirParticle;
    }

    private FluidParticle CreateFluidParticleInUnity()
    {
        return ScriptableObject.CreateInstance("FluidParticle") as FluidParticle;
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

    private Particle CreateAirParticleWithGivenPosition(double2 particlePosition)
    {
        double2 initialVelocity = new(0, 0);
        double initialMass = 0.5;
        double2x2 initialC = new double2x2(0, 0, 0, 0);
        AirParticle particle = CreateAirParticleInUnity();
        particle.InitParticle(particlePosition, initialVelocity, initialMass, initialC);
        return particle;
    }

    private Particle CreateFluidParticleWithGivenPosition(double2 particlePosition)
    {
        double2 initialVelocity = new(0, 0);
        double initialMass = 1;
        double2x2 initialC = new double2x2(0, 0, 0, 0);
        FluidParticle particle = CreateFluidParticleInUnity();
        particle.InitParticle(particlePosition, initialVelocity, initialMass, initialC);
        return particle;
    }

    private Particle[,] CreateParticleListFor9x9GridTest()
    {
        // Cell 1 at (1, 11)
        Particle p1 = CreateAirParticleWithGivenPosition(new double2(1, 11.1));

        // Cell 2 at (1, 12)
        // NONE: EMPTY CELL

        // Cell 3 at (1, 13)
        Particle p2 = CreateFluidParticleWithGivenPosition(new double2(1.1, 13.2));
        Particle p3 = CreateFluidParticleWithGivenPosition(new double2(1.2, 13.3));
        Particle p4 = CreateAirParticleWithGivenPosition(new double2(1.7, 13.7));

        // Cell 4 at (2, 11)
        Particle p5 = CreateFluidParticleWithGivenPosition(new double2(2, 11.7));
        Particle p6 = CreateAirParticleWithGivenPosition(new double2(2, 11.5));
        Particle p7 = CreateAirParticleWithGivenPosition(new double2(2.6, 11.1));
        Particle p8 = CreateAirParticleWithGivenPosition(new double2(2.3, 11));

        // Cell 5 at (2, 12). The exact particle we are looking at is also at (2, 12), in the center of our 9x9 neighborhood. 
        Particle p9 = CreateFluidParticleWithGivenPosition(new double2(2.7, 12.9));
        Particle p10 = CreateAirParticleWithGivenPosition(new double2(2.1, 12));
        Particle p11 = CreateFluidParticleWithGivenPosition(new double2(2, 12.1));
        Particle p12 = CreateAirParticleWithGivenPosition(new double2(2, 12.9));
        Particle p13_andParticleInQuestion = CreateFluidParticleWithGivenPosition(new double2(2, 12));
        Particle p14 = CreateFluidParticleWithGivenPosition(new double2(2.9, 12));

        // Cell 6 at (2, 13)
        Particle p15 = CreateAirParticleWithGivenPosition(new double2(2.6, 13.8));
        Particle p16 = CreateFluidParticleWithGivenPosition(new double2(2, 13.1));

        // Cell 7 at (3, 11)
        Particle p17 = CreateAirParticleWithGivenPosition(new double2(3.5, 11.7));

        // Cell 8 at (3, 12)
        Particle p18 = CreateAirParticleWithGivenPosition(new double2(3, 12));

        // Cell 9 at (3, 13)
        Particle p19 = CreateFluidParticleWithGivenPosition(new double2(3.6, 13.1));

        // List creation
        Particle[,] allParticles = new Particle[,] 
            { { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13_andParticleInQuestion, p14, p15, p16, p17, p18, p19 } };

        return allParticles;
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

        bool allNeighborsContained;

        // Non-neighbors:
        double2 nonNeighbor1Position = new(3, 14);
        double2 nonNeighbor2Position = new(1, 10.45);
        bool hasNonNeighbor1 = false;
        bool hasNonNeighbor2 = false;
        bool nonNeighborsContained;
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

    [Test]
    public void ComputeWeightAtParticleShouldComputeAWeightBasedOnTheDistanceBetweenTwoParticles()
    {
        // This is effectively our smoothing kernel. Reciprocal of distance.
        double2 point1 = new(2, 12);
        double2 point2 = new(1, 11);
        double expectedWeight = 1 / math.sqrt(2);
        Particle particle1 = CreateParticleWithGivenPosition(point1);
        Particle particle2 = CreateParticleWithGivenPosition(point2);
        double weight = VolumeFractionCalculator.ComputeWeightAtParticle(particle1, particle2);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual((float)weight, (float)expectedWeight);
    }

    // For gas volume, we are doing a simple approach where it's calculated based on the cell
    // So first, find the cell the particle is in, and then compute the gas volume of that cell.
    // No it's not crazy accurate but it is a good enough start.

    [Test]
    public void ComputeNumberOfParticlesInCellShouldCountTheNumberOfParticlesWithinACell()
    {
        int2 gridCellPosition = new(2, 12);
        double2 particlePosition1 = new(2.1, 12);
        double2 particlePosition2 = new(2, 12);
        double2 particlePosition3 = new(2.9, 12);
        double2 particlePosition4 = new(3, 12);
        double2 particlePosition5 = new(2, 12.1);
        double2 particlePosition6 = new(2, 12.9);

        int expectedNumberOfParticlesWithinCell = 5;

        Particle p1 = CreateParticleWithGivenPosition(particlePosition1);
        Particle p2 = CreateParticleWithGivenPosition(particlePosition2);
        Particle p3 = CreateParticleWithGivenPosition(particlePosition3);
        Particle p4 = CreateParticleWithGivenPosition(particlePosition4);
        Particle p5 = CreateParticleWithGivenPosition(particlePosition5);
        Particle p6 = CreateParticleWithGivenPosition(particlePosition6);
        Particle[,] particles = new Particle[,] { { p1, p2, p3, p4, p5, p6 } };
        int actualNumberOfParticlesWithinCell = VolumeFractionCalculator.ComputeNumberOfParticlesInCell(particles, gridCellPosition);
        Assert.AreEqual(expectedNumberOfParticlesWithinCell, actualNumberOfParticlesWithinCell);
    }

    [Test]
    public void ComputeNumberOfGasParticlesInCellShouldCountTheNumberOfAirParticlesWithinACell()
    {
        int2 gridCellPosition = new(2, 12);
        double2 particlePosition1 = new(2.1, 12);
        double2 particlePosition2 = new(2, 12);
        double2 particlePosition3 = new(2.9, 12);
        double2 particlePosition4 = new(3, 12);
        double2 particlePosition5 = new(2, 12.1);
        double2 particlePosition6 = new(2, 12.9);
        // p4 isn't in there, so expect 2
        Particle p1 = CreateAirParticleWithGivenPosition(particlePosition1);
        Particle p2 = CreateParticleWithGivenPosition(particlePosition2);
        Particle p3 = CreateParticleWithGivenPosition(particlePosition3);
        Particle p4 = CreateAirParticleWithGivenPosition(particlePosition4);
        Particle p5 = CreateParticleWithGivenPosition(particlePosition5);
        Particle p6 = CreateAirParticleWithGivenPosition(particlePosition6);
        int expectedNumberOfGasParticlesInCell = 2;
        Particle[,] particles = new Particle[,] { { p1, p2, p3, p4, p5, p6 } };
        int actualNumberOfGasParticlesInCell = VolumeFractionCalculator.ComputeNumberOfAirParticlesInCell(particles, gridCellPosition);
        Assert.AreEqual(expectedNumberOfGasParticlesInCell, actualNumberOfGasParticlesInCell);
    }

    [Test]
    public void ComputeGasVolumeOfParticleShouldDivideTheNumberOfGasParticlesInTheCorrespondingCellByTheTotalNumberOfParticlesInTheCorrespondingCell()
    {
        double expectedGasVolume = 2.0 / 6.0;
        double2 mySampleParticlePosition = new(2.9, 12.3);
        Particle mySampleParticle = CreateParticleWithGivenPosition(mySampleParticlePosition);
        double2 particlePosition1 = new(2.1, 12);
        double2 particlePosition2 = new(2, 12);
        double2 particlePosition3 = new(2.9, 12);
        double2 particlePosition4 = new(3, 12);
        double2 particlePosition5 = new(2, 12.1);
        double2 particlePosition6 = new(2, 12.9);
        Particle p1 = CreateAirParticleWithGivenPosition(particlePosition1);
        Particle p2 = CreateParticleWithGivenPosition(particlePosition2);
        Particle p3 = CreateParticleWithGivenPosition(particlePosition3);
        Particle p4 = CreateAirParticleWithGivenPosition(particlePosition4);
        Particle p5 = CreateParticleWithGivenPosition(particlePosition5);
        Particle p6 = CreateAirParticleWithGivenPosition(particlePosition6);
        Particle[,] particles = new Particle[,] { { p1, p2, p3, p4, p5, p6, mySampleParticle } };
        double actualGasVolume = VolumeFractionCalculator.ComputeGasVolumeOfParticle(particles, mySampleParticle);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual((float)expectedGasVolume, (float)actualGasVolume);
    }

    [Test]

    public void ComputeVolumeFractionContributionForParticleShouldMultiplyWeightByGasVolume()
    {
        double2 mySampleParticlePosition = new(2.9, 12.3);
        Particle mySampleParticle = CreateParticleWithGivenPosition(mySampleParticlePosition);
        double2 particlePosition1 = new(2.1, 12);
        double2 particlePosition2 = new(2, 12);
        double2 particlePosition3 = new(2.9, 12);
        double2 particlePosition4 = new(3, 12);
        double2 particlePosition5 = new(2, 12.1);
        double2 particlePosition6 = new(2, 12.9);
        Particle p1 = CreateAirParticleWithGivenPosition(particlePosition1);
        Particle p2 = CreateParticleWithGivenPosition(particlePosition2);
        Particle p3 = CreateParticleWithGivenPosition(particlePosition3);
        Particle p4 = CreateAirParticleWithGivenPosition(particlePosition4);
        Particle p5 = CreateParticleWithGivenPosition(particlePosition5);
        Particle p6 = CreateAirParticleWithGivenPosition(particlePosition6);
        Particle[,] particles = new Particle[,] { { p1, p2, p3, p4, p5, p6, mySampleParticle } };
        // distance between mySampleParticle and p5 is 0.921954
        // So let's use p5. particle i, particle j.
        // Expected: (0.3333) * (1 / 0.921954)
        double expectedVolumeFractionContribution = (1.0 / 3.0) * (1.0 / math.distance(mySampleParticlePosition, particlePosition5));
        double actualVolumeFractionContribution = VolumeFractionCalculator.ComputeVolumeFractionContributionForParticle(mySampleParticle, p5, particles);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual((float)expectedVolumeFractionContribution, (float)actualVolumeFractionContribution);
        // sanity check
        UnityEngine.Assertions.Assert.AreNotApproximatelyEqual((float)0.7, (float)0.8);
    }

    [Test]
    public void CalculateGasVolumeShouldDivideTheNumberOfGasParticlesInTheCellByTheTotalNumberOfParticlesInTheCell()
    {
        Particle[,] particles = CreateParticleListFor9x9GridTest();

        // NOTE: It is not immediately intuitive that we are calculating cell values but the GridCell class itself does not store position. This may need to be amended...
        int2 cell1Position = new(1, 11);
        double expectedGasVolume1 = 1;
        double actualGasVolume1 = VolumeFractionCalculator.CalculateGasVolume(particles, cell1Position);
        Assert.AreEqual(expectedGasVolume1, actualGasVolume1);

        int2 cell2Position = new(1, 12);
        double expectedGasVolume2 = 0;
        double actualGasVolume2 = VolumeFractionCalculator.CalculateGasVolume(particles, cell2Position);
        Assert.AreEqual(expectedGasVolume2, actualGasVolume2);

        int2 cell3Position = new(1, 13);
        double expectedGasVolume3 = 0.33;
        double actualGasVolume3 = VolumeFractionCalculator.CalculateGasVolume(particles, cell3Position);
        Assert.IsTrue(GeneralMathUtils.ApproximatelyEquals(expectedGasVolume3, actualGasVolume3));

        int2 cell4Position = new(2, 11);
        double expectedGasVolume4 = 0.75;
        double actualGasVolume4 = VolumeFractionCalculator.CalculateGasVolume(particles, cell4Position);
        Assert.AreEqual(expectedGasVolume4, actualGasVolume4);

        int2 cell5Position = new(2, 12);
        double expectedGasVolume5 = 0.33;
        double actualGasVolume5 = VolumeFractionCalculator.CalculateGasVolume(particles, cell5Position);
        Assert.IsTrue(GeneralMathUtils.ApproximatelyEquals(expectedGasVolume5, actualGasVolume5));

        int2 cell6Position = new(2, 13);
        double expectedGasVolume6 = 0.5;
        double actualGasVolume6 = VolumeFractionCalculator.CalculateGasVolume(particles, cell6Position);
        Assert.AreEqual(expectedGasVolume6, actualGasVolume6);

        int2 cell7Position = new(3, 11);
        double expectedGasVolume7 = 1;
        double actualGasVolume7 = VolumeFractionCalculator.CalculateGasVolume(particles, cell7Position);
        Assert.AreEqual(expectedGasVolume7, actualGasVolume7);

        int2 cell8Position = new(3, 12);
        double expectedGasVolume8 = 1;
        double actualGasVolume8 = VolumeFractionCalculator.CalculateGasVolume(particles, cell8Position);
        Assert.AreEqual(expectedGasVolume8, actualGasVolume8);

        int2 cell9Position = new(3, 13);
        double expectedGasVolume9 = 0;
        double actualGasVolume9 = VolumeFractionCalculator.CalculateGasVolume(particles, cell9Position);
        Assert.AreEqual(expectedGasVolume9, actualGasVolume9);

    }

    [Test]
    public void CalculateVolumeFractionForParticleShouldComputeContributionForAll9NeighborsAndSumThemUp()
    {
        // Done by hand in my notebook and transcribed here. 
        Particle[,] particles = CreateParticleListFor9x9GridTest();
        // TODO convert particles into a List<Particle> or even change its initial definition, and pass it into the function instead of new List<Particle>()
        Particle p13 = particles[0, 12]; // may not need this exact thing
        double volumeFraction = VolumeFractionCalculator.CalculateVolumeFractionForParticleAtPosition(new int2(2, 12), new List<Particle>(), p13);
    }
}
