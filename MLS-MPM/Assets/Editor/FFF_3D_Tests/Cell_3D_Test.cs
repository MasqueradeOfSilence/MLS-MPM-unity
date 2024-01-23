using NUnit.Framework;
using UnityEngine;
using Unity.Mathematics;

[TestFixture]
public class Cell_3D_Test
{
    [Test]
    public void CellClassShouldExist()
    {
        double3 velocity = 0;
        double mass = 1;
        Cell_3D cell = ScriptableObject.CreateInstance("Cell_3D") as Cell_3D;
        cell.Init(velocity, mass);
        Assert.IsNotNull(cell);
    }
}
