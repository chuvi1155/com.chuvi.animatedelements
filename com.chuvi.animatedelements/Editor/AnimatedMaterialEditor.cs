using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AnimatedMaterialEditor : AnimatedBehaviourEditor
{
    SerializedProperty material;
    SerializedProperty matAnimType;
    SerializedProperty TexturePropertyName;
    SerializedProperty FromOffsetValue;
    SerializedProperty ToOffsetValue;
    SerializedProperty FromTilingValue;
    SerializedProperty ToTilingValue;
    SerializedProperty materialAnimations;
    List<int> openedIndexes = new List<int>();
    protected override bool CanRevert { get { return false; } }

    public AnimatedMaterialEditor(AnimatedElementExEditor _ae) : base(_ae, "material")
    {
    }

    public override void Init()
    {
        base.Init();

        material = sp.FindPropertyRelative("material");
        matAnimType = sp.FindPropertyRelative("matAnimType");
        TexturePropertyName = sp.FindPropertyRelative("TexturePropertyName");
        FromOffsetValue = sp.FindPropertyRelative("FromOffsetValue");
        ToOffsetValue = sp.FindPropertyRelative("ToOffsetValue");
        FromTilingValue = sp.FindPropertyRelative("FromTilingValue");
        ToTilingValue = sp.FindPropertyRelative("ToTilingValue");
        materialAnimations = sp.FindPropertyRelative("materialAnimations");
    }

    protected override void DrawBefore()
    {
        EditorGUILayout.BeginVertical("box");

        if (!matAnimType.hasMultipleDifferentValues)
        {
            var val = EditorGUILayout.MaskField("Animation types", matAnimType.intValue, matAnimType.enumNames);
            if (val != matAnimType.intValue)
                matAnimType.intValue = val;
            if (matAnimType.intValue != 0)
            {
                EditorGUILayout.BeginHorizontal();
                if (Exists(AnimatedMaterial.MaterialAnimationType.MaterialOffset))
                {
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(FromOffsetValue, new GUIContent("FromOffset"));
                    ResetToCurrent(0, OnResetToCurrent);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(ToOffsetValue, new GUIContent("ToOffset"));
                    ResetToCurrent(1, OnResetToCurrent);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (Exists(AnimatedMaterial.MaterialAnimationType.MaterialTiling))
                {
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(FromTilingValue, new GUIContent("FromTiling"));
                    ResetToCurrent(2, OnResetToCurrent);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(ToTilingValue, new GUIContent("ToTiling"));
                    ResetToCurrent(3, OnResetToCurrent);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.PropertyField(material, new GUIContent("Material"));
        EditorGUILayout.PropertyField(TexturePropertyName, new GUIContent("TexturePropertyName", "Texture property name"));
        EditorGUILayout.BeginVertical("box");
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel++;
        if (EditorGUILayout.PropertyField(materialAnimations, new GUIContent("MaterialAnimations", "Material Animations")))
        {
            EditorGUI.indentLevel++;
            materialAnimations.arraySize = EditorGUILayout.IntField("Size", materialAnimations.arraySize);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            if (materialAnimations.arraySize > 0)
            {
                for (int i = 0; i < materialAnimations.arraySize; i++)
                {
                    var ma = materialAnimations.GetArrayElementAtIndex(i);
                    DrawMainProperties(ma, i);
                }
            }
        }
        EditorGUI.indentLevel = indent;
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }

    void OnResetToCurrent(int componentNum, int i)
    {
        AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
        if (componentNum == 0) // Offset from
        {
            if(string.IsNullOrEmpty(ae_ex.material.TexturePropertyName))
                ae_ex.material.FromOffsetValue = ae_ex.material.material.mainTextureOffset;
            else ae_ex.material.FromOffsetValue = ae_ex.material.material.GetTextureOffset(ae_ex.material.TexturePropertyName);
        }
        else if (componentNum == 1) // Offset to
        {
            if (string.IsNullOrEmpty(ae_ex.material.TexturePropertyName))
                ae_ex.material.ToOffsetValue = ae_ex.material.material.mainTextureOffset;
            else ae_ex.material.ToOffsetValue = ae_ex.material.material.GetTextureOffset(ae_ex.material.TexturePropertyName);
        }
        else if (componentNum == 2) // Tiling from
        {
            if (string.IsNullOrEmpty(ae_ex.material.TexturePropertyName))
                ae_ex.material.FromTilingValue = ae_ex.material.material.mainTextureScale;
            else ae_ex.material.FromTilingValue = ae_ex.material.material.GetTextureScale(ae_ex.material.TexturePropertyName);
        }
        else if (componentNum == 3) // Tiling to
        {
            if (string.IsNullOrEmpty(ae_ex.material.TexturePropertyName))
                ae_ex.material.ToTilingValue = ae_ex.material.material.mainTextureScale;
            else ae_ex.material.ToTilingValue = ae_ex.material.material.GetTextureScale(ae_ex.material.TexturePropertyName);
        }
    }

    private void DrawMainProperties(SerializedProperty matAnim, int index)
    {
        EditorGUI.indentLevel++;
        bool _opened = openedIndexes.Contains(index);
        bool opened = EditorGUILayout.Foldout(_opened, "Element " + index);
        if (opened)
        {
            if (opened != _opened)
                openedIndexes.Add(index);
            EditorGUILayout.BeginVertical("box");
            DrawProperty(matAnim.FindPropertyRelative("PropertyName"), "PropertyName");
            SerializedProperty type = matAnim.FindPropertyRelative("type");
            DrawProperty(type, "Type");
            if (Exists(type, AnimatedMaterial.PropertyAnimationType.PropertyFloat))
            {
                DrawProperty(matAnim.FindPropertyRelative("FromValue"), "From (float)");
                DrawProperty(matAnim.FindPropertyRelative("ToValue"), "To (float)");
            }
            else if (Exists(type, AnimatedMaterial.PropertyAnimationType.PropertyInt))
            {
                DrawProperty(matAnim.FindPropertyRelative("FromValueInt"), "From (int)");
                DrawProperty(matAnim.FindPropertyRelative("ToValueInt"), "To (int)");
            }
            else if (Exists(type, AnimatedMaterial.PropertyAnimationType.PropertyVector))
            {
                DrawProperty(matAnim.FindPropertyRelative("FromValueVec"), "From (Vector)");
                DrawProperty(matAnim.FindPropertyRelative("ToValueVec"), "To (Vector)");
            }
            else if (Exists(type, AnimatedMaterial.PropertyAnimationType.PropertyColor))
            {
                DrawProperty(matAnim.FindPropertyRelative("FromValueColor"), "From (Color)");
                DrawProperty(matAnim.FindPropertyRelative("ToValueColor"), "To (Color)");
            }
            EditorGUILayout.EndVertical();
        }
        else
        {
            if (opened != _opened)
                openedIndexes.Remove(index);
        }
        EditorGUI.indentLevel--;
    }

    public override void RevertStates()
    {
    }

    private void DrawProperty(SerializedProperty property, string name)
    {
        EditorGUILayout.PropertyField(property, new GUIContent(name));
    }

    bool Exists(SerializedProperty type, AnimatedMaterial.PropertyAnimationType proptype)
    {
        return !type.hasMultipleDifferentValues && type.intValue == (int)proptype;
    }

    bool Exists(AnimatedMaterial.MaterialAnimationType type)
    {
        return !matAnimType.hasMultipleDifferentValues && (matAnimType.intValue & (int)type) == (int)type;
    }
}