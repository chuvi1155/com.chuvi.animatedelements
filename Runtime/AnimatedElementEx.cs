using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[AddComponentMenu("Chuvi/Animation/AnimatedElementEx")]
public class AnimatedElementEx : MonoBehaviour
{
    public bool Pause;
    [SerializeField] Transform m_commonTransform;
    public float TimeWait = 0;
    public bool IsRandomTimeWait = false;
    public float MaxRandomTimeWait = 0;



    bool isStartEventRun = false;
    bool isEndEventRun = false;
    float t = 0;


    [Space]
    public AnimatedTransform transformation;
    public AnimatedRotation rotation;
    public AnimatedScale scale;
    public AnimatedColor color;
    public AnimatedSequence sequence;
    public AnimatedAction actions;
    public AnimatedMaterial material;

    public UnityEngine.Events.UnityEvent StartAnimations;
    public UnityEngine.Events.UnityEvent EndAnimations;

    public UnityEngine.Events.UnityEvent OnEnableEvent;
    public UnityEngine.Events.UnityEvent OnDisableEvent;

    public Transform commonTransform
    {
        get
        {
            if (m_commonTransform == null)
                m_commonTransform = transform;
            return m_commonTransform;
        }
    }
    public bool IsAnimated
    {
        get
        {
            return transformation.HasAnimated ||
                   rotation.HasAnimated ||
                   scale.HasAnimated ||
                   color.HasAnimated ||
                   sequence.HasAnimated ||
                   actions.HasAnimated ||
                   material.HasAnimated;
        }
    }
#if UNITY_EDITOR
    bool isEmulateInEditor = false;
    double dt;
    public bool IsEmulateInEditor
    {
        get { return isEmulateInEditor; }
        set
        {
            isEmulateInEditor = value;
            if (isEmulateInEditor)
            {
                dt = EditorApplication.timeSinceStartup;

                if (transformation.mainTransform == null)
                    transformation.mainTransform = commonTransform;
                if (rotation.mainTransform == null)
                    rotation.mainTransform = commonTransform;
                if (scale.mainTransform == null)
                    scale.mainTransform = commonTransform;
                if (color.mainTransform == null)
                    color.mainTransform = commonTransform;
                if (sequence.mainTransform == null)
                    sequence.mainTransform = commonTransform;
                if (actions.mainTransform == null)
                    actions.mainTransform = commonTransform;
                if (material.mainTransform == null)
                    material.mainTransform = commonTransform;
                ResetInFromState();
                EditorApplication.update += Emulator;
            }
            else
            {
                ResetInFromState();
                EditorApplication.update -= Emulator;
            }
        }
    }

    void Emulator()
    {
        if (EditorApplication.isPlaying) return;
        if (transform == null)
        {
            IsEmulateInEditor = false;
            return;
        }
        float _dt = (float)(EditorApplication.timeSinceStartup - dt);
        OnProcess(_dt);
        dt = EditorApplication.timeSinceStartup;
    }
#endif

    private void Start()
    {        
        if (transformation.mainTransform == null)
            transformation.mainTransform = commonTransform;
        if (rotation.mainTransform == null)
            rotation.mainTransform = commonTransform;
        if (scale.mainTransform == null)
            scale.mainTransform = commonTransform;
        if (color.mainTransform == null)
            color.mainTransform = commonTransform;
        if (sequence.mainTransform == null)
            sequence.mainTransform = commonTransform;
        if (actions.mainTransform == null)
            actions.mainTransform = commonTransform;
        if (material.mainTransform == null)
            material.mainTransform = commonTransform;
    }

#if UNITY_EDITOR
    private void OnDestroy()
    {
        IsEmulateInEditor = false;
    }
#endif

    [ContextMenu("ResetToEnd")]
    public void ResetInToState()
    {
        transformation.ResetInToState();
        rotation.ResetInToState();
        scale.ResetInToState();
        color.ResetInToState();
        sequence.ResetInToState();
        actions.ResetInToState();
        material.ResetInToState();
    }

    [ContextMenu("ResetToStart")]
    public void ResetInFromState()
    {
        transformation.ResetInFromState();
        rotation.ResetInFromState();
        scale.ResetInFromState();
        color.ResetInFromState();
        sequence.ResetInFromState();
        actions.ResetInFromState();
        material.ResetInFromState();
    }

