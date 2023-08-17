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
         *  2a) Iterate through the particles and see if IsParticleInsideCell is true
         *  2b) Add all of those to the neighbors list.
         */
        List<Particle> neighbors = new List<Particle> { };
        NineNeighborhood nineNeighborhood = new NineNeighborhood(particle);
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
}
