using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

public class VolumeFractionCalculatorTests
{
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

        // TODO: Create particles here with the aforementioned position. Doesn't matter if Air or Fluid; do a mixture of both. 

        // GridCell creation
        GridCell cell = ScriptableObject.CreateInstance("GridCell") as GridCell;
        double2 initialVelocity = new(0, 0);
        double initialMass = 0;
        cell.InitGridCell(initialVelocity, initialMass);
        // TODO: Return a list of particles from that method, see if particles with that position exist
    }
}
