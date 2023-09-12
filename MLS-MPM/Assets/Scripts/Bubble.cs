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

    }

    public Bubble(double volumeFraction)
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

    public Bubble(BubbleSize bubbleSize)
    {
        this.bubbleSize = bubbleSize;
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
