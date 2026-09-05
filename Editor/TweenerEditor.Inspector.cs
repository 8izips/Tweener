using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Tweener))]
public partial class TweenerEditor : Editor
{
	Tweener instance;
	SerializedProperty sequenceProperty;
	ReorderableList sequenceList;

	int selectedModuleIndex;
	string[] moduleNames;
	private void OnEnable()
	{
		instance = (Tweener)target;
		sequenceProperty = serializedObject.FindProperty("sequences");
		sequenceList = new ReorderableList(serializedObject, sequenceProperty, true, true, true, true)
		{
			multiSelect = true,

			drawHeaderCallback = (Rect rect) =>
			{
				EditorGUI.LabelField(rect, "Sequences");
			},

			drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
			{
				var module = instance.sequences[index];
				if (module != null)
					module.DrawSequenceSingle(rect, instance.duration);
			},
			
			onAddCallback = (ReorderableList list) =>
			{
				sequenceProperty.arraySize++;
				var newElement = sequenceProperty.GetArrayElementAtIndex(sequenceProperty.arraySize - 1);
				newElement.managedReferenceValue = Tweener.CreateModule(selectedModuleIndex);
			},
		};

		moduleNames = Tweener.modulePaths.ToArray();
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUI.BeginChangeCheck();
		
		DrawParameters();
		EditorGUILayout.Space();

		DrawSequences();
		EditorGUILayout.Space();

		DrawSequenceDetail();
		EditorGUILayout.Space();

		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(instance, "Modify Tweener");
			EditorUtility.SetDirty(instance);
		}
			
		serializedObject.ApplyModifiedProperties();
	}

	void DrawParameters()
	{
		EditorGUILayout.Space();
		instance.playerId = EditorGUILayout.IntField("Player ID", instance.playerId);
		
		EditorGUILayout.Space();
		instance.speed = Mathf.Max(0f, EditorGUILayout.FloatField("Speed", instance.speed));
		instance.duration = Mathf.Max(0f, EditorGUILayout.FloatField("Duration", instance.duration));
		instance.looping = EditorGUILayout.Toggle("Looping", instance.looping);
		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();
		instance.startDelay = Mathf.Max(0f, EditorGUILayout.FloatField("Start Delay", instance.startDelay));
		GUILayout.Space(10);
		instance.randomStartDelay = EditorGUILayout.ToggleLeft("Random", instance.randomStartDelay, GUILayout.Width(80));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
		instance.ignoreTimeScale = EditorGUILayout.Toggle("Ignore Time Scale", instance.ignoreTimeScale);
		instance.playOnAwake = EditorGUILayout.Toggle("Play On Awake*", instance.playOnAwake);
		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
		if (isPreviewing) {
			if (GUILayout.Button("Stop", GUILayout.Width(58))) {
				StopPreview();
			}	
		}
		else {
			if (GUILayout.Button("Play", GUILayout.Width(58))) {				
				StartPreview();
			}	
		}
		
		var curPreviewTime = EditorGUILayout.Slider(previewTime, 0, instance.duration);
		if (curPreviewTime != previewTime) {
			var previewTimeDiff = curPreviewTime - previewTime;
			previewTime = curPreviewTime;

			if (previewTimeDiff < 0f)
				ResetPreview();

			isPreviewing = false;
			EvaluatePreview(previewTime);
		}

		EditorGUILayout.EndHorizontal();
	}

	void DrawSequences()
	{
		sequenceList.DoLayoutList();
		EditorGUILayout.Space();

		var inspectorWidth = EditorGUIUtility.currentViewWidth;
		selectedModuleIndex = EditorGUILayout.Popup(selectedModuleIndex, moduleNames, GUILayout.Width(inspectorWidth - 120));
	}

	int selectedIndex = -1;
	void DrawSequenceDetail()
	{
		selectedIndex = -1;
		for (int i = 0; i < sequenceList.count; i++) {
			if (sequenceList.IsSelected(i))
				selectedIndex = i;
		}
		if (selectedIndex < 0)
			return;
		var selectedSequence = instance.sequences[selectedIndex];
		if (selectedSequence == null)
			return;
		
		EditorGUILayout.BeginVertical("Box");
		{
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(" ▲ ", GUILayout.Width(30))) {
				sequenceList.ClearSelection();
			}
			GUILayout.Space(10);
			selectedSequence.isEnable = EditorGUILayout.Toggle("", selectedSequence.isEnable, GUILayout.Width(20));
			EditorGUILayout.LabelField(selectedSequence.ModuleName + "@" + selectedIndex);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();

			selectedSequence.DrawSequenceDetail(instance.duration, easeRate);
		}
		EditorGUILayout.EndVertical();
	}
}