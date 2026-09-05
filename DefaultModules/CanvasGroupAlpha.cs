using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class CanvasGroupAlpha : SequenceModule
{
	public static string ModulePath = "UI/CanvasGroup/Alpha";
	static readonly string moduleName = "UI/CanvasGroup Alpha";
	static readonly string moduleShortName = "CA";
	public override string ModuleName => moduleName;
	public override string ModuleShortName => moduleShortName;
	public override string TargetName => target == null ? "None" : target.name;

	public CanvasGroup target;
	public float startAlpha;
	public float endAlpha;

	float originAlpha { get; set; }
	public override void Init()
	{
		if (isEnable && target == null) {
			isEnable = false;
			return;
		}

		originAlpha = target.alpha;
	}

	public override void Reset()
	{
		target.alpha = originAlpha;
	}

	public override void Process(float rate)
	{
		var alpha = Mathf.Lerp(startAlpha, endAlpha, rate);
		target.alpha = alpha;
	}

#if UNITY_EDITOR
	public override void DrawSequenceDetail(float duration, float easeRate)
	{
		base.DrawSequenceDetail(duration, easeRate);

		EditorGUILayout.Space();
		startAlpha = EditorGUILayout.Slider("From", startAlpha, 0f, 1f);
		endAlpha = EditorGUILayout.Slider("To", endAlpha, 0f, 1f);

		EditorGUILayout.Space();
		target = (CanvasGroup)EditorGUILayout.ObjectField(target, typeof(CanvasGroup), true);
	}
#endif
}
