using UnityEngine;
using UnityEditor;
using System;

public class AnimatedActionsEditor : AnimatedBehaviourEditor
{
    SerializedProperty act_types;

    SerializedProperty ClampedAction;
    SerializedProperty ToStringClampedAction;
    SerializedProperty InvertClampedAction;
    SerializedProperty ToStringInvertClampedAction;
    SerializedProperty CustomValueAction;
    SerializedProperty ToStringCustomValueAction;
    SerializedProperty CurvedValueAction;
    SerializedProperty ToStringCurvedValueAction;
    SerializedProperty CustomRangeValueAction;
    SerializedProperty ToStringCustomRangeValueAction;

    SerializedProperty MaxCustomValue;
    SerializedProperty CustomActionReversed;
    SerializedProperty FromCustomRangeValue;
    SerializedProperty ToCustomRangeValue;
    //SerializedProperty ToStringValueAction;
    SerializedProperty ToStringEvent;
    SerializedProperty AsIntValue;

    public AnimatedActionsEditor(AnimatedElementExEditor _ae) : base(_ae, "actions")
    {
    }

    public override void Init()
    {
        base.Init();

        act_types = sp.FindPropertyRelative("types");
        ClampedAction = sp.FindPropertyRelative("ClampedAction");
        InvertClampedAction = sp.FindPropertyRelative("InvertClampedAction");
        CustomValueAction = sp.FindPropertyRelative("CustomValueAction");
        CurvedValueAction = sp.FindPropertyRelative("CurvedValueAction");
        CustomRangeValueAction = sp.FindPropertyRelative("CustomRangeValueAction");

        ToStringClampedAction = sp.FindPropertyRelative("ToStringClampedAction");
        ToStringInvertClampedAction = sp.FindPropertyRelative("ToStringInvertClampedAction");
        ToStringCustomValueAction = sp.FindPropertyRelative("ToStringCustomValueAction");
        ToStringCurvedValueAction = sp.FindPropertyRelative("ToStringCurvedValueAction");
        ToStringCustomRangeValueAction = sp.FindPropertyRelative("ToStringCustomRangeValueAction");

        MaxCustomValue = sp.FindPropertyRelative("MaxCustomValue");
        CustomActionReversed = sp.FindPropertyRelative("CustomActionReversed");
        FromCustomRangeValue = sp.FindPropertyRelative("FromCustomRangeValue");
        ToCustomRangeValue = sp.FindPropertyRelative("ToCustomRangeValue");
        ToStringEvent = sp.FindPropertyRelative("ToStringEvent");
        AsIntValue = sp.FindPropertyRelative("AsIntValue");
    }
    public override void RevertStates()
    {
        for (int i = 0; i < ae_editor.targets.Length; i++)
        {
            AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;

            float v1 = ae_ex.Actions.FromCustomRangeValue;
            ae_ex.Actions.FromCustomRangeValue = ae_ex.Actions.ToCustomRangeValue;
            ae_ex.Actions.ToCustomRangeValue = v1;
        }
    }

