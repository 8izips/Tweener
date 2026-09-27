using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class RectTransformSize : SequenceModule
{
	public static string ModulePath = "RectTransform/Size";
	static readonly string moduleName = "RectTransform Size";
	static readonly string moduleShortName = "RZ";
	public override string ModuleName => moduleName;
	public override string ModuleShortName => moduleShortName;
	public override string TargetName => target == null ? "None" : target.name;

	public RectTransform target;
	public Vector2 startSize;
	public Vector2 endSize;
	public bool isRelative = false;

	RectTransform cachedTarget;
	Vector2 originSize;
	Vector2 diffSize;
	Vector2 relativeStartSize;
	Vector2 tempSize;

	public override void Init()
	{
		cachedTarget = null;
		cachedProcess = ProcessNone;

		if (!isEnable || target == null)
			return;

		cachedTarget = target;
		originSize = cachedTarget.sizeDelta;
		diffSize = endSize - startSize;

		if (isRelative) {
			relativeStartSize = originSize + startSize;
			cachedProcess = ProcessRelative;
		}
		else {
			cachedProcess = Process;
		}
	}

	public override void Reset()
	{
		if (cachedTarget == null)
			return;

		cachedTarget.sizeDelta = originSize;
	}

	public override void Process(float rate)
	{
		tempSize.x = startSize.x + diffSize.x * rate;
		tempSize.y = startSize.y + diffSize.y * rate;
		cachedTarget.sizeDelta = tempSize;
	}

	void ProcessRelative(float rate)
	{
		tempSize.x = relativeStartSize.x + diffSize.x * rate;
		tempSize.y = relativeStartSize.y + diffSize.y * rate;
		cachedTarget.sizeDelta = tempSize;
	}

	void ProcessNone(float rate)
	{
	}

#if UNITY_EDITOR
	public override void DrawSequenceDetail(float duration, float easeRate)
	{
		base.DrawSequenceDetail(duration, easeRate);

		EditorGUILayout.Space();
		startSize = EditorGUILayout.Vector2Field("From", startSize);
		endSize = EditorGUILayout.Vector2Field("To", endSize);
		isRelative = EditorGUILayout.Toggle("Is Relative", isRelative);

		EditorGUILayout.Space();
		target = (RectTransform)EditorGUILayout.ObjectField(target, typeof(RectTransform), true);
	}
#endif
}
