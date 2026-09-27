using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class RectTransformRotate : SequenceModule
{
	public static string ModulePath = "RectTransform/Rotate";
	static readonly string moduleName = "RectTransform Rotate";
	static readonly string moduleShortName = "RR";
	public override string ModuleName => moduleName;
	public override string ModuleShortName => moduleShortName;
	public override string TargetName => target == null ? "None" : target.name;

	public RectTransform target;
	public Vector3 startRotation;
	public Vector3 endRotation;
	public bool isLocal = false;
	public bool isRelative = false;

	Quaternion originRot { get; set; }
	public override void Init()
	{
		if (isEnable && target == null) {
			isEnable = false;
			return;
		}

		if (isLocal)
			originRot = target.localRotation;
		else
			originRot = target.rotation;
	}

	public override void Reset()
	{
		if (isLocal)
			target.localRotation = originRot;
		else
			target.rotation = originRot;
	}

	public override void Process(float rate)
	{
		var rotation = Quaternion.Lerp(Quaternion.Euler(startRotation), Quaternion.Euler(endRotation), rate);
		if (isLocal) {
			if (isRelative)
				target.localRotation = originRot * rotation;
			else
				target.localRotation = rotation;

		}
		else {
			if (isRelative)
				target.rotation = originRot * rotation;
			else
				target.rotation = rotation;
		}
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
		target = (RectTransform)EditorGUILayout.ObjectField(target, typeof(Transform), true);
	}
#endif
}
