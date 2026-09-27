using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class TextSize : SequenceModule
{
	public static string ModulePath = "Text/Size";
	static readonly string moduleName = "Text Size";
	static readonly string moduleShortName = "TS";
	public override string ModuleName => moduleName;
	public override string ModuleShortName => moduleShortName;
	public override string TargetName => target == null ? "None" : target.name;

	public Graphic target;
	public float startSize;
	public float endSize;

	Text textTarget;
	TMP_Text tmpTarget;
	float originSize;
	float diffSize;

	public override void Init()
	{
		textTarget = null;
		tmpTarget = null;
		cachedProcess = ProcessNone;

		if (!isEnable || target == null)
			return;

		diffSize = endSize - startSize;

		if (target is Text text) {
			textTarget = text;
			originSize = textTarget.fontSize;
			cachedProcess = ProcessText;
			return;
		}

		if (target is TMP_Text tmp) {
			tmpTarget = tmp;
			originSize = tmpTarget.fontSize;
			cachedProcess = ProcessTMP;
		}
	}

	public override void Reset()
	{
		if (textTarget != null) {
			textTarget.fontSize = Mathf.RoundToInt(originSize);
			return;
		}

		if (tmpTarget != null)
			tmpTarget.fontSize = originSize;
	}

	public override void Process(float rate)
	{
	}

	void ProcessText(float rate)
	{
		float size = startSize + diffSize * rate;
		textTarget.fontSize = Mathf.RoundToInt(size);
	}

	void ProcessTMP(float rate)
	{
		tmpTarget.fontSize = startSize + diffSize * rate;
	}

	void ProcessNone(float rate)
	{
	}

#if UNITY_EDITOR
	public override void DrawSequenceDetail(float duration, float easeRate)
	{
		base.DrawSequenceDetail(duration, easeRate);

		EditorGUILayout.Space();
		startSize = Mathf.Max(0f, EditorGUILayout.FloatField("From", startSize));
		endSize = Mathf.Max(0f, EditorGUILayout.FloatField("To", endSize));

		EditorGUILayout.Space();
		target = (Graphic)EditorGUILayout.ObjectField(target, typeof(Graphic), true);

		if (target != null && !(target is Text) && !(target is TMP_Text)) {
			EditorGUILayout.HelpBox("Target must be Unity UI Text or TMP_Text.", MessageType.Warning);
		}
		else if (target is Text text && text.resizeTextForBestFit) {
			EditorGUILayout.HelpBox("Text Size may be overridden while Best Fit is enabled.", MessageType.Warning);
		}
		else if (target is TMP_Text tmp && tmp.enableAutoSizing) {
			EditorGUILayout.HelpBox("Text Size may be overridden while Auto Size is enabled.", MessageType.Warning);
		}
	}
#endif
}
