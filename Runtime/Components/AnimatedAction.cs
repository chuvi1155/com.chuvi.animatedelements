using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class AnimatedAction : AnimatedBehaviour
{
    [System.Flags]
    public enum ActionTypes
    {
        ClampedAction = 1 << 0,
        InvertClampedAction = 1 << 1,
        CustomValueAction = 1 << 2,
        CurvedValueAction = 1 << 3,
        CustomValueRangeAction = 1 << 4,
    }

    public ActionTypes types = 0;
    public AnimElemEvent ClampedAction;
    public AnimElemStrEvent ToStringClampedAction;
    public AnimElemEvent InvertClampedAction;
    public AnimElemStrEvent ToStringInvertClampedAction;
    public AnimElemEvent CustomValueAction;
    public AnimElemStrEvent ToStringCustomValueAction;
    public AnimElemEvent CustomRangeValueAction;
    public AnimElemStrEvent ToStringCustomRangeValueAction;
    public AnimElemEvent CurvedValueAction;
    public AnimElemStrEvent ToStringCurvedValueAction;

    public bool ToStringEvent = false;
    public bool AsIntValue = false;
    public float MaxCustomValue;
    public bool CustomActionReversed = false;
    public float FromCustomRangeValue;
    public float ToCustomRangeValue;

    public AnimatedAction() : base()
    {
    }
    public AnimatedAction(Transform _mainTransform) : base(_mainTransform)
    {
    }

    public override void ResetInFromState()
    {
        if (!Used) return;
        t = 0;
        if (Exists(ActionTypes.ClampedAction))
        {
            ClampedAction.Invoke(0);
            if (ToStringEvent) ToStringClampedAction.Invoke("0");
        }
        if (Exists(ActionTypes.InvertClampedAction))
        {
            InvertClampedAction.Invoke(1);
            if (ToStringEvent) ToStringInvertClampedAction.Invoke("1");
        }
        if (Exists(ActionTypes.CustomValueAction))
        {
            CustomValueAction.Invoke(0);
            if (ToStringEvent) ToStringCustomValueAction.Invoke("0");
        }
        if (Exists(ActionTypes.CurvedValueAction))
        {
            CurvedValueAction.Invoke(curve.keys[0].value);
            if (ToStringEvent) ToStringCurvedValueAction.Invoke((AsIntValue ? (int)curve.keys[0].value : curve.keys[0].value).ToString());
        }
        if (Exists(ActionTypes.CustomValueRangeAction))
        {
            CustomRangeValueAction.Invoke(FromCustomRangeValue);
            if (ToStringEvent) ToStringCustomRangeValueAction.Invoke((AsIntValue ? (int)FromCustomRangeValue : FromCustomRangeValue).ToString());
        }
        onEnded = false;
    }

    public override void ResetInToState()
    {
        if (!Used) return;
        t = 0;
        if (Exists(ActionTypes.ClampedAction))
        {
            ClampedAction.Invoke(1);
            if (ToStringEvent) ToStringClampedAction.Invoke("1");
        }
        if (Exists(ActionTypes.InvertClampedAction))
        {
            InvertClampedAction.Invoke(0);
            if (ToStringEvent) ToStringInvertClampedAction.Invoke("0");
        }
        if (Exists(ActionTypes.CustomValueAction))
        {
            CustomValueAction.Invoke(MaxCustomValue);
            if (ToStringEvent) ToStringCustomValueAction.Invoke((AsIntValue ? (int)MaxCustomValue : MaxCustomValue).ToString());
        }
        if (Exists(ActionTypes.CurvedValueAction))
        {
            CurvedValueAction.Invoke(curve.keys[curve.keys.Length - 1].value);
            if (ToStringEvent) ToStringCurvedValueAction.Invoke((AsIntValue ? (int)curve.keys[curve.keys.Length - 1].value : curve.keys[curve.keys.Length - 1].value).ToString());
        }
        if (Exists(ActionTypes.CustomValueRangeAction))
        {
            CustomRangeValueAction.Invoke(ToCustomRangeValue);
            if (ToStringEvent) ToStringCustomRangeValueAction.Invoke((AsIntValue ? (int)ToCustomRangeValue : ToCustomRangeValue).ToString());
        }
        onEnded = false;
    }

    protected override object GetFrom()
    {
        throw new NotImplementedException();
    }

    protected override object GetTo()
    {
        throw new NotImplementedException();
    }
    
    protected override void OnAction(float _time, float curveValue)
    {

        if (Exists(ActionTypes.ClampedAction))
        {
            ClampedAction.Invoke(_time);
            if (ToStringEvent)
                ToStringClampedAction.Invoke((AsIntValue ? (int)_time : _time).ToString());
        }
        if (Exists(ActionTypes.InvertClampedAction))
        {
            float val = 1f - _time;
            InvertClampedAction.Invoke(val);
            if (ToStringEvent)
                ToStringInvertClampedAction.Invoke((AsIntValue ? (int)val : val).ToString());
        }
        if (Exists(ActionTypes.CustomValueAction))
        {
            var val = MaxCustomValue * (CustomActionReversed ? 1f - curveValue : curveValue);
            CustomValueAction.Invoke(AsIntValue ? (int)val : val);
            if (ToStringEvent)
                ToStringCustomValueAction.Invoke((AsIntValue ? (int)val : val).ToString());
        }
        if (Exists(ActionTypes.CurvedValueAction))
        {
            var val = AsIntValue ? (int)curveValue : curveValue;
            CurvedValueAction.Invoke(val);
            if (ToStringEvent)
                ToStringCurvedValueAction.Invoke((AsIntValue ? (int)val : val).ToString());
        }
        if (Exists(ActionTypes.CustomValueRangeAction))
        {
            var fval = Mathf.Lerp(FromCustomRangeValue, ToCustomRangeValue, _time);
            float nval = AsIntValue ? (int)fval : fval;
            CustomRangeValueAction.Invoke(nval);
            if (ToStringEvent)
            {
                //Debug.Log($"{fval} :: {nval}");
                ToStringCustomRangeValueAction.Invoke(nval.ToString());
            }
        }
    }

    protected override void SetFrom(object from)
    {
        throw new NotImplementedException();
    }

    protected override void SetTo(object to)
    {
        throw new NotImplementedException();
    }

    public bool Exists(ActionTypes type)
    {
        return (types & type) == type;
    }
    [System.Serializable]
    public class AnimElemStrEvent : UnityEngine.Events.UnityEvent<string> { }
}
