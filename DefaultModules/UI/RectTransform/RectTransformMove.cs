using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class RectTransformMove : SequenceModule
{
	public static string ModulePath = "RectTransform/Move";
	static readonly string moduleName = "RectTransform Move";
	static readonly string moduleShortName = "RTM";
	public override string ModuleName => moduleName;
	public override string ModuleShortName => moduleShortName;
	public override string TargetName => target == null ? "None" : target.name;

	public RectTransform target;
	public Vector2 startPosition;
	public Vector2 endPosition;
	public bool isRelative = false;

	Vector2 originPos;
	Vector2 diffPos;
	Vector2 relativeStartPos;
	Vector2 tempPos;

	public override void Init()
	{
		if (!isEnable || target == null)
			return;

		cachedProcess = isRelative ? ProcessRelative : Process;

		originPos = target.anchoredPosition;
		diffPos = endPosition - startPosition;

		if (isRelative)
			relativeStartPos = originPos + startPosition;
	}

	public override void Reset()
	{
		if (target == null)
			return;

		target.anchoredPosition = originPos;
	}

	public override void Process(float rate)
	{
		tempPos.x = startPosition.x + diffPos.x * rate;
		tempPos.y = startPosition.y + diffPos.y * rate;
		target.anchoredPosition = tempPos;
	}

	void ProcessRelative(float rate)
	{
		tempPos.x = relativeStartPos.x + diffPos.x * rate;
		tempPos.y = relativeStartPos.y + diffPos.y * rate;
		target.anchoredPosition = tempPos;
	}

#if UNITY_EDITOR
	public override void DrawSequenceDetail(float duration, float easeRate)
	{
		base.DrawSequenceDetail(duration, easeRate);

		EditorGUILayout.Space();
		startPosition = EditorGUILayout.Vector2Field("From", startPosition);
		endPosition = EditorGUILayout.Vector2Field("To", endPosition);
		isRelative = EditorGUILayout.Toggle("Is Relative", isRelative);

		EditorGUILayout.Space();
		target = (RectTransform)EditorGUILayout.ObjectField(
			target,
			typeof(RectTransform),
			true
		);
	}
#endif
}