    protected override void DrawBefore()
    {
        EditorGUILayout.BeginVertical("box");
        var val = EditorGUILayout.MaskField("Action types", act_types.intValue, act_types.enumNames);
        if (val != act_types.intValue)
            act_types.intValue = val;

        if (act_types.intValue != 0)
        {
            AnimatedElementEx ae = ae_editor.ae;
            GUIStyle myStyle = GUI.skin.GetStyle("HelpBox");
            myStyle.richText = true;

            EditorGUILayout.PropertyField(ToStringEvent, new GUIContent("ToStringEvent?"));

            if (Exists(AnimatedAction.ActionTypes.ClampedAction))
            {
                //EditorGUILayout.HelpBox("The event is triggered every update and returns a value from 0 to 1, where 1 - is the normalized '<b>AnimationTime</b>' field.", MessageType.Info);

                ProgressBar(ae.Actions.AnimatedTimeTick, ae.Actions.AnimationTime);

                EditorGUILayout.PropertyField(ClampedAction, new GUIContent("Clamped action"));
                if (ToStringEvent.boolValue)
                    EditorGUILayout.PropertyField(ToStringClampedAction, new GUIContent("ToString"));
            }
            if (Exists(AnimatedAction.ActionTypes.InvertClampedAction))
            {
                if (Exists(AnimatedAction.ActionTypes.ClampedAction))
                    EditorGUILayout.Space();
                //EditorGUILayout.HelpBox("The event is triggered every update and returns a inverted value from 1 to 0, where 0 - is the normalized '<b>AnimationTime</b>' field.", MessageType.Info);

                ProgressBar(ae.Actions.AnimationTime - ae.Actions.AnimatedTimeTick, ae.Actions.AnimationTime);

                EditorGUILayout.PropertyField(InvertClampedAction, new GUIContent("Invert clamped value action"));
                if(ToStringEvent.boolValue)
                    EditorGUILayout.PropertyField(ToStringInvertClampedAction, new GUIContent("ToString"));
            }
            if (Exists(AnimatedAction.ActionTypes.CurvedValueAction))
            {
                EditorGUILayout.PropertyField(AsIntValue, new GUIContent("AsIntValue?"));

                if (Exists(AnimatedAction.ActionTypes.InvertClampedAction) || Exists(AnimatedAction.ActionTypes.ClampedAction))
                    EditorGUILayout.Space();
                //EditorGUILayout.HelpBox("The event is triggered every update and returns a value from '<b>Curve</b>' field", MessageType.Info);
                
                ProgressBar(ae.Actions.CurveValue, ae.Actions.GetMinCurveValue, ae.Actions.GetMaxCurveValue);

                EditorGUILayout.PropertyField(CurvedValueAction, new GUIContent("Curved value action"));
                if (ToStringEvent.boolValue)
                    EditorGUILayout.PropertyField(ToStringCurvedValueAction, new GUIContent("ToString"));
            }
            if (Exists(AnimatedAction.ActionTypes.CustomValueAction))
            {
				EditorGUILayout.PropertyField(AsIntValue, new GUIContent("AsIntValue?"));
                if(!IsOneSelected(AnimatedAction.ActionTypes.CustomValueAction))
                    EditorGUILayout.Space();
                //EditorGUILayout.HelpBox("The event is triggered every update and returns the value set in field '<b>Max value</b>'.", MessageType.Info);
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(MaxCustomValue, new GUIContent("Max value"));
                EditorGUILayout.PropertyField(CustomActionReversed, new GUIContent("Reversed value", "Return inverted value?"));
                
                ProgressBar(ae.Actions.AnimatedTimeTick, ae.Actions.MaxCustomValue);

                EditorGUILayout.PropertyField(CustomValueAction, new GUIContent("Custom value action"));
                if (ToStringEvent.boolValue)
                    EditorGUILayout.PropertyField(ToStringCustomValueAction, new GUIContent("ToString"));
                EditorGUILayout.EndVertical();
            }
            if (Exists(AnimatedAction.ActionTypes.CustomValueRangeAction))
            {
                EditorGUILayout.PropertyField(AsIntValue, new GUIContent("AsIntValue?"));
                if (!IsOneSelected(AnimatedAction.ActionTypes.CustomValueRangeAction))
                    EditorGUILayout.Space();
                //EditorGUILayout.HelpBox("The event is triggered every update and returns the value from range '<b>From value</b>' and '<b>To value</b>'.", MessageType.Info);
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(FromCustomRangeValue, new GUIContent("From value"));
                EditorGUILayout.PropertyField(ToCustomRangeValue, new GUIContent("To value"));
                
                ProgressBar(ae.Actions.AnimatedTimeTick, ae.Actions.FromCustomRangeValue, ae.Actions.ToCustomRangeValue);

                EditorGUILayout.PropertyField(CustomRangeValueAction, new GUIContent("Custom range value action"));
                if (ToStringEvent.boolValue)
                    EditorGUILayout.PropertyField(ToStringCustomRangeValueAction, new GUIContent("ToString"));
                EditorGUILayout.EndVertical();
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }

    bool Exists(AnimatedAction.ActionTypes type)
    {
        return !act_types.hasMultipleDifferentValues && (act_types.intValue & (int)type) == (int)type;
    }
    bool IsOneSelected(AnimatedAction.ActionTypes type)
    {
        return !act_types.hasMultipleDifferentValues && act_types.intValue == (int)type;
    }
}
