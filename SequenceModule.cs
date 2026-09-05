using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public abstract class SequenceModule
{
	public delegate void EvaluateDelegate(float curTime);
	public delegate void ProcessDelegate(float rate);
	public delegate void ResetDelegate();

	public virtual string ModuleName => "";
	public virtual string ModuleShortName => "SM";      // Inspector表示用なので原則2文字
	public virtual string TargetName => "TargetName";
	public virtual bool HasDuration => true;

	public virtual void Init() { }
	public virtual void Reset() { }
	public virtual void Process(float rate) { }

	public bool isEnable = true;
	public float startTime = 0f;
	public float endTime = 1f;
	public Tweener.EaseType easeType;
	
	public EvaluateDelegate cachedEvaluate { get; private set; }
	public ProcessDelegate cachedProcess { get; protected set; }
	public ResetDelegate cachedReset { get; private set; }

	float invDuration;
	bool initialized = false;
	public void InitModule()
	{
		cachedEvaluate = HasDuration ? DurationEvaluate : SingleEvaluate;
		cachedProcess = Process;
		cachedReset = Reset;

		if (HasDuration) {
			endTime = Mathf.Max(startTime, endTime);
			invDuration = endTime <= startTime ? 0f : 1f / (endTime - startTime);
		}	

		Tweener.CreateEaseTable(easeType);

		Init();

		initialized = true;
	}

	public void ResetModule()
	{
		if (!initialized)
			return;

		isApplied = false;
		
		cachedReset();
	}

	protected bool isApplied = false;
	void SingleEvaluate(float curTime)
	{
		if (!initialized || curTime < startTime || isApplied)
			return;

		isApplied = true;
		cachedProcess.Invoke(1f);
	}

	void DurationEvaluate(float curTime)
	{
		if (!initialized || curTime < startTime || curTime > endTime)
			return;

		float t = (curTime - startTime) * invDuration;
		float rate = Tweener.EaseFunction(easeType, t);
		cachedProcess.Invoke(rate);
	}

#if UNITY_EDITOR
	public virtual void DrawSequenceSingle(Rect rect, float duration)
	{
		float rectWidth = rect.width;

		EditorGUI.LabelField(rect, ModuleShortName);
		
		rect.x += 40;
		rect.y += 2;
		rect.width -= 90;

		if (HasDuration)		
			EditorGUI.MinMaxSlider(rect, ref startTime, ref endTime, 0f, duration);
		else
			startTime = GUI.HorizontalSlider(rect, startTime, 0f, duration);

		rect.x = rectWidth;
		rect.y -= 2;
		EditorGUI.LabelField(rect, TargetName);
	}

	public virtual void DrawSequenceDetail(float duration, float easeRate)
	{
		if (HasDuration) {

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space(5);
			startTime = Mathf.Clamp(EditorGUILayout.FloatField(startTime, GUILayout.Width(40)), 0f, duration);
			EditorGUILayout.Space(5);
			EditorGUILayout.MinMaxSlider(ref startTime, ref endTime, 0f, duration);
			EditorGUILayout.Space(5);
			endTime = Mathf.Clamp(EditorGUILayout.FloatField(endTime, GUILayout.Width(40)), startTime, duration);
			EditorGUILayout.Space(5);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			DrawEaseType(easeRate);
		}
		else {
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(58);
			startTime = EditorGUILayout.Slider(startTime, 0f, duration);
			EditorGUILayout.EndHorizontal();
		}
	}

	public void DrawEaseType(float easeRate)
	{
		var rect = EditorGUILayout.BeginHorizontal();
		EditorGUILayout.BeginVertical();
		EditorGUILayout.LabelField("Ease Type");
		easeType = (Tweener.EaseType)EditorGUILayout.EnumPopup(easeType);

		const int graphSize = 32;

		// Outline
		rect.x = rect.width - graphSize * 0.5f - 1;
		rect.y += 5;
		rect.width = graphSize + 3;
		rect.height = graphSize + 3;
		EditorGUI.DrawRect(rect, Color.black);

		// Panel
		rect.x++;
		rect.y++;
		rect.width = graphSize + 1;
		rect.height = graphSize + 1;
		EditorGUI.DrawRect(rect, new Color(0.164f, 0.164f, 0.164f));
		DrawEaseFunction(rect, graphSize, new Color(0.36f, 0.6f, 0.3f), easeType, easeRate);

		EditorGUILayout.EndVertical();
		EditorGUILayout.LabelField("");
		EditorGUILayout.EndHorizontal();
	}

	void DrawEaseFunction(Rect rect, int width, Color color, Tweener.EaseType easeType, float easeRate)
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
		var cursorX = (int)(easeRate * width);
		var cursorR = Tweener.EaseFunction(easeType, easeRate);
		int cursorY = (int)(cursorR * width);

		DrawPixel(rect, cursorX, cursorY, Color.red);
		DrawPixel(rect, cursorX - 1, cursorY, Color.red);
		DrawPixel(rect, cursorX + 1, cursorY, Color.red);
		DrawPixel(rect, cursorX, cursorY - 1, Color.red);
		DrawPixel(rect, cursorX, cursorY + 1, Color.red);
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
#endif
}
