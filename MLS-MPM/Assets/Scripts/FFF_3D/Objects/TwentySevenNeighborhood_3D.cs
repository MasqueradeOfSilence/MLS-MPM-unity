using System;
using Unity.Mathematics;
using UnityEngine.UIElements;

/**
 * The 3D neighborhood around a particle. 
 */
public class TwentySevenNeighborhood_3D
{
    private int3 upperLeft;
    private int3 upper;
    private int3 upperRight;
    private int3 left;
    private int3 center;
    private int3 right;
    private int3 lowerLeft;
    private int3 lower;
    private int3 lowerRight;
    private int3 upperLeftFront;
    private int3 upperLeftBack;
    private int3 upperFront;
    private int3 upperBack;
    private int3 upperRightFront;
    private int3 upperRightBack;
    private int3 leftFront;
    private int3 leftBack;
    private int3 centerFront;
    private int3 centerBack;
    private int3 rightFront;
    private int3 rightBack;
    private int3 lowerLeftFront;
    private int3 lowerLeftBack;
    private int3 lowerFront;
    private int3 lowerBack;
    private int3 lowerRightFront;
    private int3 lowerRightBack;

    public TwentySevenNeighborhood_3D(Particle_3D p)
    {
        // We determine the corresponding cell using rounding within the int3 constructor.
        center = new(p.GetPosition());

        // Upper layer
        upperLeftFront = new(center.x - 1, center.y - 1, center.z - 1);
        upperLeft = new(center.x - 1, center.y - 1, center.z);
        upperLeftBack = new(center.x - 1, center.y - 1, center.z + 1);

        upperFront = new(center.x, center.y - 1, center.z - 1);
        upper = new(center.x, center.y - 1, center.z);
        upperBack = new(center.x, center.y - 1, center.z + 1);

        upperRightFront = new(center.x + 1, center.y - 1, center.z - 1);
        upperRight = new(center.x + 1, center.y - 1, center.z);
        upperRightBack = new(center.x + 1, center.y - 1, center.z + 1);

        // Middle layer
        leftFront = new(center.x - 1, center.y, center.z - 1);
        left = new(center.x - 1, center.y, center.z);
        leftBack = new(center.x - 1, center.y, center.z + 1);

        centerFront = new(center.x, center.y, center.z - 1);
        // Omitted: center = new(center.x, center.y, center.z); (already defined)

        centerBack = new(center.x, center.y, center.z + 1);

        rightFront = new(center.x + 1, center.y, center.z - 1);
        right = new(center.x + 1, center.y, center.z);
        rightBack = new(center.x + 1, center.y, center.z + 1);

        // Lower layer
        lowerLeftFront = new(center.x - 1, center.y + 1, center.z - 1);
        lowerLeft = new(center.x - 1, center.y + 1, center.z);
        lowerLeftBack = new(center.x - 1, center.y + 1, center.z + 1);

        lowerFront = new(center.x, center.y + 1, center.z - 1);
        lower = new(center.x, center.y + 1, center.z);
        lowerBack = new(center.x, center.y + 1, center.z + 1);

        lowerRightFront = new(center.x + 1, center.y + 1, center.z - 1);
        lowerRight = new(center.x + 1, center.y + 1, center.z);
        lowerRightBack = new(center.x + 1, center.y + 1, center.z + 1);
    }

    private bool IsParticleInsideNeighbor(Particle_3D p)
    {
        int3 castedPosition = new(p.GetPosition());
        return castedPosition.Equals(center) ||
        castedPosition.Equals(upperLeftFront) || castedPosition.Equals(upperLeft) || castedPosition.Equals(upperLeftBack) ||
        castedPosition.Equals(upperFront) || castedPosition.Equals(upper) || castedPosition.Equals(upperBack) ||
        castedPosition.Equals(upperRightFront) || castedPosition.Equals(upperRight) || castedPosition.Equals(upperRightBack) ||
        castedPosition.Equals(leftFront) || castedPosition.Equals(left) || castedPosition.Equals(leftBack) ||
        castedPosition.Equals(centerFront) || castedPosition.Equals(centerBack) ||
        castedPosition.Equals(rightFront) || castedPosition.Equals(right) || castedPosition.Equals(rightBack) ||
        castedPosition.Equals(lowerLeftFront) || castedPosition.Equals(lowerLeft) || castedPosition.Equals(lowerLeftBack) ||
        castedPosition.Equals(lowerFront) || castedPosition.Equals(lower) || castedPosition.Equals(lowerBack) ||
        castedPosition.Equals(lowerRightFront) || castedPosition.Equals(lowerRight) || castedPosition.Equals(lowerRightBack);
    }
    public bool ContainsParticle(Particle_3D p)
    {
        return IsParticleInsideNeighbor(p);
    }

    public int3 GetCenter() => center;

    // Upper layer
    public int3 GetUpperLeftFront() => upperLeftFront;
    public int3 GetUpperLeft() => upperLeft;
    public int3 GetUpperLeftBack() => upperLeftBack;

    public int3 GetUpperFront() => upperFront;
    public int3 GetUpper() => upper;
    public int3 GetUpperBack() => upperBack;

    public int3 GetUpperRightFront() => upperRightFront;
    public int3 GetUpperRight() => upperRight;
    public int3 GetUpperRightBack() => upperRightBack;

    // Middle layer
    public int3 GetLeftFront() => leftFront;
    public int3 GetLeft() => left;
    public int3 GetLeftBack() => leftBack;

    public int3 GetCenterFront() => centerFront;
    // Omitted: GetCenter() (already defined)
    public int3 GetCenterBack() => centerBack;

    public int3 GetRightFront() => rightFront;
    public int3 GetRight() => right;
    public int3 GetRightBack() => rightBack;

    // Lower layer
    public int3 GetLowerLeftFront() => lowerLeftFront;
    public int3 GetLowerLeft() => lowerLeft;
    public int3 GetLowerLeftBack() => lowerLeftBack;

    public int3 GetLowerFront() => lowerFront;
    public int3 GetLower() => lower;
    public int3 GetLowerBack() => lowerBack;

    public int3 GetLowerRightFront() => lowerRightFront;
    public int3 GetLowerRight() => lowerRight;
    public int3 GetLowerRightBack() => lowerRightBack;


}
