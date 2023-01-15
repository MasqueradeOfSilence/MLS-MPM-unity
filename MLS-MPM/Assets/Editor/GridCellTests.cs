using NUnit.Framework;
using UnityEngine;
using Unity.Mathematics;

[TestFixture]
public class GridCellTests 
{
    [Test]
    public void GridCellClassShouldExist()
    {
        double2 velocity = 0;
        double mass = 1;
        GridCell gridCell = ScriptableObject.CreateInstance("GridCell") as GridCell;
        gridCell.InitGridCell(velocity, mass);
        Assert.IsNotNull(gridCell);
    }
}
