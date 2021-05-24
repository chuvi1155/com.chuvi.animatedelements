using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(AnimatedElementEx))]
[CanEditMultipleObjects]
public class AnimatedElementExEditor : Editor
{

    // common properties
    SerializedProperty Pause, commonTransform, TimeWait, IsRandomTimeWait, MaxRandomTimeWait, StartAnimations, EndAnimations, OnEnableEvent, OnDisableEvent;

    AnimatedTransformationEditor transformation;
    AnimatedRotationEditor rotation;
    AnimatedScaleEditor scale;
    AnimatedColorEditor color;
    AnimatedSequenceEditor sequence;
    AnimatedActionsEditor actions;
    AnimatedMaterialEditor material;

    public AnimatedElementEx ae { get { return target as AnimatedElementEx; } }

    static bool showCommonSettings = true;
    static bool showTransformationSettings = false;
    static bool showRotationSettings = false;
    static bool showScaleSettings = false;
    static bool showColorSettings = false;
    static bool showSequenceSettings = false;
    static bool showActionsSettings = false;
    static bool showMaterialSettings = false;

    Color gui_col;

    public static Texture pauseIcon;
    public static Texture randomIcon;
    public static Texture waitIcon;
    public static Texture repeateIcon;
    public static Texture usedIcon;

    void CommonInit()
    {
        Pause = serializedObject.FindProperty("Pause");
        commonTransform = serializedObject.FindProperty("commonTransform");
        TimeWait = serializedObject.FindProperty("TimeWait");
        IsRandomTimeWait = serializedObject.FindProperty("IsRandomTimeWait");
        MaxRandomTimeWait = serializedObject.FindProperty("MaxRandomTimeWait");
        StartAnimations = serializedObject.FindProperty("StartAnimations");
        EndAnimations = serializedObject.FindProperty("EndAnimations");
        OnEnableEvent = serializedObject.FindProperty("OnEnableEvent");
        OnDisableEvent = serializedObject.FindProperty("OnDisableEvent");

        transformation = new AnimatedTransformationEditor(this);
        rotation = new AnimatedRotationEditor(this);
        scale = new AnimatedScaleEditor(this);
        color = new AnimatedColorEditor(this);
        sequence = new AnimatedSequenceEditor(this);
        actions = new AnimatedActionsEditor(this);
        material = new AnimatedMaterialEditor(this);
    }
    void AnimatedComponentsInit()
    {
        transformation.Init();
        rotation.Init();
        scale.Init();
        color.Init();
        sequence.Init();
        actions.Init();
        material.Init();
        /*
        transformation.showCommonSettings = false;//EditorPrefs.GetBool("AnimatedElementEx_common_showTransformationSettings", false);
        rotation.showCommonSettings = false;//EditorPrefs.GetBool("AnimatedElementEx_common_showRotationSettings", false);
        scale.showCommonSettings = false;//EditorPrefs.GetBool("AnimatedElementEx_common_showScaleSettings", false);
        color.showCommonSettings = false;//EditorPrefs.GetBool("AnimatedElementEx_common_showColorSettings", false);
        sequence.showCommonSettings = false;// EditorPrefs.GetBool("AnimatedElementEx_common_showSequenceSettings", false);
        actions.showCommonSettings = false;//EditorPrefs.GetBool("AnimatedElementEx_common_showActionsSettings", false);
        material.showCommonSettings = false;
        */
    }

