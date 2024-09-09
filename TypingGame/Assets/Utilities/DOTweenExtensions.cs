using DG.Tweening;
using System.Linq;

public static class DOTweenExtensions
{
    public static Sequence AppendIfValid(this Sequence sequence, Tween tweenToAppend)
    {
        if (tweenToAppend.IsActive())
            sequence.Append(tweenToAppend);

        return sequence;
    }
}