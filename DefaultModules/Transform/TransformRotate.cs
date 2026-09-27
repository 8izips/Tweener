using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class TransformRotate : SequenceModule
{
	public static string ModulePath = "Transform/Rotate";
	static readonly string moduleName = "Transform Rotate";
	static readonly string moduleShortName = "TR";
	public override string ModuleName => moduleName;
	public override string ModuleShortName => moduleShortName;
	public override string TargetName => target == null ? "None" : target.name;

	public Transform target;
	public Vector3 startRotation;
	public Vector3 endRotation;
	public bool isLocal = false;
	public bool isRelative = false;

	Vector3 originRot;
	Vector3 diffRot;
	Vector3 relativeStartRot;
	Vector3 tempRot;

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

		originRot = isLocal ? target.localEulerAngles : target.eulerAngles;
		diffRot = endRotation - startRotation;

		if (isRelative)
			relativeStartRot = originRot + startRotation;
	}

	public override void Reset()
	{
		if (target == null)
			return;

		if (isLocal)
			target.localEulerAngles = originRot;
		else
			target.eulerAngles = originRot;
	}

	void ProcessWorld(float rate)
	{
		tempRot.x = startRotation.x + diffRot.x * rate;
		tempRot.y = startRotation.y + diffRot.y * rate;
		tempRot.z = startRotation.z + diffRot.z * rate;
		target.eulerAngles = tempRot;
	}

	void ProcessLocal(float rate)
	{
		tempRot.x = startRotation.x + diffRot.x * rate;
		tempRot.y = startRotation.y + diffRot.y * rate;
		tempRot.z = startRotation.z + diffRot.z * rate;
		target.localEulerAngles = tempRot;
	}

	void ProcessWorldRelative(float rate)
	{
		tempRot.x = relativeStartRot.x + diffRot.x * rate;
		tempRot.y = relativeStartRot.y + diffRot.y * rate;
		tempRot.z = relativeStartRot.z + diffRot.z * rate;
		target.eulerAngles = tempRot;
	}

	void ProcessLocalRelative(float rate)
	{
		tempRot.x = relativeStartRot.x + diffRot.x * rate;
		tempRot.y = relativeStartRot.y + diffRot.y * rate;
		tempRot.z = relativeStartRot.z + diffRot.z * rate;
		target.localEulerAngles = tempRot;
	}

#if UNITY_EDITOR
	public override void DrawSequenceDetail(float duration, float easeRate)
	{
		base.DrawSequenceDetail(duration, easeRate);

		EditorGUILayout.Space();
		startRotation = EditorGUILayout.Vector3Field("From", startRotation);
		endRotation = EditorGUILayout.Vector3Field("To", endRotation);
		isLocal = EditorGUILayout.Toggle("Is Local", isLocal);
		isRelative = EditorGUILayout.Toggle("Is Relative", isRelative);

		EditorGUILayout.Space();
		target = (Transform)EditorGUILayout.ObjectField(
			target,
			typeof(Transform),
			true
		);
	}
#endif
}