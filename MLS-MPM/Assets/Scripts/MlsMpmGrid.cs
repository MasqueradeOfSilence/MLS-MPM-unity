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
                double initialMass = 1;
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
}
