using UnityEngine;
using UnityEditor;
using System;

public class AnimatedScaleEditor : AnimatedBehaviourEditor
{
    SerializedProperty UseSize;
    SerializedProperty to_scale;
    SerializedProperty from_scale;
    SerializedProperty to_size;
    SerializedProperty from_size;
    bool canEditSize = true;

    public AnimatedScaleEditor(AnimatedElementExEditor _ae) : base(_ae, "scale")
    {
    }

    public override void Init()
    {
        base.Init();

        UseSize = sp.FindPropertyRelative("UseSize");
        to_scale = sp.FindPropertyRelative("to_scale");
        from_scale = sp.FindPropertyRelative("from_scale");
        to_size = sp.FindPropertyRelative("to_size");
        from_size = sp.FindPropertyRelative("from_size");
    }

    protected override void DrawBefore()
    {
        EditorGUILayout.BeginVertical("box");

        bool en = GUI.enabled;
        canEditSize = true;

        if (!UseSize.hasMultipleDifferentValues)
        {
            AnimatedElementEx ae_ex = ae_editor.target as AnimatedElementEx;
            if ((ae_ex.Scale.mainTransform == null && ae_ex.commonTransform != null && ae_ex.commonTransform.GetComponent<RectTransform>() != null) ||
                (ae_ex.Scale.mainTransform != null && ae_ex.Scale.rtr != null) ||
                (ae_ex.Scale.mainTransform == null && ae_ex.commonTransform == null && ae_ex.transform.GetComponent<RectTransform>() != null))
                EditorGUILayout.PropertyField(UseSize, new GUIContent("UseSize"));
            else
            {
                UseSize.boolValue = false;
                canEditSize = false;
            }
        }
        else
        {
            canEditSize = false;
            EditorGUILayout.HelpBox("Set field 'UseSize' can edit only one selected object", MessageType.Warning);
        }

        EditorGUILayout.BeginHorizontal();
        if (canEditSize && UseSize.boolValue)
            EditorGUILayout.PropertyField(from_size, new GUIContent("From"));
        else EditorGUILayout.PropertyField(from_scale, new GUIContent("From"));
        ResetToZero(false, OnResetToZero);
        ResetToOne(false, OnResetToOne);
        //GUI.enabled = en && canEditSize;
        ResetToCurrent(false, OnResetToCurrent);
        GUI.enabled = en;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (canEditSize && UseSize.boolValue)
            EditorGUILayout.PropertyField(to_size, new GUIContent("To"));
        else EditorGUILayout.PropertyField(to_scale, new GUIContent("To"));
        ResetToZero(true, OnResetToZero);
        ResetToOne(true, OnResetToOne);
       // GUI.enabled = en && canEditSize;
        ResetToCurrent(true, OnResetToCurrent);
        GUI.enabled = en;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }
    public override void RevertStates()
    {
        for (int i = 0; i < ae_editor.targets.Length; i++)
        {
            AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;

            Vector3 v1;
            if (ae_ex.Scale.UseSize)
            {
                v1 = ae_ex.Scale.from_size;
                ae_ex.Scale.from_size = ae_ex.Scale.to_size;
                ae_ex.Scale.to_size = v1;
            }
            else
            {
                v1 = ae_ex.Scale.from_scale;
                ae_ex.Scale.from_scale = ae_ex.Scale.to_scale;
                ae_ex.Scale.to_scale = v1;
            }
        }
    }

    void OnResetToZero(bool to, int i)
    {
        AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
        if (to)
        {
            if(ae_ex.Scale.UseSize)
                ae_ex.Scale.to_size = Vector3.zero;
            else ae_ex.Scale.to_scale = Vector3.zero;
        }
        else
        {
            if (ae_ex.Scale.UseSize)
                ae_ex.Scale.from_size = Vector3.zero;
            else ae_ex.Scale.from_scale = Vector3.zero;
        }
    }
    void OnResetToOne(bool to, int i)
    {
        AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
        if (to)
        {
            if (ae_ex.Scale.UseSize)
                ae_ex.Scale.to_size = Vector3.one;
            else ae_ex.Scale.to_scale = Vector3.one;
        }
        else
        {
            if (ae_ex.Scale.UseSize)
                ae_ex.Scale.from_size = Vector3.one;
            else ae_ex.Scale.from_scale = Vector3.one;
        }
    }
    void OnResetToCurrent(bool to, int i)
    {
        AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
        if (to)
        {
            if (ae_ex.Scale.mainTransform != null)
            {
                if (ae_ex.Scale.UseSize) ae_ex.Scale.to_size = ae_ex.Scale.rtr.sizeDelta;
                else ae_ex.Scale.to_scale = ae_ex.Scale.mainTransform.localScale;
            }
            else
            {
                if (ae_ex.commonTransform != null)
                {
                    if (ae_ex.Scale.UseSize)
                    {
                        RectTransform rtr = ae_ex.commonTransform.GetComponent<RectTransform>();
                        ae_ex.Scale.to_size = rtr.sizeDelta;
                    }
                    else
                        ae_ex.Scale.to_scale = ae_ex.commonTransform.localScale;
                }
                else
                {
                    if (ae_ex.Scale.UseSize)
                    {
                        RectTransform rtr = ae_ex.transform.GetComponent<RectTransform>();
                        ae_ex.Scale.to_size = rtr.sizeDelta;
                    }
                    else
                        ae_ex.Scale.to_scale = ae_ex.transform.localScale;
                }
            }
        }
        else
        {
            if (ae_ex.Scale.mainTransform != null)
            {
                if (ae_ex.Scale.UseSize) ae_ex.Scale.from_size = ae_ex.Scale.rtr.sizeDelta;
                else ae_ex.Scale.from_scale = ae_ex.Scale.mainTransform.localScale;
            }
            else
            {
                if (ae_ex.commonTransform != null)
                {
                    if (ae_ex.Scale.UseSize)
                    {
                        RectTransform rtr = ae_ex.commonTransform.GetComponent<RectTransform>();
                        ae_ex.Scale.from_size = rtr.sizeDelta;
                    }
                    else
                        ae_ex.Scale.from_scale = ae_ex.commonTransform.localScale;
                }
                else
                {
                    if (ae_ex.Scale.UseSize)
                    {
                        RectTransform rtr = ae_ex.transform.GetComponent<RectTransform>();
                        ae_ex.Scale.from_size = rtr.sizeDelta;
                    }
                    else
                        ae_ex.Scale.from_scale = ae_ex.transform.localScale;
                }
            }
        }
    }
}