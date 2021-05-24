using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UI;

public class AnimatedColorEditor : AnimatedBehaviourEditor
{
    SerializedProperty from_color;
    SerializedProperty to_color;
    SerializedProperty colorAnimationType;
    SerializedProperty coloredImage;
    SerializedProperty coloredEffect;
    SerializedProperty coloredCanvasGroup;
    SerializedProperty material;

    public AnimatedColorEditor(AnimatedElementExEditor _ae) : base(_ae, "color")
    {
    }

    public override void Init()
    {
        base.Init();

        to_color = sp.FindPropertyRelative("to_color");
        from_color = sp.FindPropertyRelative("from_color");
        colorAnimationType = sp.FindPropertyRelative("colorAnimationType");
        coloredImage = sp.FindPropertyRelative("coloredImage");
        coloredEffect = sp.FindPropertyRelative("coloredEffect");
        coloredCanvasGroup = sp.FindPropertyRelative("coloredCanvasGroup");
        material = sp.FindPropertyRelative("material");
    }

    public override void RevertStates()
    {
        for (int i = 0; i < ae_editor.targets.Length; i++)
        {
            AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;

            Color v1 = ae_ex.color.from_color;
            ae_ex.color.from_color = ae_ex.color.to_color;
            ae_ex.color.to_color = v1;
        }
    }

