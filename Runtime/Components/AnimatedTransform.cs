using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimatedTransform : AnimatedBehaviour
{
    public Vector3 to_transformPos;
    public Vector3 from_transformPos;
    public bool UseRandomPositions = false;
    public FromCurrentPosition FromCurrentPosition = FromCurrentPosition.None;
    Vector3? to_transformPos_const;
    Vector3? from_transformPos_const;
    public AnimatedTransform() : base()
    {
    }
    public AnimatedTransform(Transform _mainTransform) : base(_mainTransform)
    {
    }

    protected override object GetFrom()
    {
        return from_transformPos;
    }

    protected override object GetTo()
    {
        return to_transformPos;
    }

    protected override void OnAction(float _time, float curveValue)
    {
        Vector3 newPos = Lerp(from_transformPos, to_transformPos, curveValue);
        if (rtr != null) rtr.anchoredPosition = newPos;
        else mainTransform.localPosition = newPos;
    }

    public override void ResetInFromState()
    {
        if (!Used) return;
        t = 0;
        onEnded = false;
        InitTransform();
        if (FromCurrentPosition == FromCurrentPosition.World)
            from_transformPos = mainTransform.position;
        else if (FromCurrentPosition == FromCurrentPosition.Local)
            from_transformPos = mainTransform.localPosition;
        else if (FromCurrentPosition == FromCurrentPosition.Local2D && rtr != null)
            from_transformPos = rtr.anchoredPosition;
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
            from_transformPos = (Vector3)from;
    }
    protected override void SetTo(object to)
    {
        if (to is Vector3 || to is Vector2)
            to_transformPos = (Vector3)to;
    }
    
    protected override void OnEndTime()
    {
        base.OnEndTime();
        if (UseRandomPositions)
        {
            if (!to_transformPos_const.HasValue)
                to_transformPos_const = to_transformPos;
            if (!from_transformPos_const.HasValue)
                from_transformPos_const = from_transformPos;
            Vector3 from = from_transformPos_const.Value;
            Vector3 to = to_transformPos_const.Value;
            to_transformPos = new Vector3(Random.Range(from.x, to.x),
                                          Random.Range(from.y, to.y),
                                          Random.Range(from.z, to.z));
            if (rtr != null) from_transformPos = rtr.anchoredPosition;
            else from_transformPos = mainTransform.localPosition;
        }
    }

    protected override void OnTimeWaitRepeateRestart()
    {
        base.OnTimeWaitRepeateRestart();
        if (UseRandomPositions)
        {
            if (UseRandomPositions)
            {
                if (!to_transformPos_const.HasValue)
                    to_transformPos_const = to_transformPos;
                if (!from_transformPos_const.HasValue)
                    from_transformPos_const = from_transformPos;
                Vector3 from = from_transformPos_const.Value;
                Vector3 to = to_transformPos_const.Value;
                to_transformPos = new Vector3(Random.Range(from.x, to.x),
                                              Random.Range(from.y, to.y),
                                              Random.Range(from.z, to.z));
                if (rtr != null) from_transformPos = rtr.anchoredPosition;
                else from_transformPos = mainTransform.localPosition;
            }
        }
    }
}

