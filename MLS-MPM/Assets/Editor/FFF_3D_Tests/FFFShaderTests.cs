using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[TestFixture]
public class FFFShaderTests 
{
    [Test]
    public void PointsOnSphereShouldBeComputedAsSuch()
    {
        double3 example_PositionAndCenter = new(1, 2, 3);
        double example_RadiusAndScale = 4;

        // Test points that should be on the sphere, meaning they satisfy the sphere equation
        // (x-1)^2 + (y-2)^2 + (z-3)^2 = 4^2 

        // Points that should not be on the sphere

        // Moving formula here for testing first -- just the math itself

    }
}
