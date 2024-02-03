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
    private int resolution = 16;
    private const int defaultResolution = 16; // or 64
    private bool gridInstantiated = false;
    private const int defaultZResolution = 16;
    private Cell_3D[] gridFlat;

    /**
     * Constructor
     */
    public void Init(int resolution = defaultResolution, int zResolution = defaultZResolution)
    {
        if (ResolutionTooSmall(resolution))
        {
            Debug.LogWarning("Resolution too small! (<=0) Using defaults.");
            resolution = defaultResolution;
        }
        gridFlat = new Cell_3D[resolution * resolution * resolution];
        this.resolution = resolution;

        for (int i = 0; i < gridFlat.Length; i++)
        {
            Cell_3D cell = CreateInstance("Cell_3D") as Cell_3D;
            double3 initialVelocity = new(0, 0, 0);
            double initialMass = 0;
            cell.Init(initialVelocity, initialMass);
            gridFlat[i] = cell;
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

    public int GetSize()
    {
        int size = Mathf.RoundToInt(Mathf.Pow(resolution, 1.0f / 3.0f));
        return size;
    }

    // Indexing ------------------
    public Cell_3D At(int3 position)
    {
        if (!gridInstantiated)
        {
            Init();
        }
        int x = Mathf.Clamp(position.x, 0, resolution - 1);
        int y = Mathf.Clamp(position.y, 0, resolution - 1);
        int z = Mathf.Clamp(position.z, 0, resolution - 1);

        // Calculate 1D index from 3D coordinates
        int index = x + resolution * (y + resolution * z);

        // Access the element in the 1D array
        return gridFlat[index];
    }

    public Cell_3D At(int position)
    {
        return gridFlat[position];
    }

    public void UpdateCellAt(int3 position, Cell_3D updated)
    {
        if (!gridInstantiated)
        {
            Init();
        }
        int x = Mathf.Clamp(position.x, 0, resolution - 1);
        int y = Mathf.Clamp(position.y, 0, resolution - 1);
        int z = Mathf.Clamp(position.z, 0, resolution - 1);

        // Calculate 1D index from 3D coordinates
        int index = x + resolution * (y + resolution * z);

        // Access the element in the 1D array
        gridFlat[index] = updated;
    }
}
