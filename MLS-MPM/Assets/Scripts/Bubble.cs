using UnityEngine;

public class Bubble : ScriptableObject
{
    public enum BubbleSize 
    { 
        SKIP,
        MICROSCOPIC, 
        SMALL, 
        MEDIUM, 
        LARGE 
    };

    // Thresholding: 128.8 - 134 range has the most bubbles.
    private readonly double maxMicroscopicSize = 50;
    private readonly double maxSmallSize = 100;
    private readonly double maxMediumSize = 134.5;

    private BubbleSize bubbleSize = BubbleSize.MEDIUM;
    private double volumeFraction = 0;
    private float radius = -1;
    private bool instantiated = false;

    public Bubble()
    {
        // Can't call this directly due to it being a ScriptableObject
    }

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
        if (radius == -1)
        {
            ComputeUnitySphereRadius();
        }
    }

    public float ComputeUnitySphereRadius()
    {
        // Commenting out for now. Do we want to avoid a re-init since we are not changing bubble sizes after first time?
        if (radius != -1)
        {
            return radius;
        }
        double scalingFactor = 0.01 * volumeFraction;
        // Might want more jitter.
        float randomJitter = Random.Range(-0.01f, 0.01f);
        float scalingFactorFloat = (float)scalingFactor;
        scalingFactorFloat += randomJitter;
        radius = bubbleSize switch
        {
            BubbleSize.SKIP => 0,
            BubbleSize.MICROSCOPIC => 0.1f + scalingFactorFloat,
            BubbleSize.SMALL => 0.15f + scalingFactorFloat,
            BubbleSize.MEDIUM => 0.3f + scalingFactorFloat,
            BubbleSize.LARGE => 0.4f + scalingFactorFloat,
            _ => 0.1f,
        } ;
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
