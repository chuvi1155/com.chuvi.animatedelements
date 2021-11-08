using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class AnimatedSequenceEditor : AnimatedBehaviourEditor
{
    SerializedProperty seq_types;
    SerializedProperty image;
    SerializedProperty sprite;
    SerializedProperty frameAnimation;
    // raw
    SerializedProperty PlayRaw;
    SerializedProperty RawFileName;

    SerializedProperty actionFrames;
    SerializedProperty onActionFrameEvent;

    protected override bool CanRevert { get { return false; } }

    public AnimatedSequenceEditor(AnimatedElementExEditor _ae) : base(_ae, "sequence")
    {
    }

    public override void Init()
    {
        base.Init();

        seq_types = sp.FindPropertyRelative("types");
        image = sp.FindPropertyRelative("image");
        sprite = sp.FindPropertyRelative("sprite");
        frameAnimation = sp.FindPropertyRelative("frameAnimation");

        PlayRaw = sp.FindPropertyRelative("PlayRaw");
        RawFileName = sp.FindPropertyRelative("RawFileName");

        actionFrames = sp.FindPropertyRelative("actionFrames");
        onActionFrameEvent = sp.FindPropertyRelative("onActionFrameEvent");
    }

    protected override void DrawBefore()
    {
        EditorGUILayout.BeginVertical("box");
        var val = EditorGUILayout.MaskField("Sequence types", seq_types.intValue, seq_types.enumNames);
        if (val != seq_types.intValue)
            seq_types.intValue = val;

        if (seq_types.intValue != 0)
        {
            var ae = ae_editor.target as AnimatedElementEx;
            EditorGUILayout.BeginHorizontal();
            bool smv = EditorGUI.showMixedValue;
            if (Exists(AnimatedSequence.SequenceTypes.Image))
            {
                EditorGUILayout.PropertyField(image, new GUIContent("UI"));
                ResetToCurrent(0, OnResetToCurrent);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (Exists(AnimatedSequence.SequenceTypes.Sprite))
            {
                EditorGUILayout.PropertyField(sprite, new GUIContent("2D Sprite"));
                ResetToCurrent(1, OnResetToCurrent);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(PlayRaw, new GUIContent("PlayRaw"), true);
            EditorGUILayout.PropertyField(RawFileName, new GUIContent("RawFileName"), true);

            EditorGUILayout.PropertyField(actionFrames, new GUIContent("Action Frames"), true);
            EditorGUILayout.PropertyField(onActionFrameEvent, new GUIContent("OnActionFrameEvent"), true);


            if (ae.Sequence.frameAnimation == null)
                ae.Sequence.frameAnimation = new Sprite[0];
            if (!frameAnimation.hasMultipleDifferentValues)
                EditorGUILayout.HelpBox("Frames count:" + ae.Sequence.frameAnimation.Length, MessageType.None);
            EditorGUILayout.PropertyField(frameAnimation, new GUIContent("Frames"), true);
            if (GUILayout.Button(new GUIContent("Get from selected"), EditorStyles.miniButton))
            {
                SetFramesAnimation wnd = EditorWindow.GetWindow<SetFramesAnimation>(true);
                wnd.Target = ae;
            }
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }
    bool Exists(AnimatedSequence.SequenceTypes type)
    {
        return !seq_types.hasMultipleDifferentValues && (seq_types.intValue & (int)type) == (int)type;
    }
    void OnResetToCurrent(int componentNum, int i)
    {
        AnimatedElementEx ae_ex = ae_editor.targets[i] as AnimatedElementEx;
        if (componentNum == 0) // Image
        {
            if (ae_ex.Sequence.mainTransform != null)
            {
                Image img = ae_ex.Sequence.mainTransform.GetComponent<Image>();
                ae_ex.Sequence.image = img;
            }
            else
            {
                if (ae_ex.commonTransform != null)
                {
                    Image img = ae_ex.commonTransform.GetComponent<Image>();
                    ae_ex.Sequence.image = img;
                }
                else
                {
                    Image img = ae_ex.GetComponent<Image>();
                    ae_ex.Sequence.image = img;
                }
            }
        }
        else if (componentNum == 1) // Sprite
        {
            if (ae_ex.Sequence.mainTransform != null)
            {
                SpriteRenderer img = ae_ex.Sequence.mainTransform.GetComponent<SpriteRenderer>();
                ae_ex.Sequence.sprite = img;
            }
            else
            {
                if (ae_ex.commonTransform != null)
                {
                    SpriteRenderer img = ae_ex.commonTransform.GetComponent<SpriteRenderer>();
                    ae_ex.Sequence.sprite = img;
                }
                else
                {
                    SpriteRenderer img = ae_ex.GetComponent<SpriteRenderer>();
                    ae_ex.Sequence.sprite = img;
                }
            }
        }
    }

    public override void RevertStates()
    {

    }
    class SetFramesAnimation : EditorWindow
    {
        public AnimatedElementEx Target;

        void Update()
        {
            Repaint();
        }

        void OnGUI()
        {
            Target = (AnimatedElementEx)EditorGUILayout.ObjectField(Target, typeof(AnimatedElementEx), true);
            EditorGUILayout.Space();

            if (Target != null)
            {
                List<Sprite> list = new List<Sprite>();
                foreach (var item in Selection.instanceIDs)
                {
                    list.Add(AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(item)));
                }
                list.Sort((a, b) => { return a.name.CompareTo(b.name); });
                Target.Sequence.frameAnimation = list.ToArray();

                if (GUILayout.Button("Ok"))
                {
                    Selection.activeGameObject = Target.gameObject;
                    Close();
                }
            }
        }
    }
}
