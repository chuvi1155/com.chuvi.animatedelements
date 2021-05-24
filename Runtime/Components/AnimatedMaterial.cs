using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimatedMaterial : AnimatedBehaviour
{
    public enum PropertyAnimationType
    {
        PropertyFloat = 0,
        PropertyInt = 1,
        PropertyVector = 2,
        PropertyColor = 3
    }
    [System.Flags]
    public enum MaterialAnimationType
    {
        MaterialOffset = 1 << 0,
        MaterialTiling = 1 << 1,
    }
    public Material material;
    public string TexturePropertyName = "";
    public Vector2 FromOffsetValue;
    public Vector2 ToOffsetValue;
    public Vector2 FromTilingValue;
    public Vector2 ToTilingValue;
    public MaterialAnimationType matAnimType = MaterialAnimationType.MaterialOffset;
    [SerializeField]
    public MaterialAnimation[] materialAnimations;

    public AnimatedMaterial() : base()
    {
    }
    public AnimatedMaterial(Transform _mainTransform) : base(_mainTransform)
    {
    }
    public override void ResetInFromState()
    {
        if (!Used) return;
        t = 0;
        onEnded = false;
        InitTransform();
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
        throw new System.NotImplementedException();
    }
    protected override object GetTo()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnAction(float _time, float curveValue)
    {
		if (mainTransform == null) return;
		if(material == null)
			material = mainTransform.GetComponent<Renderer>().material;

		if (material == null) return;
		if (materialAnimations != null && materialAnimations.Length > 0)
            System.Array.ForEach(materialAnimations, (ma) => ma.Execute(material, curveValue));

        if (Exists(MaterialAnimationType.MaterialOffset))
        {
            Vector2 newOffsetValue = Vector2.Lerp(FromOffsetValue, ToOffsetValue, curveValue);
            if (string.IsNullOrEmpty(TexturePropertyName))
                material.mainTextureOffset = newOffsetValue;
            else material.SetTextureOffset(TexturePropertyName, newOffsetValue);
        }
        if (Exists(MaterialAnimationType.MaterialTiling))
        {
            Vector2 newTilingValue = Vector2.Lerp(FromTilingValue, ToTilingValue, curveValue);
            if (string.IsNullOrEmpty(TexturePropertyName))
                material.mainTextureScale = newTilingValue;
            else material.SetTextureScale(TexturePropertyName, newTilingValue);
        }
    }

    protected override void SetFrom(object from)
    {
        throw new System.NotImplementedException();
    }

    protected override void SetTo(object to)
    {
        throw new System.NotImplementedException();
    }
    public bool Exists(MaterialAnimationType type)
    {
        return (this.matAnimType & type) == type;
    }

    [System.Serializable]
    public class MaterialAnimation
    {
        public string PropertyName;
        public PropertyAnimationType type = PropertyAnimationType.PropertyFloat;
        public float FromValue;
        public float ToValue;
        public int FromValueInt;
        public int ToValueInt;
        public Vector4 FromValueVec;
        public Vector4 ToValueVec;
        public Color FromValueColor;
        public Color ToValueColor;

        public void Execute(Material mat, float time)
        {
            if (mat == null || string.IsNullOrEmpty(PropertyName) || !mat.HasProperty(PropertyName)) return;

            switch (type)
            {
                case PropertyAnimationType.PropertyFloat:
                    mat.SetFloat(PropertyName, Mathf.Lerp(FromValue, ToValue, time));
                    break;
                case PropertyAnimationType.PropertyInt:
                    mat.SetInt(PropertyName, (int)Mathf.Lerp(FromValueInt, ToValueInt, time));
                    break;
                case PropertyAnimationType.PropertyVector:
                    mat.SetVector(PropertyName, Vector4.Lerp(FromValueVec, ToValueVec, time));
                    break;
                case PropertyAnimationType.PropertyColor:
                    mat.SetColor(PropertyName, Vector4.Lerp(FromValueColor, ToValueColor, time));
                    break;
                default:
                    break;
            }
        }
    }
}
