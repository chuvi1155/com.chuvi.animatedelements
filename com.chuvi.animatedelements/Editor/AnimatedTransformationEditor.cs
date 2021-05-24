using UnityEngine;
using UnityEditor;
using System;

public class AnimatedTransformationEditor : AnimatedBehaviourEditor
{
    SerializedProperty to_transformPos;
    SerializedProperty from_transformPos;
    SerializedProperty from_current;
    SerializedProperty UseRandomPositions;
    public AnimatedTransformationEditor(AnimatedElementExEditor _ae) : base(_ae, "transformation")
    { }

    public override void Init()
    {
        base.Init();

        to_transformPos = sp.FindPropertyRelative("to_transformPos");
        from_transformPos = sp.FindPropertyRelative("from_transformPos");
        UseRandomPositions = sp.FindPropertyRelative("UseRandomPositions");
        from_current = sp.FindPropertyRelative("FromCurrentPosition");
    }
    protected override void DrawBefore()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.PropertyField(from_current);
        EditorGUILayout.BeginHorizontal();
        bool en = GUI.enabled;
        GUI.enabled = from_current.enumValueIndex == 0;
        EditorGUILayout.PropertyField(from_transformPos, new GUIContent("From"));
        GUI.enabled = en;
        ResetToZero(false, OnResetToZero);
        ResetToOne(false, OnResetToOne);
        ResetToCurrent(false, OnResetToCurrent);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(to_transformPos, new GUIContent("To"));
        ResetToZero(true, OnResetToZero);
        ResetToOne(true, OnResetToOne);
        ResetToCurrent(true, OnResetToCurrent);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(UseRandomPositions, new GUIContent("Use random positions"));
        //if(GUILayout.Button("UseRandom"))
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }

    void OnResetToZero(bool to, int i)
    {
        if (to)
        {
            AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
            ae_ex.transformation.to_transformPos = Vector3.zero;
        }
        else
        {
            AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
            ae_ex.transformation.from_transformPos = Vector3.zero;
        }
    }
    void OnResetToOne(bool to, int i)
    {
        if (to)
        {
            AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
            ae_ex.transformation.to_transformPos = Vector3.one;
        }
        else
        {
            AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
            ae_ex.transformation.from_transformPos = Vector3.one;
        }
    }
    void OnResetToCurrent(bool to, int i)
    {
        AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
        if (to)
        {
            if (ae_ex.transformation.mainTransform != null)
            {
                if (ae_ex.transformation.rtr != null)
                    ae_ex.transformation.to_transformPos = ae_ex.transformation.rtr.anchoredPosition;
                else ae_ex.transformation.to_transformPos = ae_ex.transformation.mainTransform.localPosition;
            }
            else 
            {
                if (ae_ex.commonTransform != null)
                {
                    RectTransform rtr = ae_ex.commonTransform.GetComponent<RectTransform>();
                    if (rtr != null)
                        ae_ex.transformation.to_transformPos = rtr.anchoredPosition;
                    else ae_ex.transformation.to_transformPos = ae_ex.commonTransform.localPosition;
                }
                else
                {
                    RectTransform rtr = ae_ex.GetComponent<RectTransform>();
                    if (rtr != null)
                        ae_ex.transformation.to_transformPos = rtr.anchoredPosition;
                    else ae_ex.transformation.to_transformPos = ae_ex.transform.localPosition;
                }
            }
        }
        else
        {
            if (ae_ex.transformation.mainTransform != null)
            {
                if (ae_ex.transformation.rtr != null)
                    ae_ex.transformation.from_transformPos = ae_ex.transformation.rtr.anchoredPosition;
                else ae_ex.transformation.from_transformPos = ae_ex.transformation.mainTransform.localPosition;
            }
            else
            {
                if (ae_ex.commonTransform != null)
                {
                    RectTransform rtr = ae_ex.commonTransform.GetComponent<RectTransform>();
                    if (rtr != null)
                        ae_ex.transformation.from_transformPos = rtr.anchoredPosition;
                    else ae_ex.transformation.from_transformPos = ae_ex.commonTransform.localPosition;
                }
                else
                {
                    RectTransform rtr = ae_ex.GetComponent<RectTransform>();
                    if (rtr != null)
                        ae_ex.transformation.from_transformPos = rtr.anchoredPosition;
                    else ae_ex.transformation.from_transformPos = ae_ex.transform.localPosition;
                }
            }
        }
    }

    public override void RevertStates()
    {
        for (int i = 0; i < ae_editor.targets.Length; i++)
        {
            AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;

            Vector3 v1 = ae_ex.transformation.from_transformPos;
            ae_ex.transformation.from_transformPos = ae_ex.transformation.to_transformPos;
            ae_ex.transformation.to_transformPos = v1;
        }
    }
    protected override void DrawAfter()
    {
        
    }
}