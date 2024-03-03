using System.Collections.Generic;
using Unity.Mathematics;

public class VolumeFractionUtils_3D
{
    public static int ComputeNumberOfParticlesInCell(List<Particle_3D> particles, int3 cellPosition)
    {
        int count = 0;
        foreach (Particle_3D p in particles)
        {
            int3 castedPosition = new(p.GetPosition());
            if (cellPosition.Equals(castedPosition))
            {
                count++;
            }
        }
        return count;
    }
    public static int ComputeNumberOfAirParticlesInCell(List<Particle_3D> particles, int3 cellPosition)
    {
        int numAir = 0;
        foreach(Particle_3D p in particles) 
        {
            int3 castedPosition = new(p.GetPosition());
            if (cellPosition.Equals(castedPosition) && MathUtils_3D.IsAir(p)) 
            {
                numAir++;
            }
        }
        return numAir;
    }
    public static double ComputeGasVolumeOfParticle(List<Particle_3D> particles, Particle_3D p)
    {
        int3 correspondingGridCellPosition = new(p);
        double numberOfGasParticlesInCell = ComputeNumberOfAirParticlesInCell(particles, correspondingGridCellPosition);
        double totalNumberOfParticlesInCell = ComputeNumberOfParticlesInCell(particles, correspondingGridCellPosition);
        if (totalNumberOfParticlesInCell == 0)
        {
            // Guarding against divide-by-zero
            // NOTE: A value of 1 means it's pure AIR, assumed. A value of 0 would assume pure WATER. 
            return 1;
        }
        return numberOfGasParticlesInCell / totalNumberOfParticlesInCell;
    }

    public static double ComputeWeightAtParticle(Particle_3D i, Particle_3D j)
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

    public static double ComputeVolumeFractionContributionForParticle(Particle_3D i, Particle_3D j, List<Particle_3D> particles)
    {
        double weight = ComputeWeightAtParticle(i, j);
        double gasVolume = ComputeGasVolumeOfParticle(particles, j);
        return gasVolume * weight;
    }

    private static List<Particle_3D> FindNeighbors(Particle_3D particle, List<Particle_3D> allParticles)
    {
        List<Particle_3D> neighbors = new();
        TwentySevenNeighborhood_3D hood = new(particle);
        foreach(Particle_3D p in allParticles)
        {
            if (hood.ContainsParticle(p))
            {
                neighbors.Add(p);
            }
        }
        return neighbors;
    }

    public static double ComputeVolumeFraction(List<Particle_3D> particles, Particle_3D p)
    {
        List<Particle_3D> neighbors = FindNeighbors(p, particles);
        double volumeFraction = 0;
        foreach (Particle_3D neighbor in neighbors)
        {
            volumeFraction += ComputeVolumeFractionContributionForParticle(p, neighbor, neighbors);
        }
        //volumeFraction /= 10;
        return volumeFraction;
    }
}
