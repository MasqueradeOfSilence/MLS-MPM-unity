using UnityEngine;
using Unity.Mathematics;

public class G2PMath : MonoBehaviour
{
    public static double[] ComputeWeightedVelocity(double[] neighborCellVelocity, double weight)
    {
        double x = neighborCellVelocity[0] * weight;
        double y = neighborCellVelocity[1] * weight;
        double[] weightedVelocity = { x, y };
        return weightedVelocity;
    }

    public static double[,] ComputeTerm(double[] weightedVelocity, double[] distanceFromCellToNeighbor)
    {
        double[,] term = { { weightedVelocity[0] * distanceFromCellToNeighbor[0],
                    weightedVelocity[1] * distanceFromCellToNeighbor[0] },
                    { weightedVelocity[0] * distanceFromCellToNeighbor[1],
                    weightedVelocity[1] * distanceFromCellToNeighbor[1] } };
        return term;
    }

    public static double2x2 ComputeUpdatedB(double2x2 initialB, double[,] term)
    {
        double2x2 formatted = GeneralMathUtils.Format2x2MatrixForMath(term);
        return ComputeUpdatedB(initialB, formatted);
    }

    public static double2x2 ComputeUpdatedB(double2x2 initialB, double2x2 term)
    {
        double2x2 factor1 = initialB;
        double2x2 factor2 = term;
        return factor1 + factor2;
    }

    public static double[] ComputeUpdatedParticleVelocity(double2 initialParticleVelocity, double[] weightedVelocity)
    {
        double[] formatted = GeneralMathUtils.Format2DVectorForMath(initialParticleVelocity);
        return ComputeUpdatedParticleVelocity(formatted, weightedVelocity);
    }

    public static double[] ComputeUpdatedParticleVelocity(double[] initialParticleVelocity, double[] weightedVelocity)
    {
        double x = initialParticleVelocity[0] + weightedVelocity[0];
        double y = initialParticleVelocity[1] + weightedVelocity[1];
        double[] updatedParticleVelocity = { x, y };
        return updatedParticleVelocity;
    }

    public static double2x2 RecomputeCMatrix(double2x2 B)
    {
        return B * 4;
    }

    public static double[] AdvectParticle(double2 initialParticlePosition, double2 particleVelocity, double dt)
    {
        double[] formatted1 = GeneralMathUtils.Format2DVectorForMath(initialParticlePosition);
        double[] formatted2 = GeneralMathUtils.Format2DVectorForMath(particleVelocity);
        return AdvectParticle(formatted1, formatted2, dt);
    }

    public static double[] AdvectParticle(double[] initialParticlePosition, double[] particleVelocity, double dt)
    {
        double x = initialParticlePosition[0] + (particleVelocity[0] * dt);
        double y = initialParticlePosition[1] + (particleVelocity[1] * dt);
        double[] updatedParticlePosition = { x, y };
        return updatedParticlePosition;
    }
}
