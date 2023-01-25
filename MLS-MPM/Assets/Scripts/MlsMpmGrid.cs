using UnityEngine;
using Unity.Mathematics;

public class MlsMpmGrid : ScriptableObject
{
    private int gridResolution;
    private GridCell[,] grid;
    public void InitMlsMpmGrid(int gridResolution)
    {
        grid = new GridCell[gridResolution, gridResolution];
        this.gridResolution = gridResolution;
        for (int i = 0; i < gridResolution; i++)
        {
            for (int j = 0; j < gridResolution; j++)
            {
                GridCell cell = CreateInstance("GridCell") as GridCell;
                double2 initialVelocity = new(0, 0);
                double initialMass = 0;
                cell.InitGridCell(initialVelocity, initialMass);
                grid[i, j] = cell;
            }
        }
    }

    public int GetGridResolution()
    {
        return gridResolution;
    }
    
    public int2 GetSize()
    {
        return new(grid.GetLength(0), grid.GetLength(1));
    }

    public GridCell At(int x, int y)
    {
        if (grid == null)
        {
            int defaultResolution = 64;
            InitMlsMpmGrid(defaultResolution);
        }
        return grid[x, y];
    }

    public GridCell At(int[] position)
    {
        return At(position[0], position[1]);
    }

    public void UpdateCellAt(int x, int y, GridCell updated)
    {
        if (grid == null)
        {
            int defaultResolution = 64;
            InitMlsMpmGrid(defaultResolution);
        }
        grid[x, y] = updated;
    }
}
