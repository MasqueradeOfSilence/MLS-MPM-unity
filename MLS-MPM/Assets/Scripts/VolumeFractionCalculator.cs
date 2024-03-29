using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class VolumeFractionCalculator : MonoBehaviour
{
    public static bool IsParticleInsideCell(Particle particle, int2 gridCellPosition)
    {
        int2 castedParticlePosition = new(particle.GetPosition());
        return GeneralMathUtils.DeepEquals(gridCellPosition, castedParticlePosition);
    }

    public static int2 CalculateGridCellForParticle(Particle particle)
    {
        return new int2(particle.GetPosition());
    }

    public static NineNeighborhood CalculateNineNeighborhoodOfParticle(Particle particle)
    {
        return new NineNeighborhood(particle);
    }

    public static List<Particle> FindNeighborsOfParticle(Particle particle, List<Particle> allParticles)
    {
        /*
         * 1. Find 9-neighborhood of particle. 
         * 2. For each grid cell in the 9-neighborhood,
         *  2a) Iterate through the particles and see if IsParticleInsideNeighbor is true
         *  2b) Add all of those to the neighbors list.
         */
        List<Particle> neighbors = new List<Particle> { };
        NineNeighborhood nineNeighborhood = new(particle);
        int2 upperLeft = nineNeighborhood.GetUpperLeft();
        int2 upper = nineNeighborhood.GetUpper();
        int2 upperRight = nineNeighborhood.GetUpperRight();
        int2 left = nineNeighborhood.GetLeft();
        int2 center = nineNeighborhood.GetCenter();
        int2 right = nineNeighborhood.GetRight();
        int2 lowerLeft = nineNeighborhood.GetLowerLeft();
        int2 lower = nineNeighborhood.GetLower();
        int2 lowerRight = nineNeighborhood.GetLowerRight();
        foreach (Particle p in allParticles)
        {
            if (IsParticleInsideCell(p, upperLeft) || IsParticleInsideCell(p, upper)
                || IsParticleInsideCell(p, upperRight) || IsParticleInsideCell(p, left)
                || IsParticleInsideCell(p, center) || IsParticleInsideCell(p, right)
                || IsParticleInsideCell(p, lowerLeft) || IsParticleInsideCell(p, lower)
                || IsParticleInsideCell(p, lowerRight))
            {
                // I don't think we should have duplicates. 
                neighbors.Add(p);
            }
        }
        return neighbors;
    }

    public static double ComputeWeightAtParticle(Particle i, Particle j)
    {
        double distance = math.distance(i.GetPosition(), j.GetPosition());
        if (distance == 0)
        {
            // Weighting particle very strongly if at same position (likely the same particle)
            // And avoiding a divide-by-zero error
            return 100;
        }
        return 1 / distance;
    }

    public static int ComputeNumberOfParticlesInCell(List<Particle> particles, int2 gridCellPosition)
    {
        int numberOfParticlesInCell = 0;
        foreach (Particle p in particles)
        {
            int2 cellPosition = new(p.GetPosition());
            if (GeneralMathUtils.DeepEquals(cellPosition, gridCellPosition))
            {
                numberOfParticlesInCell++;
            }
        }
        return numberOfParticlesInCell;
    }

    public static int ComputeNumberOfAirParticlesInCell(List<Particle> particles, int2 gridCellPosition)
    {
        int numberOfAirParticlesInCell = 0;
        foreach (Particle p in particles)
        {
            int2 cellPosition = new(p.GetPosition());
            if (GeneralMathUtils.DeepEquals(cellPosition, gridCellPosition)
                && p.GetType().ToString().Contains("Air"))
            {
                numberOfAirParticlesInCell++;
            }
        }
        return numberOfAirParticlesInCell;
    }

    public static double ComputeGasVolumeOfParticle(List<Particle> particles, Particle particle)
    {
        int2 correspondingGridCellPosition = CalculateGridCellForParticle(particle);
        double numberOfGasParticlesInCell = ComputeNumberOfAirParticlesInCell(particles, correspondingGridCellPosition);
        double totalNumberOfParticlesInCell = ComputeNumberOfParticlesInCell(particles, correspondingGridCellPosition);
        if (totalNumberOfParticlesInCell == 0)
        {
            // Guarding against divide-by-zero
            // NOTE: A value of 1 means it's pure AIR, assumed. A value of 0 would assume pure WATER. 
            // I will need to fix the corresponding unit tests that expect that to be 0
            return 1;
        }
        return numberOfGasParticlesInCell / totalNumberOfParticlesInCell;
    }

    // Then, all the neighbor values will get summed up!
    public static double ComputeVolumeFractionContributionForParticle(Particle i, Particle j, List<Particle> particles)
    {
        double weight = ComputeWeightAtParticle(i, j);
        double gasVolume = ComputeGasVolumeOfParticle(particles, j);
        return gasVolume * weight;
    }

    public static double CalculateGasVolume(List<Particle> particles, int2 gridCellPosition)
    {
        int numAirParticlesInCell = ComputeNumberOfAirParticlesInCell(particles, gridCellPosition);
        int numTotalParticlesInCell = ComputeNumberOfParticlesInCell(particles, gridCellPosition);
        if (numTotalParticlesInCell == 0)
        {
            // can't divide by 0
            return 0;
        }
        // C# by itself will round to an integer if you do (int / int), so you have to convert them to doubles before dividing. 
        return (Convert.ToDouble(numAirParticlesInCell) / Convert.ToDouble(numTotalParticlesInCell));
    }

    public static double CalculateVolumeFractionForParticleAtPosition(List<Particle> particles, Particle particle)
    {
        List<Particle> neighborsOfParticle = FindNeighborsOfParticle(particle, particles);
        double volumeFraction = 0;
        foreach (Particle neighbor in neighborsOfParticle)
        {
            // Compute its conbribution and sum them all up. 
            volumeFraction += ComputeVolumeFractionContributionForParticle(particle, neighbor, neighborsOfParticle);
        }
        return volumeFraction;
    }
}
