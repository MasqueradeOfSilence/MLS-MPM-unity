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
        // https://www.wolframalpha.com/input?i=%28x-1%29%5E2%2B%28y-2%29%5E2%2B%28z-3%29%5E2%3D4%5E2
        double3 example_PositionAndCenter = new(1, 2, 3);
        double example_RadiusAndScale = 4;

        // Test points that should be on the sphere, meaning they satisfy the sphere equation (Replacing IN.worldPos)
        // (x-1)^2 + (y-2)^2 + (z-3)^2 = 4^2 

        // Verification: https://www.wolframalpha.com/input?i=%281-1%29%5E2%2B%282-2%29%5E2%2B%287-3%29%5E2%3D4%5E2
        double3 worldPos1 = new(1, 2, 7);
        // Verification: https://www.wolframalpha.com/input?i=%285-1%29%5E2%2B%282-2%29%5E2%2B%283-3%29%5E2%3D4%5E2
        double3 worldPos2 = new(5, 2, 3);
        // Verification: https://www.wolframalpha.com/input?i=%281-1%29%5E2%2B%286-2%29%5E2%2B%283-3%29%5E2%3D4%5E2
        double3 worldPos3 = new(1, 6, 3);

        // Points that should not be on the sphere
        double3 worldPos4 = new(77, 0, 1924);
        double3 worldPos5 = new(666, 2, 0);

        // Moving formula here for testing first -- just the math itself
        // bool onCurrentSphere = distance(_SphereCenters[j].xyz, IN.worldPos) <= (_SphereRadii[j] * radiusOfCollider);
        double distance1 = math.distance(example_PositionAndCenter, worldPos1);
        double distance2 = math.distance(example_PositionAndCenter, worldPos2);
        double distance3 = math.distance(example_PositionAndCenter, worldPos3);
        double distance4 = math.distance(example_PositionAndCenter, worldPos4);
        double distance5 = math.distance(example_PositionAndCenter, worldPos5);
        Assert.IsTrue(distance1 <= example_RadiusAndScale); // No multiplying necessary right now
        Assert.IsTrue(distance2 <= example_RadiusAndScale);
        Assert.IsTrue(distance3 <= example_RadiusAndScale);
        Assert.IsFalse(distance4 <= example_RadiusAndScale);
        Assert.IsFalse(distance5 <= example_RadiusAndScale);
    }
}
