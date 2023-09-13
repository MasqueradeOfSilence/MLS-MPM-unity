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

    // Thresholding
    double maxMicroscopicSize = 50;
    double maxSmallSize = 100;
    double maxMediumSize = 143.5;

    private BubbleSize bubbleSize = BubbleSize.MEDIUM;

    public Bubble()
    {
        // Can't call this directly due to it being a ScriptableObject
    }

    public void InstantiateBubble(double volumeFraction)
    {
        if (volumeFraction <= maxMicroscopicSize)
        {
            Debug.Log("Micro");
            bubbleSize = BubbleSize.MICROSCOPIC;
        }
        else if (volumeFraction <= maxSmallSize)
        {
            Debug.Log("Sm");
            bubbleSize = BubbleSize.SMALL;
        }
        else if (volumeFraction <= maxMediumSize)
        {
            Debug.Log("Med");
            bubbleSize = BubbleSize.MEDIUM;
        }
        else
        {
            Debug.Log("Lg");
            bubbleSize = BubbleSize.LARGE;
        }
    }

    public float ComputeUnitySphereRadius()
    {
        // TODO for some reason the spheres are still created with 0.1f on everything despite different bubble sizes. 
        // Med should actually be 0.5 and large should be 0.8, but I am going bigger because I need to see some sort of results.
        return bubbleSize switch
        {
            BubbleSize.MICROSCOPIC => 0.1f,
            BubbleSize.SMALL => 0.3f,
            BubbleSize.MEDIUM => 10f,
            BubbleSize.LARGE => 15f,
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
