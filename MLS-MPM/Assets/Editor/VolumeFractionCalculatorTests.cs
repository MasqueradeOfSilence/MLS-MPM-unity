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

        // Assertions
        Assert.IsTrue(insideParticle1ShouldBeInsideTheCell);
    }
}
