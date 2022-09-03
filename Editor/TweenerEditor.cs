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

	GUIContent pickIcon;
	GUIContent fillIcon;
	Vector4 copiedValue = Vector3.zero;

	void OnEnable()
	{
		_instance = (Tweener)target;

		pickIcon = EditorGUIUtility.IconContent("d_Grid.PickingTool");
		fillIcon = EditorGUIUtility.IconContent("d_Grid.FillTool");

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
				case Tweener.ModuleType.MaterialColor:
					EditorGUI.LabelField(rect, "MC");
					break;
				case Tweener.ModuleType.MaterialAlpha:
					EditorGUI.LabelField(rect, "MA");
					break;
				case Tweener.ModuleType.CanvasGroupAlpha:
					EditorGUI.LabelField(rect, "CA");
					break;
				case Tweener.ModuleType.ImageColor:
					EditorGUI.LabelField(rect, "IC");
					break;
				case Tweener.ModuleType.ImageAlpha:
					EditorGUI.LabelField(rect, "IA");
					break;
				case Tweener.ModuleType.TextColor:
					EditorGUI.LabelField(rect, "TC");
					break;
				case Tweener.ModuleType.TextAlpha:
					EditorGUI.LabelField(rect, "TA");
					break;
			}

			rect.x += 30;
			rect.width = rectWidth - 80;

			var start = sequence.startTime;
			var end = sequence.endTime;
			EditorGUI.BeginDisabledGroup(true);
			EditorGUI.MinMaxSlider(rect, ref start, ref end, 0f, _instance.tweenData.duration);

			rect.x += rect.width + 5;
			rect.width = 50;
			if (sequence.targets != null && sequence.targets.Length > 0 && sequence.targets[0] != null) {
				EditorGUI.TextField(rect, sequence.targets[0].name);
			}
			else {
				EditorGUI.TextField(rect, "None");
			}
			EditorGUI.EndDisabledGroup();

			rect.x += rectWidth;
		};
		sequences.onAddCallback = list => {
			_sequenceProperty.arraySize++;
			var newIndex = _sequenceProperty.arraySize - 1;
			var newElement = _sequenceProperty.GetArrayElementAtIndex(newIndex);

			// new sequence initialization
			var moduleType = newElement.FindPropertyRelative("moduleType");
			moduleType.enumValueIndex = (int)Tweener.ModuleType.TransformPosition;

			var endTime = newElement.FindPropertyRelative("endTime");
			endTime.floatValue = _instance.tweenData.duration;

			var targetsProperty = newElement.FindPropertyRelative("targets");
			if (targetsProperty.arraySize == 0) {
				targetsProperty.arraySize++;
				var targets = targetsProperty.GetArrayElementAtIndex(0);
				targets.objectReferenceValue = _instance.gameObject;
			}
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
		var duration = EditorGUILayout.FloatField("Duration", data.duration);
		if (duration != data.duration && _instance.tweenData.sequences != null && _instance.tweenData.sequences.Length > 0) {
			data.duration = duration;
			for (int i = 0; i < _instance.tweenData.sequences.Length; i++) {
				var sequence = _instance.tweenData.sequences[i];
				if (sequence.startTime > duration)
					sequence.startTime = duration;
				if (sequence.endTime > duration)
					sequence.endTime = duration;
			}
		}
		data.loopType = (Tweener.TweenData.LoopType)EditorGUILayout.EnumPopup("Loop Type", data.loopType);

		if (isPlaying) {
			EditorGUI.EndDisabledGroup();
		}

		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
		if (!Application.isPlaying) {
			if (!isPlaying) {
				if (GUILayout.Button("Play", GUILayout.Width(50)))
					StartSimulation();
			}
			else {
				if (GUILayout.Button("Stop", GUILayout.Width(50)))
					StopSimulation();
			}
		}
		else {
			if (!_instance.isPlaying) {
				if (GUILayout.Button("Play", GUILayout.Width(50)))
					_instance.Play();
			}
			else {
				if (GUILayout.Button("Stop", GUILayout.Width(50)))
					_instance.Stop();
			}
		}
		EditorGUI.BeginDisabledGroup(true);
		if (!Application.isPlaying)
			EditorGUILayout.Slider(playtime, 0f, _instance.tweenData.duration);
		else
			EditorGUILayout.Slider(_instance.curTime, 0f, _instance.tweenData.duration);
		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndHorizontal();

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
		if (sequences.index == -1 || sequences.count <= 0 || sequences.index >= _instance.tweenData.sequences.Length)
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
		if (sequence.moduleType == Tweener.ModuleType.GameObjectEnable) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space(45, false);
			sequence.startTime = EditorGUILayout.Slider(sequence.startTime, 0f, _instance.tweenData.duration);
			EditorGUILayout.EndHorizontal();
			sequence.startTime = EditorGUILayout.DelayedFloatField("Target Time", sequence.startTime);
			sequence.startTime = sequence.startTime.DigitFloor();
			sequence.endTime = sequence.startTime;

			EditorGUILayout.Space();

			sequence.targetEnable = EditorGUILayout.Toggle("Target Enable", sequence.targetEnable);
		}
		else {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space(45, false);
			EditorGUILayout.MinMaxSlider(ref sequence.startTime, ref sequence.endTime, 0f, _instance.tweenData.duration);
			EditorGUILayout.Space(50, false);
			EditorGUILayout.EndHorizontal();
			sequence.startTime = EditorGUILayout.DelayedFloatField("Start Time", sequence.startTime);
			sequence.endTime = EditorGUILayout.DelayedFloatField("End Time", sequence.endTime);
			sequence.startTime = sequence.startTime.DigitFloor();
			sequence.endTime = sequence.endTime.DigitFloor();

			EditorGUILayout.Space();

			switch (sequence.moduleType) {
				case Tweener.ModuleType.TransformPosition:
				case Tweener.ModuleType.TransformRotation:
				case Tweener.ModuleType.RectTransformRotation:
					sequence.isLocal = EditorGUILayout.Toggle("IsLocal", sequence.isLocal);					
					CopyPasteVector3("Begin", ref sequence.begin);
					CopyPasteVector3("End", ref sequence.end);					
					break;
				case Tweener.ModuleType.TransformScale:				
					CopyPasteVector3("Begin", ref sequence.begin);
					CopyPasteVector3("End", ref sequence.end);
					break;
				case Tweener.ModuleType.RectTransformPosition:				
					sequence.isLocal = EditorGUILayout.Toggle("IsLocal", sequence.isLocal);
					CopyPasteVector2("Begin", ref sequence.begin);
					CopyPasteVector2("End", ref sequence.end);
					break;
				case Tweener.ModuleType.RectTransformScale:
					CopyPasteVector2("Begin", ref sequence.begin);
					CopyPasteVector2("End", ref sequence.end);
					break;
				case Tweener.ModuleType.MaterialColor:
				case Tweener.ModuleType.ImageColor:
				case Tweener.ModuleType.TextColor:
					CopyPasteColor("Begin", ref sequence.begin);
					CopyPasteColor("End", ref sequence.end);
					break;
				case Tweener.ModuleType.MaterialAlpha:
				case Tweener.ModuleType.CanvasGroupAlpha:
				case Tweener.ModuleType.ImageAlpha:
				case Tweener.ModuleType.TextAlpha:
					sequence.beginAlpha = EditorGUILayout.FloatField("Begin", sequence.beginAlpha);
					sequence.endAlpha = EditorGUILayout.FloatField("End", sequence.endAlpha);
					break;
			}

			EditorGUILayout.Space();

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
		}

		// Target
		EditorGUILayout.Space();
		EditorGUI.indentLevel++;
		EditorGUILayout.PropertyField(targetProperty);
		EditorGUI.indentLevel--;

		if (EditorGUI.EndChangeCheck())
			EditorUtility.SetDirty(_instance);
		EditorGUILayout.EndVertical();
	}

	void CopyPasteVector3(string name, ref Vector4 source)
	{
		EditorGUILayout.BeginHorizontal();
		source = EditorGUILayout.Vector3Field(name, source);
		if (GUILayout.Button(pickIcon, GUILayout.Width(24), GUILayout.Height(18))) {
			copiedValue = source;
		}
		if (GUILayout.Button(fillIcon, GUILayout.Width(24), GUILayout.Height(18))) {
			source = copiedValue;
		}
		EditorGUILayout.EndHorizontal();
	}

	void CopyPasteVector2(string name, ref Vector4 source)
	{
		EditorGUILayout.BeginHorizontal();
		source = EditorGUILayout.Vector2Field(name, source);
		if (GUILayout.Button(pickIcon, GUILayout.Width(24), GUILayout.Height(18))) {
			copiedValue = source;
		}
		if (GUILayout.Button(fillIcon, GUILayout.Width(24), GUILayout.Height(18))) {
			source = copiedValue;
		}
		EditorGUILayout.EndHorizontal();
	}

	void CopyPasteColor(string name, ref Vector4 source)
	{
		EditorGUILayout.BeginHorizontal();
		source = EditorGUILayout.ColorField(name, source);
		if (GUILayout.Button(pickIcon, GUILayout.Width(24), GUILayout.Height(18))) {
			copiedValue = source;
		}
		if (GUILayout.Button(fillIcon, GUILayout.Width(24), GUILayout.Height(18))) {
			source = copiedValue;
		}
		EditorGUILayout.EndHorizontal();
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
			var applyTime = playtime;

			switch (_instance.tweenData.loopType) {
				case Tweener.TweenData.LoopType.PlayOnce:
				case Tweener.TweenData.LoopType.Loop:
					if (playtime >= _instance.tweenData.duration) {
						StopSimulation();
						return;
					}
					break;
				case Tweener.TweenData.LoopType.PingPongOnce:
				case Tweener.TweenData.LoopType.PingPongLoop:
					if (playtime >= _instance.tweenData.duration * 2f) {
						StopSimulation();
						return;
					}
					else if (playtime >= _instance.tweenData.duration) {
						applyTime = _instance.tweenData.duration * 2f - playtime;
					}
					break;
			}
			_instance.tweenData.Update(applyTime);
		}
	}

	int undoGroup;
	void StartSimulation()
	{
		_instance.tweenData.Init();
		_instance.tweenData.Update(0f);

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

public static class FloatExtensions
{
	public static float DigitFloor(this float origin, int numDigits = 2)
	{
		var digit = Mathf.Pow(10, numDigits);
		return Mathf.Floor(origin * digit) / digit;
	}
}
