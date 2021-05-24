using UnityEngine;
using System.Collections;

public class EnumFlagsAttribute : PropertyAttribute
{
    public EnumFlagsAttribute() { }
}

#if UNITY_EDITOR
[UnityEditor.CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
public class EnumFlagsAttributeDrawer : UnityEditor.PropertyDrawer
{
    public override void OnGUI(Rect _position, UnityEditor.SerializedProperty _property, GUIContent _label)
    {
        _property.intValue = UnityEditor.EditorGUI.MaskField(_position, _label, _property.intValue, _property.enumNames);
    }
} 
#endif
