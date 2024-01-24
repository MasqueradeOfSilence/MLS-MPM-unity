using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public static double ComputeWeight(List<double3> weights, int nx, int ny, int nz) 
    {
        return weights.ElementAt(nx).x * weights.ElementAt(ny).y * weights.ElementAt(nz).z;
    }

    public static int3 ComputeNeighborPosition(int3 cellPosition, int nx, int ny, int nz)
    {
        int x = cellPosition.x + nx - 1;
        int y = cellPosition.y + ny - 1;
        int z = cellPosition.z + nz - 1;
        return new int3(Math.Abs(x), Math.Abs(y), Math.Abs(z));
    }

    public static double3 ComputeDistanceFromParticleToNeighbor(int3 neighborPosition, double3 particlePosition)
    {
        double x = (neighborPosition.x - particlePosition.x) + 0.5;
        double y = (neighborPosition.y - particlePosition.y) + 0.5;
        double z = (neighborPosition.z - particlePosition.z) + 0.5;
        return new double3(x, y, z);
    }

     /**
     * P2G1
     */
    public static double3 ComputeQ(double3x3 C, double3 distanceFromParticleToNeighbor)
    {
        double3x3 a = C;
        double3 b = distanceFromParticleToNeighbor;
        double3 Q = math.mul(a, b);
        return Q;
    }

    public static double ComputeMassContribution(double weight, double mass)
    {
        return weight * mass;
    }
}
