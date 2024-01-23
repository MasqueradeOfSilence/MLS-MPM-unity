using NUnit.Framework;
using UnityEngine;
using Unity.Mathematics;

[TestFixture]
public class Grid_3D_Test
{
    [Test]
    public void GridClassShouldExist()
    {
        int resolution = 64;
        Grid_3D grid = ScriptableObject.CreateInstance("Grid_3D") as Grid_3D;
        grid.Init(resolution);
        Assert.IsNotNull(grid);
        Assert.AreEqual(grid.GetResolution(), resolution);
        Assert.AreEqual(grid.GetSize(), new int3(resolution, resolution, resolution));
    }
}
