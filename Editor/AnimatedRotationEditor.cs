using UnityEngine;
using UnityEditor;
using System;

public class AnimatedRotationEditor : AnimatedBehaviourEditor
{
    SerializedProperty to_rotation;
    SerializedProperty from_rotation;

    public AnimatedRotationEditor(AnimatedElementExEditor _ae) : base(_ae, "rotation")
    {
    }

    public override void Init()
    {
        base.Init();

        to_rotation = sp.FindPropertyRelative("to_rotation");
        from_rotation = sp.FindPropertyRelative("from_rotation");
    }

    public override void RevertStates()
    {
        for (int i = 0; i < ae_editor.targets.Length; i++)
        {
            AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;

            Vector3 v1 = ae_ex.rotation.from_rotation;
            ae_ex.rotation.from_rotation = ae_ex.rotation.to_rotation;
            ae_ex.rotation.to_rotation = v1;
        }
    }

    protected override void DrawAfter()
    {
    }

    protected override void DrawBefore()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(from_rotation, new GUIContent("From"));
        ResetToZero(false, OnResetToZero);
        ResetToOne(false, OnResetToOne);
        ResetToCurrent(false, OnResetToCurrent);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(to_rotation, new GUIContent("To"));
        ResetToZero(true, OnResetToZero);
        ResetToOne(true, OnResetToOne);
        ResetToCurrent(true, OnResetToCurrent);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }

    void OnResetToZero(bool to, int i)
    {
        if (to)
        {
            AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
            ae_ex.rotation.to_rotation = Vector3.zero;
        }
        else
        {
            AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
            ae_ex.rotation.from_rotation = Vector3.zero;
        }
    }
    void OnResetToOne(bool to, int i)
    {
        if (to)
        {
            AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
            ae_ex.rotation.to_rotation = new Vector3(360, 360, 360);
        }
        else
        {
            AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
            ae_ex.rotation.from_rotation = new Vector3(360, 360, 360);
        }
    }
    void OnResetToCurrent(bool to, int i)
    {
        AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
        if (to)
        {
            if (ae_ex.rotation.mainTransform != null)
            {
                ae_ex.rotation.to_rotation = ae_ex.rotation.mainTransform.localEulerAngles;
            }
            else
            {
                if (ae_ex.commonTransform != null)
                    ae_ex.rotation.to_rotation = ae_ex.commonTransform.localEulerAngles;
                else ae_ex.rotation.to_rotation = ae_ex.transform.localEulerAngles;
            }
        }
        else
        {
            if (ae_ex.rotation.mainTransform != null)
            {
                ae_ex.rotation.from_rotation = ae_ex.rotation.mainTransform.localEulerAngles;
            }
            else
            {
                if (ae_ex.commonTransform != null)
                    ae_ex.rotation.from_rotation = ae_ex.commonTransform.localEulerAngles;
                else ae_ex.rotation.from_rotation = ae_ex.transform.localEulerAngles;
            }
        }
    }
}