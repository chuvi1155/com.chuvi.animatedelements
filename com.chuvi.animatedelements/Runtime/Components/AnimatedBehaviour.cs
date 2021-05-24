using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class AnimatedBehaviour
{
    protected float t = 0;
    protected bool onEnded = false;
    public float AnimatedTimeTick { get { return t; } }
    public float CurveValue { get; private set; }
    public bool IsWaiting { get; private set; }

    [SerializeField]
    public RectTransform rtr { get; private set; }
    protected abstract object GetFrom();
    protected abstract void SetFrom(object from);
    protected abstract object GetTo();
    protected abstract void SetTo(object to);
    [SerializeField]
    Transform _mainTransform;
    public Transform mainTransform
    {
        get { return _mainTransform; }
        set
        {
            _mainTransform = value;
            InitTransform();
        }
    }

    public bool Used = false;
    public float AnimationTime = 1;
    public bool UseRandomTime = false;
    public float MaxRandomTime = 1;
    public bool IsRepeat = false;
    public float TimeWaitRepeate = 0;
    public bool UseRandomTimeWaitRepeate = false;
    public float MaxRandomTimeWaitRepeate = 1;

    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimElemEventFull StartAnimation;
    public AnimElemEventFull OnAnimation;
    public AnimElemEventFull EndAnimation;
    public bool HasAnimated
    {
        get { return Used && !onEnded; }
    }

    [System.Serializable]
    public class AnimElemEvent : UnityEngine.Events.UnityEvent<float> { }
    [System.Serializable]
    public class AnimElemEventFull : UnityEngine.Events.UnityEvent<AnimatedBehaviour, float> { }
    public float GetMaxCurveValue
    {
        get
        {
            if (curve.length > 0)
            {
                float maxVal = float.MinValue;
                for (int i = 0; i < curve.keys.Length; i++)
                {
                    maxVal = Mathf.Max(curve.keys[i].value, maxVal);
                }
                return maxVal;
            }
            return 0;
        }
    }
    public float GetMinCurveValue
    {
        get
        {
            if (curve.length > 0)
            {
                float maxVal = float.MaxValue;
                for (int i = 0; i < curve.keys.Length; i++)
                {
                    maxVal = Mathf.Min(curve.keys[i].value, maxVal);
                }
                return maxVal;
            }
            return 0;
        }
    }

    public AnimatedBehaviour()
    {
        StartAnimation = new AnimElemEventFull();
        OnAnimation = new AnimElemEventFull();
        EndAnimation = new AnimElemEventFull();
        curve = AnimationCurve.Linear(0, 0, 1, 1);
    }
    public AnimatedBehaviour(Transform _mainTransform) : this()
    {
        mainTransform = _mainTransform;
    }

    public virtual void InitTransform()
    {
        if (_mainTransform != null)
            rtr = _mainTransform.GetComponent<RectTransform>();
        else rtr = null;
    }

    public void Update(float deltaTime)
    {
        if (!Used) return;
        if (!onEnded)
        {
            if (t == 0)
            {
                OnStartTime();
                StartAnimation.Invoke(this, 0);
            }
            OnAnimation.Invoke(this, t);
            if (IsRepeat)
            {
                if (t >= AnimationTime)
                {
                    if (t - AnimationTime >= TimeWaitRepeate)
                    {
                        OnTimeWaitRepeateRestart();
                        if (UseRandomTimeWaitRepeate)
                            TimeWaitRepeate = Random.Range(0, MaxRandomTimeWaitRepeate);
                        t = 0;
                        if (UseRandomTime)
                            AnimationTime = Random.Range(0, MaxRandomTime);

                        float ttt = GetTime(Mathf.Repeat(t, AnimationTime), AnimationTime);
                        CurveValue = curve.Evaluate(ttt);
                        OnAction(ttt, CurveValue);
                        EndAnimation.Invoke(this, ttt);
                        IsWaiting = false;
                    }
                    else
                    {
                        if (!IsWaiting)
                        {
                            CurveValue = curve.Evaluate(1);
                            OnAction(1, CurveValue);
                            EndAnimation.Invoke(this, t);
                        }
                        IsWaiting = true;
                    }
                }
                else
                {
                    IsWaiting = false;
                    float ttt = GetTime(Mathf.Repeat(t, AnimationTime), AnimationTime);
                    CurveValue = curve.Evaluate(ttt);
                    OnAction(ttt, CurveValue);
                }
                t += deltaTime;
            }
            else
            {
                IsWaiting = false;
                float ttt = GetTime(t, AnimationTime);
                CurveValue = curve.Evaluate(ttt);
                OnAction(ttt, CurveValue);
                if (ttt >= 1f)
                {
                    OnEndTime();
                    ResetInToState();
                    onEnded = true;
                    EndAnimation.Invoke(this, t);
                }
                else t += deltaTime;
            }
        }
    }

    protected abstract void OnAction(float _time, float curveValue);
    public abstract void ResetInToState();
    public abstract void ResetInFromState();

    public void Restart()
    {
        onEnded = false;
        t = 0;
        ResetInFromState();
    }

    public virtual void ReverseCurve()
    {
        for (int i = 0; i < curve.length / 2; i++)
        {
            Keyframe t = curve.keys[i];
            Keyframe last = curve.keys[curve.length - 1 - i];

            curve.MoveKey(i, new Keyframe(t.time, last.value, last.inTangent, last.outTangent));
            curve.MoveKey(curve.length - 1 - i, new Keyframe(last.time, t.value, t.inTangent, t.outTangent));
        }
    }
    protected virtual void OnEndTime() { }
    protected virtual void OnTimeWaitRepeateRestart() { }
    protected virtual void OnStartTime() { }
    protected static float GetTime(float time, float maxTime)
    {
        return maxTime != 0 ? time / maxTime : 1f;
    }
    protected static float GetTime0(float time, float maxTime)
    {
        return maxTime != 0 ? time / maxTime : 0;
    }

    protected static Vector2 Lerp(Vector2 from, Vector2 to, float t)
    {
        return new Vector2(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t);
    }
    protected static Vector3 Lerp(Vector3 from, Vector3 to, float t)
    {
        return new Vector3(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t);
    }
}