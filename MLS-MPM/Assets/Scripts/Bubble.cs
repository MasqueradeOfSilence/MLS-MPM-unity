using UnityEngine;

public class Bubble : ScriptableObject
{
    public enum BubbleSize 
    { 
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

    public Bubble()
    {
        // Can't call this directly due to it being a ScriptableObject
    }

    public void InstantiateBubble(double volumeFraction)
    {
        if (volumeFraction <= maxMicroscopicSize)
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
    }

    public float ComputeUnitySphereRadius()
    {
        return bubbleSize switch
        {
            BubbleSize.MICROSCOPIC => 0.1f,
            BubbleSize.SMALL => 0.2f,
            BubbleSize.MEDIUM => 0.6f,
            BubbleSize.LARGE => 0.8f,
            _ => 0.1f,
        };
    }

    public void SetBubbleSize(BubbleSize bubbleSize) 
    {
        this.bubbleSize = bubbleSize;
    }

    public BubbleSize GetBubbleSize()
    {
        return bubbleSize;
    }
}