    protected override void DrawBefore()
    {
        EditorGUILayout.BeginVertical("box");
        if (!colorAnimationType.hasMultipleDifferentValues)
        {
            int anim_type = 0;
            EditorGUILayout.BeginHorizontal();
            bool clicked = false;
            if (LeftButton("CanvasGroup", Exists(AnimatedColor.ColorAnimation.CanvasGroup)))
            {
                if(!Exists(AnimatedColor.ColorAnimation.CanvasGroup))
                    anim_type = (int)AnimatedColor.ColorAnimation.CanvasGroup;
                if (Exists(AnimatedColor.ColorAnimation.Image))
                    anim_type |= (int)AnimatedColor.ColorAnimation.Image;
                if (Exists(AnimatedColor.ColorAnimation.Effect))
                    anim_type |= (int)AnimatedColor.ColorAnimation.Effect;
                clicked = true;
            }
            if (MidButton("Image", Exists(AnimatedColor.ColorAnimation.Image)))
            {
                if (Exists(AnimatedColor.ColorAnimation.CanvasGroup))
                    anim_type = (int)AnimatedColor.ColorAnimation.CanvasGroup;
                if (!Exists(AnimatedColor.ColorAnimation.Image))
                    anim_type |= (int)AnimatedColor.ColorAnimation.Image;
                if (Exists(AnimatedColor.ColorAnimation.Effect))
                    anim_type |= (int)AnimatedColor.ColorAnimation.Effect;
                clicked = true;
            }
            if (RightButton("Effect", Exists(AnimatedColor.ColorAnimation.Effect)))
            {
                if (Exists(AnimatedColor.ColorAnimation.CanvasGroup))
                    anim_type = (int)AnimatedColor.ColorAnimation.CanvasGroup;
                if (Exists(AnimatedColor.ColorAnimation.Image))
                    anim_type |= (int)AnimatedColor.ColorAnimation.Image;
                if (!Exists(AnimatedColor.ColorAnimation.Effect))
                    anim_type |= (int)AnimatedColor.ColorAnimation.Effect;
                clicked = true;
            }
            if (clicked)
                Set(anim_type);
            EditorGUILayout.EndHorizontal();

            //var val = EditorGUILayout.MaskField("Animation types", colorAnimationType.intValue, colorAnimationType.enumNames);
            //if(val != colorAnimationType.intValue)
            //    colorAnimationType.intValue = val;

            if (colorAnimationType.intValue != 0)
            {
                EditorGUILayout.BeginHorizontal();
                bool smv = EditorGUI.showMixedValue;
                if (Exists(AnimatedColor.ColorAnimation.CanvasGroup) && (!Exists(AnimatedColor.ColorAnimation.Image) || !Exists(AnimatedColor.ColorAnimation.Effect)))
                {
                    EditorGUI.showMixedValue = from_color.hasMultipleDifferentValues;
                    var ae = ae_editor.target as AnimatedElementEx;
                    ae.color.from_alpha = EditorGUILayout.Slider("From alpha", ae.color.from_alpha, 0f, 1f);
                    EditorGUI.showMixedValue = smv;
                }
                else EditorGUILayout.PropertyField(from_color, new GUIContent("From"));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (Exists(AnimatedColor.ColorAnimation.CanvasGroup) && (!Exists(AnimatedColor.ColorAnimation.Image) || !Exists(AnimatedColor.ColorAnimation.Effect)))
                {
                    EditorGUI.showMixedValue = to_color.hasMultipleDifferentValues;
                    var ae = ae_editor.target as AnimatedElementEx;
                    ae.color.to_alpha = EditorGUILayout.Slider("To alpha", ae.color.to_alpha, 0f, 1f);
                    EditorGUI.showMixedValue = smv;
                }
                else EditorGUILayout.PropertyField(to_color, new GUIContent("To"));
                EditorGUILayout.EndHorizontal();
            }

            if (Exists(AnimatedColor.ColorAnimation.Image))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(coloredImage, new GUIContent("Image"));
                ResetToCurrent(0, OnResetToCurrent);
                EditorGUILayout.EndHorizontal();
            }
            if (Exists(AnimatedColor.ColorAnimation.Effect))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(coloredEffect, new GUIContent("Effect"));
                ResetToCurrent(1, OnResetToCurrent);
                EditorGUILayout.EndHorizontal();
            }
            if (Exists(AnimatedColor.ColorAnimation.CanvasGroup))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(coloredCanvasGroup, new GUIContent("CanvasGroup"));
                ResetToCurrent(2, OnResetToCurrent);
                EditorGUILayout.EndHorizontal();
            }
            if (Exists(AnimatedColor.ColorAnimation.Material))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(material, new GUIContent("Material"));
                EditorGUILayout.EndHorizontal();
            }
        }
        else EditorGUILayout.HelpBox("'Animation types'can edit only one selected component", MessageType.Warning);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }

    void OnResetToCurrent(int componentNum, int i)
    {
        AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
        if (componentNum == 0) // Image
        {
            if (ae_ex.color.mainTransform != null)
            {
                Graphic img = ae_ex.color.mainTransform.GetComponent<Graphic>();
                ae_ex.color.coloredImage = img;
            }
            else
            {
                if (ae_ex.commonTransform != null)
                {
                    Graphic img = ae_ex.commonTransform.GetComponent<Graphic>();
                    ae_ex.color.coloredImage = img;
                }
                else
                {
                    Graphic img = ae_ex.GetComponent<Graphic>();
                    ae_ex.color.coloredImage = img;
                }
            }
        }
        else if (componentNum == 1) // Effect
        {
            if (ae_ex.color.mainTransform != null)
            {
                Shadow img = ae_ex.color.mainTransform.GetComponent<Shadow>();
                ae_ex.color.coloredEffect = img;
            }
            else
            {
                if (ae_ex.commonTransform != null)
                {
                    Shadow img = ae_ex.commonTransform.GetComponent<Shadow>();
                    ae_ex.color.coloredEffect = img;
                }
                else
                {
                    Shadow img = ae_ex.GetComponent<Shadow>();
                    ae_ex.color.coloredEffect = img;
                }
            }
        }
        else if (componentNum == 2) // CanvasGroup
        {
            if (ae_ex.color.mainTransform != null)
            {
                CanvasGroup img = ae_ex.color.mainTransform.GetComponent<CanvasGroup>();
                ae_ex.color.coloredCanvasGroup = img;
            }
            else
            {
                if (ae_ex.commonTransform != null)
                {
                    CanvasGroup img = ae_ex.commonTransform.GetComponent<CanvasGroup>();
                    ae_ex.color.coloredCanvasGroup = img;
                }
                else
                {
                    CanvasGroup img = ae_ex.GetComponent<CanvasGroup>();
                    ae_ex.color.coloredCanvasGroup = img;
                }
            }
        }
    }

    bool Exists(AnimatedColor.ColorAnimation type)
    {
        return !colorAnimationType.hasMultipleDifferentValues && (colorAnimationType.intValue & (int)type) == (int)type;
    }
    void Set(int animationType)
    {
        colorAnimationType.intValue = animationType;
    }

    bool LeftButton(string title, bool used)
    {
        bool clicked = false;
        Rect rect = GUILayoutUtility.GetRect(30f, 25f);
        var style = EditorStyles.miniButtonLeft;
        var col = GUI.color;
        if (used)
            GUI.color = Color.yellow;
        GUI.BeginGroup(rect);
        if (GUI.Button(new Rect(0, 0, rect.width + style.border.right, rect.height), title, style)) 
            clicked = true;
        GUI.EndGroup();
        GUI.color = col;
        return clicked;
    }

    bool MidButton(string title, bool used)
    {
        bool clicked = false;
        Rect rect = GUILayoutUtility.GetRect(30f, 25f);
        var style = EditorStyles.miniButtonMid;
        var col = GUI.color;
        if (used)
            GUI.color = Color.yellow;
        GUI.BeginGroup(rect);
        if (GUI.Button(new Rect(-style.border.left, 0, rect.width + style.border.left + style.border.right, rect.height), title, style)) 
            clicked = true;
        GUI.EndGroup();
        GUI.color = col;
        return clicked;
    }

    bool RightButton(string title, bool used)
    {
        bool clicked = false;
        Rect rect = GUILayoutUtility.GetRect(30f, 25f);
        var style = EditorStyles.miniButtonRight;
        var col = GUI.color;
        if (used)
            GUI.color = Color.yellow;
        GUI.BeginGroup(rect);
        if (GUI.Button(new Rect(-style.border.left, 0, rect.width + style.border.left, rect.height), title, style))
            clicked = true;
        GUI.EndGroup();
        GUI.color = col;
        return clicked;
    }
}