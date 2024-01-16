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
    double maxMicroscopicSize = 50;
    double maxSmallSize = 100;
    double maxMediumSize = 134.5;

    private BubbleSize bubbleSize = BubbleSize.MEDIUM;
    private double volumeFraction = 0;

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
    }

    public float ComputeUnitySphereRadius()
    {
        double scalingFactor = 0.01 * volumeFraction;
        float randomJitter = Random.Range(-0.01f, 0.01f);
        float scalingFactorFloat = (float)scalingFactor;
        scalingFactorFloat += randomJitter;
        return bubbleSize switch
        {
            BubbleSize.SKIP => 0,
            BubbleSize.MICROSCOPIC => 0.1f + scalingFactorFloat,
            BubbleSize.SMALL => 0.15f + scalingFactorFloat,
            BubbleSize.MEDIUM => 0.3f + scalingFactorFloat,
            BubbleSize.LARGE => 0.4f + scalingFactorFloat,
            _ => 0.1f,
        } ;
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
}
