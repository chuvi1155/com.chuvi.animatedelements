using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

[System.Serializable]
public class AnimatedColor : AnimatedBehaviour
{
    [System.Flags]
    public enum ColorAnimation
    {
        Image = 1 << 0,
        Effect = 1 << 1,
        CanvasGroup = 1 << 2,
        Material = 1 << 3,
    }
    public bool FromCurrentColor = false;
    public Color from_color = Color.white;
    public Color to_color = Color.white;
    public float from_alpha
    {
        get { return from_color.a; }
        set { from_color.a = value; }
    }
    public float to_alpha
    {
        get { return to_color.a; }
        set { to_color.a = value; }
    }
    public ColorAnimation colorAnimationType = ColorAnimation.Image;
    public Graphic coloredImage;
    public Shadow coloredEffect;
    public CanvasGroup coloredCanvasGroup;
    public Material material;

    public AnimatedColor() : base()
    {
    }
    public AnimatedColor(Transform _mainTransform) : base(_mainTransform)
    {
    }

    public override void ResetInFromState()
    {
        if (!Used) return;
        t = 0;
        onEnded = false;
        InitTransform(); 
        if(FromCurrentColor)
        {
            if (Exists(ColorAnimation.Image) && coloredImage == null)
            {
                coloredImage = mainTransform.GetComponent<Graphic>();
                if (coloredImage != null)
                    from_color = coloredImage.color;
            }
            if (Exists(ColorAnimation.Effect) && coloredEffect == null)
            {
                coloredEffect = mainTransform.GetComponent<Shadow>();
                if (coloredEffect != null)
                    from_color = coloredEffect.effectColor;
            }
            if (Exists(ColorAnimation.CanvasGroup) && coloredCanvasGroup == null)
            {
                coloredCanvasGroup = mainTransform.GetComponent<CanvasGroup>();
                if (coloredCanvasGroup != null)
                    from_color.a = coloredCanvasGroup.alpha;
            }
        }
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

    protected override object GetFrom()
    {
        return from_color;
    }

    protected override object GetTo()
    {
        return to_color;
    }

    protected override void OnAction(float _time, float curveValue)
    {
        if (Exists(ColorAnimation.Image) && coloredImage == null)
            coloredImage = mainTransform.GetComponent<Graphic>();
        if (Exists(ColorAnimation.Effect) && coloredEffect == null)
            coloredEffect = mainTransform.GetComponent<Shadow>();
        if (Exists(ColorAnimation.CanvasGroup) && coloredCanvasGroup == null)
            coloredCanvasGroup = mainTransform.GetComponent<CanvasGroup>();
        if (coloredImage == null && coloredEffect == null && coloredCanvasGroup == null && material == null) return;

        Color newCol = Color.Lerp(from_color, to_color, curveValue);
        if (Exists(ColorAnimation.Image) && coloredImage != null) coloredImage.color = newCol;
        if (Exists(ColorAnimation.Effect) && coloredEffect != null) coloredEffect.effectColor = newCol;
        if (Exists(ColorAnimation.CanvasGroup) && coloredCanvasGroup != null) coloredCanvasGroup.alpha = newCol.a;
        if (Exists(ColorAnimation.Material) && material != null) material.color = newCol;
    }

    protected override void SetFrom(object from)
    {
        if (from is Color || from is Color32)
            from_color = (Color)from;
    }

    protected override void SetTo(object to)
    {
        if (to is Color || to is Color32)
            to_color = (Color)to;
    }

    public bool Exists(ColorAnimation type)
    {
        return (this.colorAnimationType & type) == type;
    }
}
