using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
// Tutorial from https://www.studytonight.com/game-development-in-2D/tdd-unit-testing#:~:text=Test%2Ddriven%20development%20(TDD),on%20the%20basis%20of%20tests.

public class Function
{
    public float Value(float x)
    {
        return (Mathf.Pow(x, 2) - (4f * x) + 4f);
    }
}

[TestFixture]
public class FunctionTester
{
    public Function function = new Function();

    [Test]
    public void T00_PassingTest()
    {
        Assert.AreEqual(1, 1);
    }

    [Test]
    public void T01_X2Y0()
    {
        Assert.AreEqual(function.Value(2f), 0f);
    }

    [Test]
    public void T02_X0Y4()
    {
        Assert.AreEqual(function.Value(0f), 4f);
    }
}
