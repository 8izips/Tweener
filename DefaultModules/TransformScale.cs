using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class TransformScale : SequenceModule
{
	public static string ModulePath = "Transform/Scale";
	static readonly string moduleName = "Transform Scale";
	static readonly string moduleShortName = "TS";
	public override string ModuleName => moduleName;
	public override string ModuleShortName => moduleShortName;
	public override string TargetName => target == null ? "None" : target.name;

	public Transform target;
	public Vector3 startScale;
	public Vector3 endScale;
	public bool isRelative = false;

	Vector3 originScale { get; set; }
	public override void Init()
	{
		if (isEnable && target == null) {
			isEnable = false;
			return;
		}

		originScale = target.localScale;
	}

	public override void Reset()
	{
		target.localScale = originScale;
	}

	public override void Process(float rate)
	{
		var scale = FastLerp(startScale, endScale, rate);
		if (isRelative)
			target.localScale = originScale + scale;
		else
			target.localScale = scale;
	}

	Vector3 FastLerp(Vector3 a, Vector3 b, float t)
	{
		return new Vector3(
			a.x + (b.x - a.x) * t,
			a.y + (b.y - a.y) * t,
			a.z + (b.z - a.z) * t
		);
	}

#if UNITY_EDITOR
	public override void DrawSequenceDetail(float duration, float easeRate)
	{
		base.DrawSequenceDetail(duration, easeRate);

		EditorGUILayout.Space();
		startScale = EditorGUILayout.Vector3Field("From", startScale);
		endScale = EditorGUILayout.Vector3Field("To", endScale);
		isRelative = EditorGUILayout.Toggle("Is Relative", isRelative);

		EditorGUILayout.Space();
		target = (Transform)EditorGUILayout.ObjectField(target, typeof(Transform), true);
	}
#endif
}