    private void OnEnable()
    {
        Play();
    }

    void InitFields()
    {
        if (transformation == null || OnEnableEvent == null)
        {
            transformation = new AnimatedTransform(commonTransform);
            rotation = new AnimatedRotation(commonTransform);
            scale = new AnimatedScale(commonTransform);
            color = new AnimatedColor(commonTransform);
            sequence = new AnimatedSequence(commonTransform);
            actions = new AnimatedAction(commonTransform);
            material = new AnimatedMaterial(commonTransform);

            StartAnimations = new UnityEngine.Events.UnityEvent();
            EndAnimations = new UnityEngine.Events.UnityEvent();

            OnEnableEvent = new UnityEngine.Events.UnityEvent();
            OnDisableEvent = new UnityEngine.Events.UnityEvent(); 
        }
    }

    public void Play()
    {
        OnEnableEvent.Invoke();
        t = 0;
        isStartEventRun = false;
        isEndEventRun = false;
        if (IsRandomTimeWait)
            TimeWait = Random.Range(0.0f, this.MaxRandomTimeWait);

        if (transformation.mainTransform == null)
            transformation.mainTransform = commonTransform;
        if (rotation.mainTransform == null)
            rotation.mainTransform = commonTransform;
        if (scale.mainTransform == null)
            scale.mainTransform = commonTransform;
        if (color.mainTransform == null)
            color.mainTransform = commonTransform;
        if (sequence.mainTransform == null)
            sequence.mainTransform = commonTransform;
        if (actions.mainTransform == null)
            actions.mainTransform = commonTransform;
        if (material.mainTransform == null)
            material.mainTransform = commonTransform;
        if (!Pause)
            ResetInFromState();
    }

    private void OnDisable()
    {
        OnDisableEvent.Invoke();
    }

    public void Update()
    {
        if (Pause) return;
        float dt = Time.deltaTime;
        OnProcess(dt);
    }

    void OnProcess(float dt)
    {
        if (Pause) return;
        if (t < TimeWait)
        {
            t += dt;
            return;
        }

        if (!isStartEventRun)
        {
            isStartEventRun = true;
            StartAnimations.Invoke();
        }

        if (transformation.mainTransform == null)
            transformation.mainTransform = commonTransform;
        if (rotation.mainTransform == null)
            rotation.mainTransform = commonTransform;
        if (scale.mainTransform == null)
            scale.mainTransform = commonTransform;
        if (color.mainTransform == null)
            color.mainTransform = commonTransform;
        if (sequence.mainTransform == null)
            sequence.mainTransform = commonTransform;
        if (actions.mainTransform == null)
            actions.mainTransform = commonTransform;
        if (material.mainTransform == null)
            material.mainTransform = commonTransform;

        transformation.Update(dt);
        rotation.Update(dt);
        scale.Update(dt);
        color.Update(dt);
        sequence.Update(dt);
        actions.Update(dt);
        material.Update(dt);

        if (!isEndEventRun)
        {
            if (!transformation.HasAnimated && !rotation.HasAnimated && !scale.HasAnimated && !color.HasAnimated && !sequence.HasAnimated)
            {
                EndAnimations.Invoke();
                isEndEventRun = true;
            }
        }
    }

    public void SetPause(bool pause)
    {
        Pause = pause;
    }

    public void ReverseAll()
    {
        ReverseTransformation();
        ReverseRotation();
        ReverseScale();
        ReverseColor();
        ReverseSequence();
        ReverseActions();
    }

    public void ReverseTransformation()
    {
        transformation.ReverseCurve();
    }
    public void ReverseRotation()
    {
        rotation.ReverseCurve();
    }
    public void ReverseScale()
    {
        scale.ReverseCurve();
    }
    public void ReverseColor()
    {
        color.ReverseCurve();
    }
    public void ReverseSequence()
    {
        sequence.ReverseCurve();
    }
    public void ReverseActions()
    {
        actions.ReverseCurve();
    }
    public void ReverseMaterial()
    {
        material.ReverseCurve();
    }
}
