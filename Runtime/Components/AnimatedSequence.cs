using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

[System.Serializable]
public class AnimatedSequence : AnimatedBehaviour
{
    [System.Flags]
    public enum SequenceTypes
    {
        Image = 1 << 0,
        Sprite = 1 << 1,
    }
    public SequenceTypes types = SequenceTypes.Image;
    public Image image;
    public SpriteRenderer sprite;
    public Sprite[] frameAnimation;

    public AnimatedSequence() : base()
    {
    }
    public AnimatedSequence(Transform _mainTransform) : base(_mainTransform)
    {
    }

    public override void ResetInFromState()
    {
        if (!Used) return;
        t = 0;
        onEnded = false;
        InitTransform();
        if (image != null) image.sprite = frameAnimation[0];
        if (sprite != null) sprite.sprite = frameAnimation[0];
    }

    public override void ResetInToState()
    {
        if (!Used) return;
        t = 0;
        onEnded = false;
        InitTransform();
        if (image != null) image.sprite = frameAnimation[frameAnimation.Length - 1];
        if (sprite != null) sprite.sprite = frameAnimation[frameAnimation.Length - 1];
    }

    protected override object GetFrom()
    {
        return frameAnimation[0];
    }

    protected override object GetTo()
    {
        return frameAnimation[frameAnimation.Length - 1];
    }

    protected override void OnAction(float _time, float curveValue)
    {
        if (Exists(SequenceTypes.Image) && image == null) image = mainTransform.GetComponent<Image>();
        if (Exists(SequenceTypes.Sprite) && sprite == null) sprite = mainTransform.GetComponent<SpriteRenderer>();
        if (image == null && sprite == null) return;
        if (frameAnimation == null || frameAnimation.Length == 0) return;

        int n = (int)(_time * (frameAnimation.Length));
        n = Mathf.Clamp(n, 0, frameAnimation.Length - 1);
        if (Exists(SequenceTypes.Image) && image != null) image.sprite = frameAnimation[n];
        if (Exists(SequenceTypes.Sprite) && sprite != null) sprite.sprite = frameAnimation[n];
    }

    protected override void SetFrom(object from)
    {
        throw new NotImplementedException();
    }

    protected override void SetTo(object to)
    {
        throw new NotImplementedException();
    }

    public bool Exists(SequenceTypes type)
    {
        return (types & type) == type;
    }
}