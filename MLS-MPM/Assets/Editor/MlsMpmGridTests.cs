using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class MlsMpmGridTests
{
    [Test]
    public void MlsMpmGridClassShouldExist()
    {
        int gridResolution = 64;
        MlsMpmGrid mlsMpmGrid = ScriptableObject.CreateInstance("MlsMpmGrid") as MlsMpmGrid;
        mlsMpmGrid.InitMlsMpmGrid(gridResolution);
        Assert.IsNotNull(mlsMpmGrid);
        Assert.AreEqual(mlsMpmGrid.GetGridResolution(), gridResolution);
        Assert.AreEqual(mlsMpmGrid.GetSize()[0], gridResolution);
        Assert.AreEqual(mlsMpmGrid.GetSize()[1], gridResolution);
    }
    
}
