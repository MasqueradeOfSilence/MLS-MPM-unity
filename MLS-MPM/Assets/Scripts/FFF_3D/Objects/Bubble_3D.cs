using UnityEngine;

public class Bubble_3D : ScriptableObject
{
    public enum BubbleSize
    {
        SKIP,
        MICROSCOPIC,
        SMALL,
        MEDIUM,
        LARGE
    };

    // 105 - 421 is the range in 3D
    private readonly double maxMicroscopicSize = 130;
    private readonly double maxSmallSize = 300;
    private readonly double maxMediumSize = 415;

    private BubbleSize bubbleSize = BubbleSize.MEDIUM;
    private double volumeFraction = 0;
    private float radius = -100;
    private bool instantiated = false;

    public void InstantiateBubble(double volumeFraction, bool skipMe = false)
    {
        if (skipMe)
        {
            bubbleSize = BubbleSize.SKIP;
        }
        else if (volumeFraction <= maxMicroscopicSize)
        {
            bubbleSize = BubbleSize.MICROSCOPIC;
        }
        else if (volumeFraction <= maxSmallSize)
        {
            bubbleSize = BubbleSize.SMALL;
        }
        else if (volumeFraction <= maxMediumSize)
        {
            bubbleSize = BubbleSize.MEDIUM;
        }
        else
        {
            bubbleSize = BubbleSize.LARGE;
        }
        this.volumeFraction = volumeFraction;
        instantiated = true;
        if (radius == -100)
        {
            ComputeUnitySphereRadius();
        }
    }

    public float ComputeUnitySphereRadius()
    {
        if (radius != -100)
        {
            return radius;
        }
        // Scaling factor computed experimentally. Lots of experimentation here
        double scalingFactor = 0.0075 * volumeFraction;
        // Might want more jitter.
        float randomJitter = Random.Range(-0.05f, 0.05f);
        float scalingFactorFloat = (float)scalingFactor;
        float tinyScalingFactorFloat = scalingFactorFloat * 0.1f;
        scalingFactorFloat += randomJitter;
        radius = bubbleSize switch
        {
            BubbleSize.SKIP => 0,
            BubbleSize.MICROSCOPIC => 0.01f + tinyScalingFactorFloat, // Microscopic = Water.
            BubbleSize.SMALL => 0.05f + scalingFactorFloat,
            BubbleSize.MEDIUM => 0.1f + scalingFactorFloat,
            BubbleSize.LARGE => 0.15f + scalingFactorFloat,
            _ => 0.1f,
        };
        return radius;
    }

    public void SetBubbleSize(BubbleSize bubbleSize)
    {
        this.bubbleSize = bubbleSize;
    }

    public BubbleSize GetBubbleSize()
    {
        return bubbleSize;
    }

    public double GetVolumeFraction()
    {
        return volumeFraction;
    }

    public float GetRadius()
    {
        return radius;
    }

    public bool IsInstantiated()
    {
        return instantiated;
    }
}
