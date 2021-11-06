using UnityEngine;
using UnityEditor;

public abstract class AnimatedBehaviourEditor
{
    protected AnimatedElementExEditor ae_editor;
    protected SerializedProperty sp;
    protected SerializedProperty mainTransform;
    public SerializedProperty Used;
    protected SerializedProperty AnimationTime;
    protected SerializedProperty UseRandomTime;
    protected SerializedProperty MaxRandomTime;
    protected SerializedProperty IsRepeat;
    protected SerializedProperty TimeWaitRepeate;
    protected SerializedProperty UseRandomTimeWaitRepeate;
    protected SerializedProperty MaxRandomTimeWaitRepeate;
    protected SerializedProperty curve;
    protected SerializedProperty StartAnimation;
    protected SerializedProperty OnAnimation;
    protected SerializedProperty EndAnimation;

    static GUIContent mainTr_cont = new GUIContent("Main Transform");
    static GUIContent used_cont = new GUIContent("Used");
    static GUIContent animTime_cont = new GUIContent("AnimationTime");
    static GUIContent curve_cont = new GUIContent("Curve");
    static GUIContent isRepeate_cont = new GUIContent("Is repeat");
    static GUIContent timeWaitRepeate_cont = new GUIContent("Time wait repeate");
    static GUIContent useRandTimeWaitRepeate_cont = new GUIContent("Use random time wait repeate");
    static GUIContent maxRandTomeWaitRepeate_cont = new GUIContent("Max random time wait repeate");
    static GUIContent useRandTime_cont = new GUIContent("Use random time");
    static GUIContent maxRandTime_cont = new GUIContent("Max random time");
    static GUIContent startAnim_cont = new GUIContent("StartAnimation");
    static GUIContent onAnim_cont = new GUIContent("OnAnimation");
    static GUIContent endAnim_cont = new GUIContent("EndAnimation");
    static GUIContent rc_cont = new GUIContent("RC", "Reset to current");
    static GUIContent r1_cont = new GUIContent("R1", "Reset to one");
    static GUIContent r0_cont = new GUIContent("R0", "Reset to zero");
    static GUIContent commonSettings_cont = new GUIContent("Common settings");

    internal bool showCommonSettings = true;

    protected virtual bool CanRevert { get { return true; } }

    public AnimatedBehaviourEditor(AnimatedElementExEditor _ae, string propName)
    {
        ae_editor = _ae;
        sp = ae_editor.serializedObject.FindProperty(propName);
    }

    public virtual void Init()
    {
        mainTransform = sp.FindPropertyRelative("_mainTransform");
        Used = sp.FindPropertyRelative("Used");
        AnimationTime = sp.FindPropertyRelative("AnimationTime");
        UseRandomTime = sp.FindPropertyRelative("UseRandomTime");
        MaxRandomTime = sp.FindPropertyRelative("MaxRandomTime");
        IsRepeat = sp.FindPropertyRelative("IsRepeat");
        TimeWaitRepeate = sp.FindPropertyRelative("TimeWaitRepeate");
        UseRandomTimeWaitRepeate = sp.FindPropertyRelative("UseRandomTimeWaitRepeate");
        MaxRandomTimeWaitRepeate = sp.FindPropertyRelative("MaxRandomTimeWaitRepeate");
        curve = sp.FindPropertyRelative("curve");
        StartAnimation = sp.FindPropertyRelative("StartAnimation");
        OnAnimation = sp.FindPropertyRelative("OnAnimation");
        EndAnimation = sp.FindPropertyRelative("EndAnimation");
    }

    public bool? IsUsed()
    {
        if (Used.hasMultipleDifferentValues)
            return null;
        return Used.boolValue;
    }

    public bool? IsRepeate()
    {
        if (IsRepeat.hasMultipleDifferentValues)
            return null;
        return IsRepeat.boolValue;
    }

    public bool? IsRandomTimeWaitRepeate()
    {
        if(UseRandomTimeWaitRepeate.hasMultipleDifferentValues)
            return null;
        return UseRandomTimeWaitRepeate.boolValue;
    }

    public bool? IsRandom()
    {
        if (UseRandomTime.hasMultipleDifferentValues)
            return null;
        return UseRandomTime.boolValue; 
    }

    public string GetUsedText()
    {
        return Used.boolValue && !Used.hasMultipleDifferentValues ? " (Used)" :
            Used.hasMultipleDifferentValues ? " (Mixed)" : "";
    }

    public bool? IsWaiting()
    {
        if (ae_editor.targets.Length > 1) return null;
        AnimatedElementEx ae_ex = ae_editor.targets[0] as AnimatedElementEx;
        switch (sp.name)
        {
            case "transformation":
                return ae_ex.Transformation.IsWaiting;
            case "rotation":
                return ae_ex.Rotation.IsWaiting;
            case "scale":
                return ae_ex.Scale.IsWaiting;
            case "color":
                return ae_ex.Color.IsWaiting;
            case "sequence":
                return ae_ex.Sequence.IsWaiting;
            case "actions":
                return ae_ex.Actions.IsWaiting;
            case "material":
                return ae_ex.Material.IsWaiting;
            default:
                Debug.LogError("Unknown property name: " + sp.name);
                break;
        }
        return null;
    }

