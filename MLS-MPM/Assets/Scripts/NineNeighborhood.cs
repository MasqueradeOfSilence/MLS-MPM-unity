using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class NineNeighborhood
{
    private int2 upperLeft;
    private int2 upper;
    private int2 upperRight;
    private int2 left;
    private int2 center;
    private int2 right;
    private int2 lowerLeft;
    private int2 lower;
    private int2 lowerRight;

    public NineNeighborhood(Particle particle)
    {
        this.center = new(particle.GetPosition());
        this.upperLeft = new(this.center[0] - 1, this.center[1] - 1);
        this.upper = new(this.center[0] - 1, this.center[1]);
        this.upperRight = new(this.center[0] - 1, this.center[1] + 1);
        this.left = new(this.center[0], this.center[1] - 1);
        this.right = new(this.center[0], this.center[1] + 1);
        this.lowerLeft = new(this.center[0] + 1, this.center[1] - 1);
        this.lower = new(this.center[0] + 1, this.center[1]);
        this.lowerRight = new(this.center[0] + 1, this.center[1] + 1);
    }

    public int2 GetUpperLeft()
    {
        return upperLeft;
    }

    public int2 GetUpper()
    {
        return upper;
    }

    public int2 GetUpperRight()
    {
        return upperRight;
    }

    public int2 GetLeft()
    {
        return left;
    }

    public int2 GetCenter()
    {
        return center;
    }

    public int2 GetRight()
    {
        return right;
    }

    public int2 GetLowerLeft()
    {
        return lowerLeft;
    }

    public int2 GetLower()
    {
        return lower;
    }

    public int2 GetLowerRight()
    {
        return lowerRight;
    }
}
