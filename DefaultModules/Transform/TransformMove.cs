using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class TransformMove : SequenceModule
{
	public static string ModulePath = "Transform/Move";
	static readonly string moduleName = "Transform Move";
	static readonly string moduleShortName = "TM";
	public override string ModuleName => moduleName;
	public override string ModuleShortName => moduleShortName;
	public override string TargetName => target == null ? "None" : target.name;

	public Transform target;
	public Vector3 startPosition;
	public Vector3 endPosition;
	public bool isLocal = false;
	public bool isRelative = false;

	Vector3 originPos;
	Vector3 diffPos;
	Vector3 relativeStartPos;
	Vector3 tempPos;
	public override void Init() 
	{
		if (!isEnable || target == null)
			return;

		if (isLocal) {
			cachedProcess = isRelative ? ProcessLocalRelative : ProcessLocal;
		}
		else {
			cachedProcess = isRelative ? ProcessWorldRelative : ProcessWorld;
		}

		// 現在のTransform状態をOriginとしてキャッシュ
		originPos = isLocal ? target.localPosition : target.position;
		diffPos = endPosition - startPosition;

		if (isRelative)
			relativeStartPos = originPos + startPosition;
	}

	// Init時に取得したOriginへ戻す
	public override void Reset()
	{
		if (target == null)
			return;
		
		if (isLocal)
			target.localPosition = originPos;
		else
			target.position = originPos;
	}

	void ProcessWorld(float rate)
	{
		tempPos.x = startPosition.x + diffPos.x * rate;
		tempPos.y = startPosition.y + diffPos.y * rate;
		tempPos.z = startPosition.z + diffPos.z * rate;
		target.position = tempPos;
	}

	void ProcessLocal(float rate)
	{
		tempPos.x = startPosition.x + diffPos.x * rate;
		tempPos.y = startPosition.y + diffPos.y * rate;
		tempPos.z = startPosition.z + diffPos.z * rate;
		target.localPosition = tempPos;
	}

	void ProcessWorldRelative(float rate)
	{
		tempPos.x = relativeStartPos.x + diffPos.x * rate;
		tempPos.y = relativeStartPos.y + diffPos.y * rate;
		tempPos.z = relativeStartPos.z + diffPos.z * rate;
		target.position = tempPos;
	}

	void ProcessLocalRelative(float rate)
	{
		tempPos.x = relativeStartPos.x + diffPos.x * rate;
		tempPos.y = relativeStartPos.y + diffPos.y * rate;
		tempPos.z = relativeStartPos.z + diffPos.z * rate;
		target.localPosition = tempPos;
	}

#if UNITY_EDITOR
	public override void DrawSequenceDetail(float duration, float easeRate)
	{
		base.DrawSequenceDetail(duration, easeRate);

		EditorGUILayout.Space();
		startPosition = EditorGUILayout.Vector3Field("From", startPosition);
		endPosition = EditorGUILayout.Vector3Field("To", endPosition);
		isLocal = EditorGUILayout.Toggle("Is Local", isLocal);
		isRelative = EditorGUILayout.Toggle("Is Relative", isRelative);
		
		EditorGUILayout.Space();
		target = (Transform)EditorGUILayout.ObjectField(target, typeof(Transform), true);
	}
#endif
}