    private void OnEnable()
    {
        showCommonSettings = EditorPrefs.GetBool("AnimatedElementEx_showCommonSettings", true);
        showTransformationSettings = EditorPrefs.GetBool("AnimatedElementEx_showTransformationSettings", false);
        showRotationSettings = EditorPrefs.GetBool("AnimatedElementEx_showRotationSettings", false);
        showScaleSettings = EditorPrefs.GetBool("AnimatedElementEx_showScaleSettings", false);
        showColorSettings = EditorPrefs.GetBool("AnimatedElementEx_showColorSettings", false);
        showSequenceSettings = EditorPrefs.GetBool("AnimatedElementEx_showSequenceSettings", false);
        showActionsSettings = EditorPrefs.GetBool("AnimatedElementEx_showActionsSettings", false);
        showMaterialSettings = EditorPrefs.GetBool("AnimatedElementEx_showMaterialSettings", false);

        string editorPath = GetPackageRelativePath();
        if (pauseIcon == null) pauseIcon = AssetDatabase.LoadAssetAtPath<Texture>(editorPath + "/Editor/Icons/pause-icon.png");
        if (randomIcon == null) randomIcon = AssetDatabase.LoadAssetAtPath<Texture>(editorPath + "/Editor/Icons/random-icon.png");
        if (waitIcon == null) waitIcon = AssetDatabase.LoadAssetAtPath<Texture>(editorPath + "/Editor/Icons/wait-icon.png");
        if (repeateIcon == null) repeateIcon = AssetDatabase.LoadAssetAtPath<Texture>(editorPath + "/Editor/Icons/repeate-icon.png");
        if (usedIcon == null) usedIcon = AssetDatabase.LoadAssetAtPath<Texture>(editorPath + "/Editor/Icons/used-icon.png");

        CommonInit();
        AnimatedComponentsInit();
    }

    private static string GetPackageRelativePath()
    {
        // Check for potential UPM package
        string packagePath = Path.GetFullPath("Packages/com.chuvi.animatedelements");
        if (Directory.Exists(packagePath))
        {
            return "Packages/com.chuvi.animatedelements";
        }

        packagePath = Path.GetFullPath("Assets/..");
        if (Directory.Exists(packagePath))
        {
            // Search default location for development package
            if (Directory.Exists(packagePath + "/Assets/com.chuvi.animatedelements/Runtime/Editor"))
            {
                return "Assets/com.chuvi.animatedelements/Runtime/Editor";
            }

            // Search for default location of normal TextMesh Pro AssetStore package
            if (Directory.Exists(packagePath + "/Assets/AnimatedElement/Editor"))
            {
                return "Assets/AnimatedElement";
            }

            // Search for potential alternative locations in the user project
            //string[] matchingPaths = Directory.GetDirectories(packagePath, "AnimatedElement", SearchOption.AllDirectories);
            //packagePath = ValidateLocation(matchingPaths, packagePath);
            return packagePath;
        }

        return null;
    }

    void OnDisable()
    {
        EditorPrefs.SetBool("AnimatedElementEx_showCommonSettings", showCommonSettings);
        EditorPrefs.SetBool("AnimatedElementEx_showTransformationSettings", showTransformationSettings);
        EditorPrefs.SetBool("AnimatedElementEx_showRotationSettings", showRotationSettings);
        EditorPrefs.SetBool("AnimatedElementEx_showScaleSettings", showScaleSettings);
        EditorPrefs.SetBool("AnimatedElementEx_showColorSettings", showColorSettings);
        EditorPrefs.SetBool("AnimatedElementEx_showSequenceSettings", showSequenceSettings);
        EditorPrefs.SetBool("AnimatedElementEx_showActionsSettings", showActionsSettings);
        EditorPrefs.SetBool("AnimatedElementEx_showMaterialSettings", showMaterialSettings);
    }

    public override void OnInspectorGUI()
    {
        gui_col = GUI.color;
        serializedObject.Update();
        DrawCommonSettings();
        bool changed = GUI.changed;
        DrawTransformationSettings();
        changed |= GUI.changed;
        DrawRotationSettings();
        changed |= GUI.changed;
        DrawScaleSettings();
        changed |= GUI.changed;
        DrawColorSettings();
        changed |= GUI.changed;
        DrawSequenceSettings();
        changed |= GUI.changed;
        DrawActionsSettings();
        changed |= GUI.changed;
        DrawMaterialSettings();
        GUI.changed = changed;
        serializedObject.ApplyModifiedProperties();
        if (Application.isPlaying)
            Repaint();
    }
       
