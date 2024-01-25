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
    
    public static double UpdateMass(double initialMass, double contribution)
    {
        return initialMass + contribution;
    }

    public static double3 UpdateVelocity(double massContribution, double3 particleVelocity, double3 Q, double3 oldCellVelocity) 
    {
        double3 newCellVelocity = massContribution * (particleVelocity + Q);
        return oldCellVelocity + newCellVelocity;
    }

    /**
     * P2G2
     */
    public static double UpdateDensity(double weight, double mass, double density)
    {
        return density + (mass * weight);
    }

    public static double ComputeVolume(double mass, double density)
    {
        if (density == 0)
        {
            density = 0.001;
        }
        return mass / density;
    }

    public static double ComputeTrace(double3x3 strain)
    {
        return strain.c0.x + strain.c1.y + strain.c2.z;
    }

    public static double ComputePressure(double eosStiffness, double density, double restDensity, double eosPower)
    {
        // Note: eosStiffness is applied to the term after it's raised to the power and 1 is subtracted from it.
        return Math.Max(-0.1, eosStiffness * (Math.Pow((density / restDensity), eosPower) - 1));
    }

    public static double3x3 CreateStressMatrix(double pressure)
    {
        double3x3 stressMatrix = new(
            new double3(-pressure, 0, 0),
            new double3(0, -pressure, 0),
            new double3(0, 0, -pressure)
        );

        return stressMatrix;
    }

    public static double3x3 ComputeHerschelBulkleyStress(double yieldStress_T0, double3x3 strain_deltaVPlusDeltaVTransposed,
        double viscosity_mu, double flowIndex_n, double eosStiffness, double density, double restDensity, int eosPower, double offset = 0)
    {
        double pressure = ComputePressure(eosStiffness, density, restDensity, eosPower);
        double3x3 pressureTimesTranspose = CreateStressMatrix(pressure);
        double3x3 strainRaisedToPower = new();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                strainRaisedToPower[i][j] = math.pow(strain_deltaVPlusDeltaVTransposed[i][j], flowIndex_n);
            }
        }
        strainRaisedToPower -= offset;
        double3x3 viscosity = viscosity_mu * strainRaisedToPower;
        double3x3 shearStress = yieldStress_T0 + viscosity;
        double3x3 toReturn = pressureTimesTranspose + shearStress;
        return toReturn;
    }

    public static double3x3 ComputeEquation16Term0(double3x3 stress, double volume, double dt)
    {
        double3x3 term0 = -volume * 4 * stress * dt;
        return term0;
    }

    public static double3 ComputeMomentum(double3x3 equation16Term0, double weight, double3 distanceFromParticleToNeighbor)
    {
        double3x3 firstFactorMatrix = equation16Term0 * weight;
        double3 secondFactorVector = distanceFromParticleToNeighbor;
        return math.mul(firstFactorMatrix, secondFactorVector);
    }

    public static double3 AddMomentumToVelocity(double3 momentum, double3 initialVelocity)
    {
        return momentum + initialVelocity;
    }

    /**
     * G2P
     */
    public static double3 ComputeWeightedVelocity(double3 velocity, double weight)
    {
        return velocity * weight;
    }

    public static double3x3 ComputeTerm(double3 velocity, double3 distance)
    {
        return new double3x3(
            velocity.x * distance.x, velocity.y * distance.x, velocity.z * distance.x,
            velocity.x * distance.y, velocity.y * distance.y, velocity.z * distance.y,
            velocity.x * distance.z, velocity.y * distance.z, velocity.z * distance.z
        );
    }

    public static double3x3 UpdateB(double3x3 B, double3x3 term)
    {
        return B + term;
    }

    public static double3 AddWeightedVelocity(double3 velocity, double3 weightedVelocity)
    {
        return velocity + weightedVelocity;
    }

    public static double3x3 RecomputeCMatrix(double3x3 B)
    {
        return B * 4;
    }

    public static double3 AdvectParticle(double3 position, double3 velocity, double dt)
    {
        return position + (velocity * dt);
    }

    public static bool IsAir(Particle_3D p)
    {
        double airMass = 0.5;
        return p.GetMass() == airMass;
    }
}
