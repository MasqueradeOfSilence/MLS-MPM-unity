using Unity.Mathematics;
using UnityEngine;

/**
 * Grid: Represents the 3D MLS-MPM grid.
 *  The grid is a scratchpad for efficient calculation,
 *  and is reset at every iteration of MLS-MPM. 
 */
public class Grid_3D : ScriptableObject
{
    /**
     * Data members
     */
    private int resolution;
    private Cell_3D[][][] grid;
    private const int defaultResolution = 64;
    private bool gridInstantiated = false;

    /**
     * Constructor
     */
    public void Init(int resolution = defaultResolution)
    {
        if (ResolutionTooSmall(resolution))
        {
            Debug.LogWarning("Resolution too small! (<=0) Using defaults.");
            resolution = defaultResolution;
        }
        grid = new Cell_3D[resolution][][];
        this.resolution = resolution;
        for (int i = 0; i < resolution; i++) 
        {
            for (int j = 0; j < resolution; j++)
            {
                for (int k = 0; k < resolution; k++)
                {
                    Cell_3D cell = CreateInstance("Cell_3D") as Cell_3D;
                    double3 initialVelocity = new(0, 0, 0);
                    double initialMass = 0;
                    cell.Init(initialVelocity, initialMass);
                    grid[i][j][k] = cell;
                }
            }
        }
        gridInstantiated = true;
    }

    /**
     * Utilities
     */

    private bool ResolutionTooSmall(int resolution)
    {
        return resolution <= 0;
    }

    /**
     * Getters and setters
     * 
     * Resolution ------------------
     */

    public int GetResolution()
    {
        return resolution;
    }

    // Size ------------------

    public int3 GetSize()
    {
        return new(grid.Length, grid[0].Length, grid[0][0].Length);
    }

    // Indexing ------------------
    public Cell_3D At(int3 position)
    {
        if (!gridInstantiated)
        {
            Init();
        }
        return grid[position.x][position.y][position.z];
    }

    public void UpdateCellAt(int3 position, Cell_3D updated)
    {
        if (!gridInstantiated)
        {
            Init();
        }
        grid[position.x][position.y][position.z] = updated;
    }
}
