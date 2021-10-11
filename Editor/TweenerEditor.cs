using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Tweener))]
public partial class TweenerEditor : Editor
{
    Tweener _instance;
    SerializedProperty _sequenceProperty;
    ReorderableList sequences;

    // Preview
    bool isPlaying = false;
    float playtime;

    void OnEnable()
    {
        _instance = (Tweener)target;
        _sequenceProperty = serializedObject.FindProperty("tweenData.sequences");
        sequences = new ReorderableList(serializedObject, _sequenceProperty);
        sequences.drawHeaderCallback = (rect) => {
            EditorGUI.LabelField(rect, "Sequences");
        };
        sequences.drawElementCallback = (rect, index, isActive, isFocused) => {
            var sequence = _instance.tweenData.sequences[index];
            float rectWidth = rect.width;
            rect.width = 40;
            rect.height -= 4;
            rect.y += 1;

            switch (sequence.moduleType) {
                case Tweener.ModuleType.GameObjectEnable:
                    EditorGUI.LabelField(rect, "GE");
                    break;
                case Tweener.ModuleType.TransformPosition:
                    EditorGUI.LabelField(rect, "TP");
                    break;
                case Tweener.ModuleType.TransformRotation:
                    EditorGUI.LabelField(rect, "TR");
                    break;
                case Tweener.ModuleType.TransformScale:
                    EditorGUI.LabelField(rect, "TS");
                    break;
                case Tweener.ModuleType.RectTransformPosition:
                    EditorGUI.LabelField(rect, "RP");
                    break;
                case Tweener.ModuleType.RectTransformRotation:
                    EditorGUI.LabelField(rect, "RR");
                    break;
                case Tweener.ModuleType.RectTransformScale:
                    EditorGUI.LabelField(rect, "RS");
                    break;
                case Tweener.ModuleType.ImageColor:
                    EditorGUI.LabelField(rect, "IC");
                    break;
                case Tweener.ModuleType.ImageAlpha:
                    EditorGUI.LabelField(rect, "IA");
                    break;
            }

            rect.x += 30;
            rect.width = rectWidth - 32;

            var start = sequence.startTime;
            var end = sequence.endTime;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.MinMaxSlider(rect, ref start, ref end, 0f, _instance.tweenData.duration);
            EditorGUI.EndDisabledGroup();

            rect.x += rectWidth;
        };

        EditorApplication.update += UpdateMethod;
    }

