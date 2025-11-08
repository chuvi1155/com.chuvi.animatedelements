using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class AnimatedScale : AnimatedBehaviour
{
    public bool UseSize = false;
    public Vector3 to_scale = Vector3.one;
    public Vector3 from_scale = Vector3.one;
    public Vector2 to_size = Vector2.one;
    public Vector2 from_size = Vector2.one;
    public FromCurrentSize FromCurrentSize = FromCurrentSize.None;

    public AnimatedScale() : base()
    {
    }
    public AnimatedScale(Transform _mainTransform) : base(_mainTransform)
    {
    }

    protected override object GetFrom()
    {
        return from_scale;
    }

    protected override object GetTo()
    {
        return to_scale;
    }

    protected override void OnAction(float _time, float curveValue)
    {
        if (!UseSize)
        {
            Vector3 newScale = Lerp(from_scale, to_scale, curveValue);
            mainTransform.localScale = newScale;
        }
        else
        {
            Vector2 newScale = Lerp(from_size, to_size, curveValue);
            rtr.sizeDelta = newScale;
        }
    }

    public override void ResetInFromState()
    {
        if (!Used) return;
        t = 0;
        onEnded = false;
        InitTransform();
        if (FromCurrentSize == FromCurrentSize.Use)
            from_scale = UseSize ? rtr.sizeDelta : mainTransform.localScale;
        OnAction(0, curve.Evaluate(0));
    }

    public override void ResetInToState()
    {
        if (!Used) return;
        t = 0;
        onEnded = false;
        InitTransform();

        OnAction(AnimationTime, curve.Evaluate(1));
    }

    protected override void SetFrom(object from)
    {
        if (from is Vector3 || from is Vector2)
            from_scale = (Vector3)from;
    }

    protected override void SetTo(object to)
    {
        if (to is Vector3 || to is Vector2)
            to_scale = (Vector3)to;
    }
}
