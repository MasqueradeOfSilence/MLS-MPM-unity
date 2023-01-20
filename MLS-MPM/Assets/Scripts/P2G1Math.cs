using System;
using UnityEngine;
using Unity.Mathematics;

/**
 * Utils class for the P2G1 stage of MLS-MPM.
 */

public class P2G1Math: MonoBehaviour
{
    public static double[] ComputeWeight0(double[] distanceFromParticleToCell)
    {
        double x = 0.5 * Math.Pow((0.5 - distanceFromParticleToCell[0]), 2);
        double y = 0.5 * Math.Pow((0.5 - distanceFromParticleToCell[1]), 2);
        double[] weight0 = { x, y };
        return weight0;
    }

    public static double[] ComputeWeight1(double[] distanceFromParticleToCell)
    {
        double x = 0.75 - Math.Pow(distanceFromParticleToCell[0], 2);
        double y = 0.75 - Math.Pow(distanceFromParticleToCell[1], 2);
        double[] weight1 = { x, y };
        return weight1;
    }

    public static double[] ComputeWeight2(double[] distanceFromParticleToCell)
    {
        double x = 0.5 * Math.Pow(0.5 + distanceFromParticleToCell[0], 2);
        double y = 0.5 * Math.Pow(0.5 + distanceFromParticleToCell[1], 2);
        double[] weight2 = { x, y };
        return weight2;
    }

    public static double[][] ComputeAllWeights(double[] distanceFromParticleToCell)
    {
        double[] weight0 = ComputeWeight0(distanceFromParticleToCell);
        double[] weight1 = ComputeWeight1(distanceFromParticleToCell);
        double[] weight2 = ComputeWeight2(distanceFromParticleToCell);
        double[][] weights = { weight0, weight1, weight2 };
        return weights;
    }

    public static double[] ComputeDistanceFromCurrentParticleToCurrentNeighbor(int[] currentNeighborPosition, double[] particlePosition)
    {
        double x = (currentNeighborPosition[0] - particlePosition[0]) + 0.5;
        double y = (currentNeighborPosition[1] - particlePosition[1]) + 0.5;
        double[] distanceFromCurrentParticleToCurrentNeighbor = { x, y };
        return distanceFromCurrentParticleToCurrentNeighbor;
    }

    public static double2 ComputeQ(double2x2 C, double2 distanceFromCurrentParticleToCurrentNeighbor)
    {
        double2x2 a = C;
        double2 b = distanceFromCurrentParticleToCurrentNeighbor;
        double2 Q = math.mul(a, b);
        return Q;
    }

    public static double ComputeMassContribution(double weight, double particleMass)
    {
        return weight * particleMass;
    }

    public static double RecomputeCellMassAndReturnIt(double initialCellMass, double massContribution)
    {
        return initialCellMass + massContribution;
    }

    public static double2 RecomputeCellVelocityAndReturnIt(double massContribution, double2 particleVelocity, double2 Q)
    {
        double x = massContribution * (particleVelocity[0] + Q[0]);
        double y = massContribution * (particleVelocity[1] + Q[1]);
        double2 newCellVelocity = new(particleVelocity[0] + x, particleVelocity[1] + y);
        return newCellVelocity;
    }

}
