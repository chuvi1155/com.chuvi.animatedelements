using UnityEngine;
using UnityEditor;
using System;

public class AnimatedRotationEditor : AnimatedBehaviourEditor
{
    SerializedProperty to_rotation;
    SerializedProperty from_rotation;
    SerializedProperty from_current;
    SerializedProperty UseRandomPositions;

    public AnimatedRotationEditor(AnimatedElementExEditor _ae) : base(_ae, "rotation")
    {
    }

    public override void Init()
    {
        base.Init();

        to_rotation = sp.FindPropertyRelative("to_rotation");
        from_rotation = sp.FindPropertyRelative("from_rotation");
        UseRandomPositions = sp.FindPropertyRelative("UseRandomPositions");
        from_current = sp.FindPropertyRelative("FromCurrentPosition");
    }

    public override void RevertStates()
    {
        for (int i = 0; i < ae_editor.targets.Length; i++)
        {
            AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;

            Vector3 v1 = ae_ex.Rotation.from_rotation;
            ae_ex.Rotation.from_rotation = ae_ex.Rotation.to_rotation;
            ae_ex.Rotation.to_rotation = v1;
        }
    }

    protected override void DrawAfter()
    {
    }

    protected override void DrawBefore()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.PropertyField(from_current);
        EditorGUILayout.BeginHorizontal();
        bool en = GUI.enabled;
        GUI.enabled = from_current.enumValueIndex == 0;
        EditorGUILayout.PropertyField(from_rotation, new GUIContent("From"));
        GUI.enabled = en;
        //EditorGUILayout.PropertyField(from_rotation, new GUIContent("From"));
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
        EditorGUILayout.PropertyField(UseRandomPositions, new GUIContent("Use random positions"));
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }

    void OnResetToZero(bool to, int i)
    {
        if (to)
        {
            AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
            ae_ex.Rotation.to_rotation = Vector3.zero;
        }
        else
        {
            AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
            ae_ex.Rotation.from_rotation = Vector3.zero;
        }
    }
    void OnResetToOne(bool to, int i)
    {
        if (to)
        {
            AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
            ae_ex.Rotation.to_rotation = new Vector3(360, 360, 360);
        }
        else
        {
            AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
            ae_ex.Rotation.from_rotation = new Vector3(360, 360, 360);
        }
    }
    void OnResetToCurrent(bool to, int i)
    {
        AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
        if (to)
        {
            if (ae_ex.Rotation.mainTransform != null)
            {
                ae_ex.Rotation.to_rotation = ae_ex.Rotation.mainTransform.localEulerAngles;
            }
            else
            {
                if (ae_ex.commonTransform != null)
                    ae_ex.Rotation.to_rotation = ae_ex.commonTransform.localEulerAngles;
                else ae_ex.Rotation.to_rotation = ae_ex.transform.localEulerAngles;
            }
        }
        else
        {
            if (ae_ex.Rotation.mainTransform != null)
            {
                ae_ex.Rotation.from_rotation = ae_ex.Rotation.mainTransform.localEulerAngles;
            }
            else
            {
                if (ae_ex.commonTransform != null)
                    ae_ex.Rotation.from_rotation = ae_ex.commonTransform.localEulerAngles;
                else ae_ex.Rotation.from_rotation = ae_ex.transform.localEulerAngles;
            }
        }
    }
}
