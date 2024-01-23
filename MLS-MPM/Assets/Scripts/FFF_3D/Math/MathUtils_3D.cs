using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MathUtils_3D
{
    /**
     * Common
     */
    public static int3 ParticlePositionToCellPosition(double3 particlePosition)
    {
        return new int3(particlePosition);
    }

    public static double3 ComputeDistanceFromParticleToCell(double3 particlePosition, int3 cellPosition)
    {
        return new double3(particlePosition - cellPosition - 0.5);
    }

    /**
     * P2G1
     */
    private static double3 ComputeWeight0(double3 dist)
    {
        double x = 0.5 * Math.Pow((0.5 - dist.x), 2);
        double y = 0.5 * Math.Pow((0.5 - dist.y), 2);
        double z = 0.5 * Math.Pow((0.5 - dist.z), 2);
        return new double3(x, y, z);
    }
    private static double3 ComputeWeight1(double3 dist)
    {
        double x = 0.75 - Math.Pow((dist.x), 2);
        double y = 0.75 - Math.Pow((dist.y), 2);
        double z = 0.75 - Math.Pow((dist.z), 2);
        return new double3(x, y, z);
    }
    private static double3 ComputeWeight2(double3 dist)
    {
        double x = 0.5 * Math.Pow(0.5 + dist.x, 2);
        double y = 0.5 * Math.Pow(0.5 + dist.y, 2);
        double z = 0.5 * Math.Pow(0.5 + dist.z, 2);
        return new double3(x, y, z);
    }
    public static List<double3> ComputeAllWeights(double3 distanceFromParticleToCell)
    {
        double3 weight0 = ComputeWeight0(distanceFromParticleToCell);
        double3 weight1 = ComputeWeight1(distanceFromParticleToCell);
        double3 weight2 = ComputeWeight2(distanceFromParticleToCell);
        List<double3> weights = new List<double3>
        {
            weight0, weight1, weight2
        };
        return weights;
    }
}
