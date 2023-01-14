using System;
using UnityEngine;
using Unity.Mathematics;

/**
 * P2G2 Math: A collection of static utilties for the P2G2 portion of MLS-MPM.
 */

public class P2G2Math : MonoBehaviour
{
    public static int[] FindNearestGridCellToParticle(double[] particlePosition)
    {
        // Do NOT cast! C# will simply truncate the decimal!
        int x = Convert.ToInt32(particlePosition[0]);
        int y = Convert.ToInt32(particlePosition[1]);
        int[] nearestGridCellToParticle = { x, y };
        return nearestGridCellToParticle;
    }

    public static double ComputeUpdatedDensity(double weight, double gridCellMass, double initialDensity)
    {
        return initialDensity + (gridCellMass * weight);
    }

    public static double ComputeVolume(double particleMass, double density)
    {
        return particleMass * density;
    }

    public static double ComputePressure(double eosStiffness, double density, double restDensity, double eosPower)
    {
        // Note: eosStiffness is applied to the term after it's raised to the power and 1 is subtracted from it.
        return Math.Max(-0.1, eosStiffness * (Math.Pow((density / restDensity), eosPower) - 1));
    }

    public static double2x2 CreateStressMatrix(double pressure)
    {
        double2x2 stressMatrix = new(-pressure, 0, 0, -pressure);
        return stressMatrix;
    }

    public static double2x2 InitializeStrainMatrix(double2x2 C)
    {
        double2x2 strainMatrix = C;
        return strainMatrix;
    }

    public static double ComputeTrace(double[,] strain)
    {
        return strain[1, 0] + strain[0, 1];
    }

    public static double[,] UpdateStrain(double[,] initialStrain, double trace)
    {
        initialStrain[0, 1] = trace;
        initialStrain[1, 0] = trace;
        return initialStrain;
    }

    public static double2x2 ComputeViscosity(double2x2 strain, double2x2 dynamicViscosity)
    {
        double2x2 a = strain;
        a *= dynamicViscosity;
        double2x2 viscosity = a;
        return viscosity;
    }

    public static double2x2 UpdateStress(double2x2 initialStress, double2x2 viscosity)
    {
        double2x2 a = initialStress;
        double2x2 b = viscosity;
        double2x2 updatedStress = a + b;
        return updatedStress;
    }

    public static double2x2 ComputeEquation16Term0(double2x2 stress, double volume, double dt)
    {
        double2x2 term0 = -volume * 4 * stress * dt;
        return term0;
    }
    public static double[] ComputeDistanceFromCellToNeighbor(double[] neighborCellPosition, double[] currentCellPosition)
    {
        double x = (neighborCellPosition[0] - currentCellPosition[0]) + 0.5;
        double y = (neighborCellPosition[1] - currentCellPosition[1]) + 0.5;
        double[] distanceFromCellToNeighbor = { x, y };
        return distanceFromCellToNeighbor;
    }

    public static double2 ComputeMomentum(double2x2 equation16Term0, double weight, double2 distanceFromCellToNeighbor)
    {
        double2x2 firstFactorMatrix = equation16Term0 * weight;
        double2 secondFactorVector = distanceFromCellToNeighbor;
        return math.mul(firstFactorMatrix, secondFactorVector);
    }

    public static double2 UpdateCellVelocity(double2 momentum, double initialCellVelocity)
    {
        return momentum + initialCellVelocity;
    }

}