    void OnDisable()
    {
        EditorApplication.update -= UpdateMethod;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (isPlaying) {
            EditorGUI.BeginDisabledGroup(true);
        }

        EditorGUI.BeginChangeCheck();

        _instance.playOnAwake = EditorGUILayout.Toggle("Play OnAwake", _instance.playOnAwake);
        _instance.ignoreTimeScale = EditorGUILayout.Toggle("Ignore TimeScale", _instance.ignoreTimeScale);

        var data = _instance.tweenData;
        data.duration = EditorGUILayout.FloatField("Duration", data.duration);
        data.loopType = (Tweener.TweenData.LoopType)EditorGUILayout.EnumPopup("Loop Type", data.loopType);

        if (isPlaying) {
            EditorGUI.EndDisabledGroup();
        }

        EditorGUILayout.Space();

        if (!Application.isPlaying) {
            EditorGUILayout.BeginHorizontal();

            if (!isPlaying) {
                if (GUILayout.Button("Play", GUILayout.Width(50))) {
                    StartSimulation();
                }
            }
            else {
                if (GUILayout.Button("Stop", GUILayout.Width(50))) {
                    StopSimulation();
                }
            }
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Slider(playtime, 0f, _instance.tweenData.duration);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        if (isPlaying) {
            EditorGUI.BeginDisabledGroup(true);
        }
        sequences.DoLayoutList();
        DrawSequenceDetail();

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(_instance);

        if (isPlaying) {
            EditorGUI.EndDisabledGroup();
        }

        serializedObject.ApplyModifiedProperties();
    }

    void DrawSequenceDetail()
    {
        if (_instance.tweenData.sequences == null || _instance.tweenData.sequences.Length == 0)
            return;
        if (sequences.index == -1 || sequences.index >= _instance.tweenData.sequences.Length)
            return;

        var sequence = _instance.tweenData.sequences[sequences.index];
        if (sequence == null)
            return;
        var sequenceElement = _sequenceProperty.GetArrayElementAtIndex(sequences.index);
        var targetProperty = sequenceElement.FindPropertyRelative("targets");

        EditorGUILayout.BeginVertical("Box");
        EditorGUI.BeginChangeCheck();

        // Module Type
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(" Å£ ", GUILayout.Width(30))) {
            sequences.index = -1;
        }
        EditorGUILayout.LabelField("Module", EditorStyles.boldLabel, GUILayout.Width(50));
        sequence.moduleType = (Tweener.ModuleType)EditorGUILayout.EnumPopup(sequence.moduleType);
        EditorGUILayout.EndHorizontal();

        // Start, End Time
        EditorGUILayout.MinMaxSlider(ref sequence.startTime, ref sequence.endTime, 0f, _instance.tweenData.duration);
        sequence.startTime = EditorGUILayout.DelayedFloatField("Start Time", sequence.startTime);
        sequence.endTime = EditorGUILayout.DelayedFloatField("End Time", sequence.endTime);

        switch (sequence.moduleType) {
            case Tweener.ModuleType.GameObjectEnable:
                sequence.beginEnable = EditorGUILayout.Toggle("Begin Enable", sequence.beginEnable);
                sequence.endEnable = EditorGUILayout.Toggle("Begin Enable", sequence.endEnable);
                break;
            case Tweener.ModuleType.TransformPosition:
            case Tweener.ModuleType.TransformRotation:
            case Tweener.ModuleType.RectTransformRotation:
                sequence.isRelative = EditorGUILayout.Toggle("IsRelative", sequence.isRelative);
                sequence.begin = EditorGUILayout.Vector3Field("Begin", sequence.begin);
                sequence.end = EditorGUILayout.Vector3Field("End", sequence.end);
                break;
            case Tweener.ModuleType.TransformScale:
                sequence.isRelative = EditorGUILayout.Toggle("IsRelative", sequence.isRelative);
                if (!sequence.isRelative)
                    sequence.begin = EditorGUILayout.Vector3Field("Begin", sequence.begin);
                sequence.end = EditorGUILayout.Vector3Field("End", sequence.end);
                break;
            case Tweener.ModuleType.RectTransformPosition:
                sequence.isRelative = EditorGUILayout.Toggle("IsRelative", sequence.isRelative);
                sequence.begin = EditorGUILayout.Vector2Field("Begin", sequence.begin);
                sequence.end = EditorGUILayout.Vector2Field("End", sequence.end);
                break;
            case Tweener.ModuleType.RectTransformScale:
                sequence.isRelative = EditorGUILayout.Toggle("IsRelative", sequence.isRelative);
                if (!sequence.isRelative)
                    sequence.begin = EditorGUILayout.Vector2Field("Begin", sequence.begin);
                sequence.end = EditorGUILayout.Vector2Field("End", sequence.end);
                break;
            case Tweener.ModuleType.ImageColor:
                sequence.isRelative = EditorGUILayout.Toggle("IsRelative", sequence.isRelative);
                if (!sequence.isRelative)
                    sequence.beginColor = EditorGUILayout.ColorField("Begin", sequence.beginColor);
                sequence.endColor = EditorGUILayout.ColorField("End", sequence.endColor);
                break;
            case Tweener.ModuleType.ImageAlpha:
                sequence.isRelative = EditorGUILayout.Toggle("IsRelative", sequence.isRelative);
                if (!sequence.isRelative)
                    sequence.beginAlpha = EditorGUILayout.FloatField("Begin", sequence.beginAlpha);
                sequence.endAlpha = EditorGUILayout.FloatField("End", sequence.endAlpha);
                break;
        }

        var rect = EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Ease Type");
        sequence.easeType = (Tweener.EaseType)EditorGUILayout.EnumPopup(sequence.easeType);

        const int graphSize = 32;

        // Outline
        rect.x = rect.width - graphSize * 0.5f - 1;
        rect.y += 3;
        rect.width = graphSize + 3;
        rect.height = graphSize + 3;
        EditorGUI.DrawRect(rect, Color.black);

        // Panel
        rect.x++;
        rect.y++;
        rect.width = graphSize + 1;
        rect.height = graphSize + 1;
        EditorGUI.DrawRect(rect, new Color(0.164f, 0.164f, 0.164f));
        DrawEaseFunction(rect, graphSize, new Color(0.36f, 0.6f, 0.3f), sequence.easeType);

        EditorGUILayout.EndVertical();
        EditorGUILayout.LabelField("");
        EditorGUILayout.EndHorizontal();

        // Target
        EditorGUILayout.PropertyField(targetProperty);

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(_instance);
        EditorGUILayout.EndVertical();
    }

    void DrawEaseFunction(Rect rect, int width, Color color, Tweener.EaseType easeType)
    {
        // Graph
        for (int i = 0; i < width; i++) {
            int x = i;
            var t = (float)x / (float)width;
            var r = Tweener.EaseFunction(easeType, t);
            int y = (int)(r * width);

            DrawPixel(rect, x, y, color);
        }

        // Cursor
        var cursorX = (int)(cursorRate * width);
        var cursorR = Tweener.EaseFunction(easeType, cursorRate);
        int cursorY = (int)(cursorR * width);

        DrawPixel(rect, cursorX, cursorY, Color.red);
    }

    void DrawPixel(Rect rect, int x, int y, Color color)
    {
        var point = new Rect();
        point.width = 1;
        point.height = 1;
        point.x = rect.x + x;
        point.y = rect.y + rect.height - y - 1;

        EditorGUI.DrawRect(point, color);
    }

    float cursorRate;
    float cursorSpeed = 0.005f;
    void UpdateMethod()
    {
        cursorRate += cursorSpeed;
        if (cursorRate > 1f)
            cursorRate -= 1f;
        Repaint();

        if (isPlaying) {
            playtime += 0.01667f;
            if (playtime >= _instance.tweenData.duration) {
                StopSimulation();
            }
            else {
                _instance.tweenData.Update(playtime);
            }
        }
    }

    int undoGroup;
    void StartSimulation()
    {
        _instance.tweenData.Init();
        _instance.tweenData.Reset();
        isPlaying = true;
        playtime = 0f;
    }

    void StopSimulation()
    {
        playtime = 0f;
        isPlaying = false;

        _instance.tweenData.Restore();
    }
}
