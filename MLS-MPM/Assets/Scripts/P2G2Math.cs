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
        if (density == 0)
        {
            Debug.LogError("Density is 0 in P2G2Math.ComputeVolume()! Displaying divide by zero error...");
        }
        return particleMass / density;
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

    public static double ComputeTrace(double2x2 strain)
    {
        double[,] formatted = GeneralMathUtils.Format2x2MatrixForMath(strain);
        return ComputeTrace(formatted);
    }

    public static double ComputeTrace(double[,] strain)
    {
        return strain[1, 0] + strain[0, 1];
    }

    public static double2x2 UpdateStrainAndReturnUnityMatrix(double2x2 initialStrain, double trace)
    {
        return GeneralMathUtils.Format2x2MatrixForMath(UpdateStrain(initialStrain, trace));
    }

    public static double[,] UpdateStrain(double2x2 initialStrain, double trace)
    {
        double[,] formatted = GeneralMathUtils.Format2x2MatrixForMath(initialStrain);
        return UpdateStrain(formatted, trace);
    }

    public static double[,] UpdateStrain(double[,] initialStrain, double trace)
    {
        initialStrain[0, 1] = trace;
        initialStrain[1, 0] = trace;
        return initialStrain;
    }

    public static double2x2 ComputeViscosity(double2x2 strain, double dynamicViscosity)
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

    public static double[] ComputeDistanceFromCellToNeighbor(int[] neighborCellPosition, int[] currentCellPosition)
    {
        double x = (neighborCellPosition[0] - currentCellPosition[0]) + 0.5;
        double y = (neighborCellPosition[1] - currentCellPosition[1]) + 0.5;
        double[] distanceFromCellToNeighbor = { x, y };
        return distanceFromCellToNeighbor;
    }

    public static double2 ComputeMomentum(double2x2 equation16Term0, double weight, double[] distanceFromCellToNeighbor)
    {
        double2 formatted = GeneralMathUtils.Format2DVectorForMath(distanceFromCellToNeighbor);
        return ComputeMomentum(equation16Term0, weight, formatted);
    }

    public static double2 ComputeMomentum(double2x2 equation16Term0, double weight, double2 distanceFromParticleToNeighbor)
    {
        double2x2 firstFactorMatrix = equation16Term0 * weight;
        double2 secondFactorVector = distanceFromParticleToNeighbor;
        return math.mul(firstFactorMatrix, secondFactorVector);
    }

    public static double2 UpdateCellVelocity(double2 momentum, double2 initialCellVelocity)
    {
        return momentum + initialCellVelocity;
    }

    public static double2x2 ComputeHerschelBulkleyStress(double yieldStress_T0, double2x2 strain_deltaVPlusDeltaVTransposed, 
        double viscosity_mu, double flowIndex_n, double eosStiffness, double density, double restDensity, int eosPower)
    {
        double pressure = ComputePressure(eosStiffness, density, restDensity, eosPower);
        double2x2 pressureTimesTranspose = CreateStressMatrix(pressure);
        // viscositySecondHalf often gives us NaN -- we don't want to raise negative base to a fractional power, so the math is wrong, debug this
        double2x2 viscositySecondHalf = viscosity_mu * (new double2x2(math.pow(strain_deltaVPlusDeltaVTransposed[0], flowIndex_n), math.pow(strain_deltaVPlusDeltaVTransposed[1], flowIndex_n)));
        //Debug.Log("First piece: " + math.pow(strain_deltaVPlusDeltaVTransposed[0], flowIndex_n) + " and here was the first part: " + strain_deltaVPlusDeltaVTransposed[0]);
        //Debug.Log("Second piece: " + math.pow(strain_deltaVPlusDeltaVTransposed[1], flowIndex_n) + " and here was the first part: " + strain_deltaVPlusDeltaVTransposed[1]);
        //Debug.Log("Whole double2x2 " + (new double2x2(math.pow(strain_deltaVPlusDeltaVTransposed[0], flowIndex_n), math.pow(strain_deltaVPlusDeltaVTransposed[1], flowIndex_n))));
        double2x2 viscosity = yieldStress_T0 + viscositySecondHalf;
        double2x2 toReturn = pressureTimesTranspose + viscosity;
        return toReturn;
    }

}