    public virtual void Draw()
    {
        EditorGUI.showMixedValue = Used.hasMultipleDifferentValues;
        bool val = EditorGUILayout.BeginToggleGroup(used_cont, Used.boolValue);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset", GUILayout.Width(50)))
        {
            ResetInFromState();
        }
        if (CanRevert)
        {
            if (GUILayout.Button("Revert", GUILayout.Width(50)))
            {
                RevertStates();
            } 
        }
        EditorGUILayout.EndHorizontal();
        if (GUI.changed)
            Used.boolValue = val;
        EditorGUI.showMixedValue = false;
        DrawBefore();
        if (showCommonSettings)
        {
            Color gui_col = GUI.color;
            GUI.color = new Color(0.6f, 0.6f, 0.7f, 1f);
            if (GUILayout.Button(commonSettings_cont, EditorStyles.toolbarButton))
                showCommonSettings = !showCommonSettings;
            GUI.color = gui_col;
            Color bg_col = GUI.backgroundColor;
            Color new_bg_col = new Color32(161, 178, 158, 255);//bg_col * 0.8f;
                                                               //new_bg_col.a = 1f;
            GUI.backgroundColor = new_bg_col;
            EditorGUILayout.BeginVertical("box");
            GUI.backgroundColor = bg_col;
            if (mainTransform != null)
            {
                EditorGUILayout.BeginHorizontal();
                Object old = mainTransform.objectReferenceValue;
                EditorGUILayout.PropertyField(mainTransform, mainTr_cont);
                if (old != mainTransform.objectReferenceValue)
                {
                    for (int i = 0; i < ae_editor.targets.Length; i++)
                    {
                        AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
                        if (sp.name == "transformation")
                            ae_ex.Transformation.mainTransform = mainTransform.objectReferenceValue as Transform;
                        else if (sp.name == "rotation")
                            ae_ex.Rotation.mainTransform = mainTransform.objectReferenceValue as Transform;
                        else if (sp.name == "scale")
                            ae_ex.Scale.mainTransform = mainTransform.objectReferenceValue as Transform;
                        else if (sp.name == "color")
                            ae_ex.Color.mainTransform = mainTransform.objectReferenceValue as Transform;
                        else if (sp.name == "sequence")
                            ae_ex.Sequence.mainTransform = mainTransform.objectReferenceValue as Transform;
                        else if (sp.name == "actions")
                            ae_ex.Actions.mainTransform = mainTransform.objectReferenceValue as Transform;
                        else if (sp.name == "material")
                            ae_ex.Material.mainTransform = mainTransform.objectReferenceValue as Transform;
                        else Debug.LogError("Unknown property name: " + sp.name);
                    }
                }
                ResetToCurrentMainTransform();
                EditorGUILayout.EndHorizontal();
            }
            if (AnimationTime != null) EditorGUILayout.PropertyField(AnimationTime, animTime_cont);
            if (curve != null) EditorGUILayout.PropertyField(curve, curve_cont);
            if (IsRepeat != null)
            {
                EditorGUILayout.PropertyField(IsRepeat, isRepeate_cont);
                if (!IsRepeat.hasMultipleDifferentValues && IsRepeat.boolValue)
                {
                    EditorGUI.indentLevel++;
                    if (TimeWaitRepeate != null) EditorGUILayout.PropertyField(TimeWaitRepeate, timeWaitRepeate_cont);
                    if (UseRandomTimeWaitRepeate != null)
                    {
                        EditorGUILayout.PropertyField(UseRandomTimeWaitRepeate, useRandTimeWaitRepeate_cont);
                        if (!UseRandomTimeWaitRepeate.hasMultipleDifferentValues && UseRandomTimeWaitRepeate.boolValue)
                        {
                            if (MaxRandomTimeWaitRepeate != null)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(MaxRandomTimeWaitRepeate, maxRandTomeWaitRepeate_cont);
                                EditorGUI.indentLevel--;
                            }
                        }
                    }
                    EditorGUI.indentLevel--;
                }
                if (UseRandomTime != null)
                {
                    EditorGUILayout.PropertyField(UseRandomTime, useRandTime_cont);
                    if (!UseRandomTime.hasMultipleDifferentValues && UseRandomTime.boolValue)
                    {
                        if (MaxRandomTime != null)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(MaxRandomTime, maxRandTime_cont);
                            EditorGUI.indentLevel--;
                        }
                    }
                }
            }
            if (StartAnimation != null) EditorGUILayout.PropertyField(StartAnimation, startAnim_cont);
            if (OnAnimation != null) EditorGUILayout.PropertyField(OnAnimation, onAnim_cont);
            if (EndAnimation != null)
            {
                EditorGUILayout.PropertyField(EndAnimation, endAnim_cont);
            }
            EditorGUILayout.EndVertical();
        }
        DrawAfter();
        if (!showCommonSettings)
        {
            Color gui_col = GUI.color;
            GUI.color = new Color(0.6f, 0.6f, 0.7f, 1f);
            if (GUILayout.Button(commonSettings_cont, EditorStyles.toolbarButton))
                showCommonSettings = !showCommonSettings;
            GUI.color = gui_col;
        }
        EditorGUILayout.EndToggleGroup();
    }

    protected virtual void DrawBefore() { }
    protected virtual void DrawAfter() { }

    protected delegate void OnResetToAction<T>(T to, int i);
    protected delegate void OnResetTransform(int i);

    protected void ProgressBar(float value, float maxValue)
    {
        if (Application.isPlaying && ae_editor.targets.Length == 1)
        {
            Rect rect = GUILayoutUtility.GetRect(18, 5, "TextField");
            if (maxValue != 0)
                EditorGUI.ProgressBar(rect, value / maxValue, "");
            else EditorGUI.ProgressBar(rect, 0, "");
        }
    }
    protected void ProgressBar(float value, float minValue, float maxValue)
    {
        if (Application.isPlaying && ae_editor.targets.Length == 1)
        {
            Rect rect = GUILayoutUtility.GetRect(18, 5, "TextField");
            float len = Mathf.Abs(maxValue - minValue);
            if (len != 0)
                EditorGUI.ProgressBar(rect, (value - minValue) / len, "");
            else EditorGUI.ProgressBar(rect, 0, "");
        }
    }

    public abstract void RevertStates();
    public void ResetInFromState()
    {
        //((AnimatedBehaviour)(object)sp.serializedObject.targetObject).ResetInFromState();
        for (int i = 0; i < ae_editor.targets.Length; i++)
        {
            AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
            if (sp.name == "transformation")
                ae_ex.Transformation.ResetInFromState();
            else if (sp.name == "rotation")
                ae_ex.Rotation.ResetInFromState();
            else if (sp.name == "scale")
                ae_ex.Scale.ResetInFromState();
            else if (sp.name == "color")
                ae_ex.Color.ResetInFromState();
            else if (sp.name == "sequence")
                ae_ex.Sequence.ResetInFromState();
            else if (sp.name == "actions")
                ae_ex.Actions.ResetInFromState();
            else if (sp.name == "material")
                ae_ex.Material.ResetInFromState();
            else Debug.LogError("Unknown property name: " + sp.name);
        }
    }
    protected void ResetToZero(bool to, OnResetToAction<bool> action)
    {
        if (GUILayout.Button(r0_cont, EditorStyles.miniButton, GUILayout.Width(25)))
        {
            for (int i = 0; i < ae_editor.targets.Length; i++)
            {
                action(to, i);
            }
        }
    }
    protected void ResetToOne(bool to, OnResetToAction<bool> action)
    {
        if (GUILayout.Button(r1_cont, EditorStyles.miniButton, GUILayout.Width(25)))
        {
            for (int i = 0; i < ae_editor.targets.Length; i++)
            {
                action(to, i);
            }
        }
    }
    protected void ResetToCurrent(bool to, OnResetToAction<bool> action)
    {
        if (GUILayout.Button(rc_cont, EditorStyles.miniButton, GUILayout.Width(25)))
        {
            for (int i = 0; i < ae_editor.targets.Length; i++)
            {
                action(to, i);
            }
        }
    }
    protected void ResetToCurrent(int componentNum, OnResetToAction<int> action)
    {
        if (GUILayout.Button(rc_cont, EditorStyles.miniButton, GUILayout.Width(25)))
        {
            for (int i = 0; i < ae_editor.targets.Length; i++)
            {
                action(componentNum, i);
            }
        }
    }
    protected void ResetToCurrentMainTransform()
    {
        if (GUILayout.Button(rc_cont, EditorStyles.miniButton, GUILayout.Width(25)))
        {
            for (int i = 0; i < ae_editor.targets.Length; i++)
            {
                AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
                switch (sp.name)
                {
                    case "transformation":
                        ae_ex.Transformation.mainTransform = ae_ex.transform;
                        ae_ex.Transformation.InitTransform();
                        break;
                    case "rotation":
                        ae_ex.Rotation.mainTransform = ae_ex.transform;
                        ae_ex.Rotation.InitTransform();
                        break;
                    case "scale":
                        ae_ex.Scale.mainTransform = ae_ex.transform;
                        ae_ex.Scale.InitTransform();
                        break;
                    case "color":
                        ae_ex.Color.mainTransform = ae_ex.transform;
                        ae_ex.Color.InitTransform();
                        break;
                    case "sequence":
                        ae_ex.Sequence.mainTransform = ae_ex.transform;
                        ae_ex.Sequence.InitTransform();
                        break;
                    case "actions":
                        ae_ex.Actions.mainTransform = ae_ex.transform;
                        ae_ex.Actions.InitTransform();
                        break;
                    case "material":
                        ae_ex.Material.mainTransform = ae_ex.transform;
                        ae_ex.Material.InitTransform();
                        break;
                    default:
                        Debug.LogError("Unknown property name: " + sp.name);
                        break;
                }
            }
        }
    }
}