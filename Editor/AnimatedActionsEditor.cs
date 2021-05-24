using UnityEngine;
using UnityEditor;
using System;

public class AnimatedActionsEditor : AnimatedBehaviourEditor
{
    SerializedProperty act_types;
    SerializedProperty ClampedAction;
    SerializedProperty InvertClampedAction;
    SerializedProperty CustomValueAction;
    SerializedProperty CurvedValueAction;
    SerializedProperty MaxCustomValue;
    SerializedProperty CustomActionReversed;
    SerializedProperty CustomRangeValueAction;
    SerializedProperty FromCustomRangeValue;
    SerializedProperty ToCustomRangeValue;
    SerializedProperty ToStringValueAction;
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
        MaxCustomValue = sp.FindPropertyRelative("MaxCustomValue");
        CustomActionReversed = sp.FindPropertyRelative("CustomActionReversed");
        CustomRangeValueAction = sp.FindPropertyRelative("CustomRangeValueAction");
        FromCustomRangeValue = sp.FindPropertyRelative("FromCustomRangeValue");
        ToCustomRangeValue = sp.FindPropertyRelative("ToCustomRangeValue");
		ToStringValueAction = sp.FindPropertyRelative("ToStringValueAction");
        ToStringEvent = sp.FindPropertyRelative("ToStringEvent");
        AsIntValue = sp.FindPropertyRelative("AsIntValue");
    }
    public override void RevertStates()
    {
        for (int i = 0; i < ae_editor.targets.Length; i++)
        {
            AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;

            float v1 = ae_ex.actions.FromCustomRangeValue;
            ae_ex.actions.FromCustomRangeValue = ae_ex.actions.ToCustomRangeValue;
            ae_ex.actions.ToCustomRangeValue = v1;
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
                EditorGUILayout.HelpBox("The event is triggered every update and returns a value from 0 to 1, where 1 - is the normalized '<b>AnimationTime</b>' field.", MessageType.Info);

                ProgressBar(ae.actions.AnimatedTimeTick, ae.actions.AnimationTime);

                EditorGUILayout.PropertyField(ClampedAction, new GUIContent("Clamped action"));
                if (ToStringEvent.boolValue)
                    EditorGUILayout.PropertyField(ToStringValueAction, new GUIContent("ToStringValueAction"));
            }
            if (Exists(AnimatedAction.ActionTypes.InvertClampedAction))
            {
                if (Exists(AnimatedAction.ActionTypes.ClampedAction))
                    EditorGUILayout.Space();
                EditorGUILayout.HelpBox("The event is triggered every update and returns a inverted value from 1 to 0, where 0 - is the normalized '<b>AnimationTime</b>' field.", MessageType.Info);

                ProgressBar(ae.actions.AnimationTime - ae.actions.AnimatedTimeTick, ae.actions.AnimationTime);

                EditorGUILayout.PropertyField(InvertClampedAction, new GUIContent("Invert clamped value action"));
                if(ToStringEvent.boolValue)
                    EditorGUILayout.PropertyField(ToStringValueAction, new GUIContent("ToStringValueAction"));
            }
            if (Exists(AnimatedAction.ActionTypes.CurvedValueAction))
            {
                EditorGUILayout.PropertyField(AsIntValue, new GUIContent("AsIntValue?"));

                if (Exists(AnimatedAction.ActionTypes.InvertClampedAction) || Exists(AnimatedAction.ActionTypes.ClampedAction))
                    EditorGUILayout.Space();
                EditorGUILayout.HelpBox("The event is triggered every update and returns a value from '<b>Curve</b>' field", MessageType.Info);
                
                ProgressBar(ae.actions.CurveValue, ae.actions.GetMinCurveValue, ae.actions.GetMaxCurveValue);

                EditorGUILayout.PropertyField(CurvedValueAction, new GUIContent("Curved value action"));
                if (ToStringEvent.boolValue)
                    EditorGUILayout.PropertyField(ToStringValueAction, new GUIContent("ToStringValueAction"));
            }
            if (Exists(AnimatedAction.ActionTypes.CustomValueAction))
            {
				EditorGUILayout.PropertyField(AsIntValue, new GUIContent("AsIntValue?"));
                if(!IsOneSelected(AnimatedAction.ActionTypes.CustomValueAction))
                    EditorGUILayout.Space();
                EditorGUILayout.HelpBox("The event is triggered every update and returns the value set in field '<b>Max value</b>'.", MessageType.Info);
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(MaxCustomValue, new GUIContent("Max value"));
                EditorGUILayout.PropertyField(CustomActionReversed, new GUIContent("Reversed value", "Return inverted value?"));
                
                ProgressBar(ae.actions.AnimatedTimeTick, ae.actions.MaxCustomValue);

                EditorGUILayout.PropertyField(CustomValueAction, new GUIContent("Custom value action"));
                if (ToStringEvent.boolValue)
                    EditorGUILayout.PropertyField(ToStringValueAction, new GUIContent("ToStringValueAction"));
                EditorGUILayout.EndVertical();
            }
            if (Exists(AnimatedAction.ActionTypes.CustomValueRangeAction))
            {
                EditorGUILayout.PropertyField(AsIntValue, new GUIContent("AsIntValue?"));
                if (!IsOneSelected(AnimatedAction.ActionTypes.CustomValueRangeAction))
                    EditorGUILayout.Space();
                EditorGUILayout.HelpBox("The event is triggered every update and returns the value from range '<b>From value</b>' and '<b>To value</b>'.", MessageType.Info);
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(FromCustomRangeValue, new GUIContent("From value"));
                EditorGUILayout.PropertyField(ToCustomRangeValue, new GUIContent("To value"));
                
                ProgressBar(ae.actions.AnimatedTimeTick, ae.actions.FromCustomRangeValue, ae.actions.ToCustomRangeValue);

                EditorGUILayout.PropertyField(CustomRangeValueAction, new GUIContent("Custom range value action"));
                if (ToStringEvent.boolValue)
                    EditorGUILayout.PropertyField(ToStringValueAction, new GUIContent("ToStringValueAction"));
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