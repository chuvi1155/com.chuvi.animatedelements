using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimatedRotation : AnimatedBehaviour
{
    public Vector3 to_rotation;
    public Vector3 from_rotation;
    public bool UseRandomPositions = false;
    public FromCurrentPosition FromCurrentPosition = FromCurrentPosition.None;
    Vector3? to_rotation_const;
    Vector3? from_rotation_const;

    public AnimatedRotation() : base()
    {
    }
    public AnimatedRotation(Transform _mainTransform) : base(_mainTransform)
    {
    }

    protected override object GetFrom()
    {
        return from_rotation;
    }

    protected override object GetTo()
    {
        return to_rotation;
    }

    protected override void OnAction(float _time, float curveValue)
    {
        Vector3 newRot = new Vector3
                        (
                            Mathf.Lerp(from_rotation.x, to_rotation.x, curveValue),
                            Mathf.Lerp(from_rotation.y, to_rotation.y, curveValue),
                            Mathf.Lerp(from_rotation.z, to_rotation.z, curveValue)
                        );
        mainTransform.localEulerAngles = newRot;
    }

    public override void ResetInFromState()
    {
        if (!Used) return;
        t = 0;
        onEnded = false;
        InitTransform();
        if (FromCurrentPosition == FromCurrentPosition.World)
            from_rotation = mainTransform.position;
        else if (FromCurrentPosition == FromCurrentPosition.Local)
            from_rotation = mainTransform.localPosition;
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
        if (from is Vector3)
            from_rotation = (Vector3)from;
    }

    protected override void SetTo(object to)
    {
        if (to is Vector3)
            to_rotation = (Vector3)to;
    }
    protected override void OnEndTime()
    {
        base.OnEndTime();
        if (UseRandomPositions)
        {
            if (!to_rotation_const.HasValue)
                to_rotation_const = to_rotation;
            if (!from_rotation_const.HasValue)
                from_rotation_const = from_rotation;
            Vector3 from = from_rotation_const.Value;
            Vector3 to = to_rotation_const.Value;
            to_rotation = new Vector3(Random.Range(from.x, to.x),
                                          Random.Range(from.y, to.y),
                                          Random.Range(from.z, to.z));
            if (rtr != null) from_rotation = rtr.localEulerAngles;
            else from_rotation = mainTransform.localEulerAngles;
        }
    }

    protected override void OnTimeWaitRepeateRestart()
    {
        base.OnTimeWaitRepeateRestart();
        if (UseRandomPositions)
        {
            if (UseRandomPositions)
            {
                if (!to_rotation_const.HasValue)
                    to_rotation_const = to_rotation;
                if (!from_rotation_const.HasValue)
                    from_rotation_const = from_rotation;
                Vector3 from = from_rotation_const.Value;
                Vector3 to = to_rotation_const.Value;
                to_rotation = new Vector3(Random.Range(from.x, to.x),
                                              Random.Range(from.y, to.y),
                                              Random.Range(from.z, to.z));
                if (rtr != null) from_rotation = rtr.localEulerAngles;
                else from_rotation = mainTransform.localEulerAngles;
            }
        }
    }
}