    void DrawCommonSettings()
    {
        EditorGUILayout.BeginVertical("box");
        GUI.color = !Pause.hasMultipleDifferentValues && Pause.boolValue ? Color.red : (Color)new Color32(0, 208, 255, 255);
        if (GUILayout.Button("Common settings" + (GUI.color == Color.red? " (Pause)" : "") , EditorStyles.toolbarButton))
            showCommonSettings = !showCommonSettings;
        GUI.color = gui_col;
        Rect rect = GUILayoutUtility.GetLastRect();
        rect.xMin += 5;
        rect.width -= 5;
        if (showCommonSettings)
        {
            GUI.color = (target as AnimatedElementEx).IsEmulateInEditor ? Color.yellow : gui_col;
            if (GUILayout.Button("Emulate"))
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    (targets[i] as AnimatedElementEx).IsEmulateInEditor = !(targets[i] as AnimatedElementEx).IsEmulateInEditor;
                }
            }
            GUI.color = gui_col;
            EditorGUILayout.PropertyField(Pause, new GUIContent("Pause"));
            if (!Pause.hasMultipleDifferentValues)
            {
                if (Pause.boolValue)
                {
                    EditorGUILayout.HelpBox("Pause....", MessageType.Error);
                    Rect r = new Rect(rect);
                    r.width = r.height;
                    GUI.DrawTexture(r, pauseIcon);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Some where Pause....", MessageType.Error);
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(commonTransform, new GUIContent("Transform"));
            ResetToCurrentMainTransform();
            ResetToCurrentParentMainTransform();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(TimeWait, new GUIContent("TimeWait"));
            EditorGUILayout.PropertyField(IsRandomTimeWait, new GUIContent("IsRandomTimeWait"));
            if (!IsRandomTimeWait.hasMultipleDifferentValues && IsRandomTimeWait.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(MaxRandomTimeWait, new GUIContent("MaxRandomTimeWait"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(OnEnableEvent);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(StartAnimations, new GUIContent("StartAnimations"));
            EditorGUILayout.PropertyField(EndAnimations, new GUIContent("EndAnimations"));

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(OnDisableEvent);
        }
        else
        {
            if (!Pause.hasMultipleDifferentValues)
            {
                if (Pause.boolValue)
                {
                    Rect r = new Rect(rect);
                    r.width = r.height;
                    GUI.DrawTexture(r, pauseIcon);
                }
            }
        }
        EditorGUILayout.EndVertical();
    }
    void DrawTransformationSettings()
    {
        EditorGUILayout.BeginVertical("box");
        GUI.color = new Color32(0, 208, 255, 255);
        if (GUILayout.Button("Transformation settings" + transformation.GetUsedText(), EditorStyles.toolbarButton))
            showTransformationSettings = !showTransformationSettings;
        GUI.color = gui_col;
        DrawIcons(transformation);
        GUI.changed = false;
        if (showTransformationSettings)
        {
            transformation.Draw();
        }
        EditorGUILayout.EndVertical();
    }
    void DrawRotationSettings()
    {
        EditorGUILayout.BeginVertical("box");
        GUI.color = new Color32(0, 208, 255, 255);
        if (GUILayout.Button("Rotation settings" + rotation.GetUsedText(), EditorStyles.toolbarButton))
            showRotationSettings = !showRotationSettings;
        GUI.color = gui_col;
        DrawIcons(rotation);
        GUI.changed = false;
        if (showRotationSettings)
        {
            rotation.Draw();
        }
        EditorGUILayout.EndVertical();
    }
    void DrawScaleSettings()
    {
        EditorGUILayout.BeginVertical("box");
        GUI.color = new Color32(0, 208, 255, 255);
        if (GUILayout.Button("Scale settings" + scale.GetUsedText(), EditorStyles.toolbarButton))
            showScaleSettings = !showScaleSettings;
        GUI.color = gui_col;
        DrawIcons(scale);
        GUI.changed = false;
        if (showScaleSettings)
        {
            scale.Draw();
        }
        EditorGUILayout.EndVertical();
    }
    void DrawColorSettings()
    {
        EditorGUILayout.BeginVertical("box");
        GUI.color = new Color32(0, 208, 255, 255);
        if (GUILayout.Button("Color settings" + color.GetUsedText(), EditorStyles.toolbarButton))
            showColorSettings = !showColorSettings;
        GUI.color = gui_col;
        DrawIcons(color);
        GUI.changed = false;
        if (showColorSettings)
        {
            color.Draw();
        }
        EditorGUILayout.EndVertical();
    }
    void DrawSequenceSettings()
    {
        EditorGUILayout.BeginVertical("box");
        GUI.color = new Color32(0, 208, 255, 255);
        if (GUILayout.Button("Sequence settings" + sequence.GetUsedText(), EditorStyles.toolbarButton))
            showSequenceSettings = !showSequenceSettings;
        GUI.color = gui_col;
        DrawIcons(sequence);
        GUI.changed = false;
        if (showSequenceSettings)
        {
            sequence.Draw();
        }
        EditorGUILayout.EndVertical();
    }
    void DrawActionsSettings()
    {
        EditorGUILayout.BeginVertical("box");
        GUI.color = new Color32(0, 208, 255, 255);
        if (GUILayout.Button("Actions settings" + actions.GetUsedText(), EditorStyles.toolbarButton))
            showActionsSettings = !showActionsSettings;
        GUI.color = gui_col;
        DrawIcons(actions);
        GUI.changed = false;
        if (showActionsSettings)
        {
            actions.Draw();
        }
        EditorGUILayout.EndVertical();
    }
    void DrawMaterialSettings()
    {
        EditorGUILayout.BeginVertical("box");
        GUI.color = new Color32(0, 208, 255, 255);
        if (GUILayout.Button("Material settings" + material.GetUsedText(), EditorStyles.toolbarButton))
            showMaterialSettings = !showMaterialSettings;
        GUI.color = gui_col;
        DrawIcons(material);
        GUI.changed = false;
        if (showMaterialSettings)
        {
            material.Draw();
        }
        EditorGUILayout.EndVertical();
    }

    void DrawIcons(AnimatedBehaviourEditor abe)
    {
        Rect rect = GUILayoutUtility.GetLastRect();
        rect.xMin += 5;
        rect.width -= 5;
        bool? isUsed = abe.IsUsed();
        bool? isRandom = abe.IsRandom();
        bool? isRepeat = abe.IsRepeate();
        bool? isRandomTimeWaitRepeate = abe.IsRandomTimeWaitRepeate();
        bool? isWaiting = abe.IsWaiting();
        if (isUsed.HasValue && isUsed.Value)
        {
            Rect r = new Rect(rect);
            r.width = rect.height;
            rect.xMin += r.width;
            GUI.DrawTexture(r, usedIcon);
        }
        if (isRandom.HasValue && isRandom.Value && (!isRepeat.HasValue || !isRepeat.Value))
        {
            Rect r = new Rect(rect);
            r.width = rect.height;
            rect.xMin += r.width;
            GUI.DrawTexture(r, randomIcon);
        }
        else if ((isRepeat.HasValue && isRepeat.Value))
        {
            Rect r = new Rect(rect);
            r.width = rect.height;
            rect.xMin += r.width;
            GUI.DrawTexture(r, repeateIcon);
            if (isRandomTimeWaitRepeate.HasValue && isRandomTimeWaitRepeate.Value)
            {
                r = new Rect(rect);
                r.width = rect.height;
                rect.xMin += r.width;
                GUI.DrawTexture(r, randomIcon);
            }
        }
        if (isWaiting.HasValue && isWaiting.Value)
        {
            Rect r = new Rect(rect);
            r.width = rect.height;
            rect.xMin += r.width;
            GUI.DrawTexture(r, waitIcon);
        }
        //Rect _r = new Rect(rect.xMax - 50, rect.y, 50, rect.height - 2);
        //if (GUI.Button(_r, "Reset"))
        //{
        //    abe.ResetInFromState();
        //}
    }

    void ResetToCurrentMainTransform()
    {
        if (GUILayout.Button(new GUIContent("RC", "Reset to current"), EditorStyles.miniButton, GUILayout.Width(25)))
        {
            for (int i = 0; i < targets.Length; i++)
            {
                AnimatedElementEx ae_ex = targets[i] as AnimatedElementEx;
                ae_ex.commonTransform = ae_ex.transform;
            }
        }
    }
    void ResetToCurrentParentMainTransform()
    {
        if (GUILayout.Button(new GUIContent("RCP", "Reset to current parent"), EditorStyles.miniButton, GUILayout.Width(25)))
        {
            for (int i = 0; i < targets.Length; i++)
            {
                AnimatedElementEx ae_ex = targets[i] as AnimatedElementEx;
                ae_ex.commonTransform = ae_ex.transform.parent;
            }
        }
    }
}
