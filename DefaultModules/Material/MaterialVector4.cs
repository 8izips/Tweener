using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class MaterialVector4 : SequenceModule
{
	public static string ModulePath = "Material/Vector4";
	static readonly string moduleName = "Material Vector4";
	static readonly string moduleShortName = "MV";
	public override string ModuleName => moduleName;
	public override string ModuleShortName => moduleShortName;
	public override string TargetName => target == null ? "None" : target.name;

	public Material target;
	public string propertyName;
	public Vector4 startValue;
	public Vector4 endValue;

	Material cachedTarget;
	int propertyId;
	Vector4 originValue;
	Vector4 diffValue;
	Vector4 tempValue;

	public override void Init()
	{
		cachedTarget = null;
		cachedProcess = ProcessNone;

		if (!isEnable || target == null || string.IsNullOrEmpty(propertyName))
			return;

		propertyId = Shader.PropertyToID(propertyName);
		if (!target.HasProperty(propertyId))
			return;

		cachedTarget = target;
		originValue = cachedTarget.GetVector(propertyId);
		diffValue = endValue - startValue;
		cachedProcess = Process;
	}

	public override void Reset()
	{
		if (cachedTarget == null)
			return;

		cachedTarget.SetVector(propertyId, originValue);
	}

	public override void Process(float rate)
	{
		tempValue.x = startValue.x + diffValue.x * rate;
		tempValue.y = startValue.y + diffValue.y * rate;
		tempValue.z = startValue.z + diffValue.z * rate;
		tempValue.w = startValue.w + diffValue.w * rate;
		cachedTarget.SetVector(propertyId, tempValue);
	}

	void ProcessNone(float rate)
	{
	}

#if UNITY_EDITOR
	public override void DrawSequenceDetail(float duration, float easeRate)
	{
		base.DrawSequenceDetail(duration, easeRate);

		EditorGUILayout.Space();
		propertyName = EditorGUILayout.TextField("Property", propertyName);
		startValue = EditorGUILayout.Vector4Field("From", startValue);
		endValue = EditorGUILayout.Vector4Field("To", endValue);

		EditorGUILayout.Space();
		target = (Material)EditorGUILayout.ObjectField(target, typeof(Material), false);

		if (target != null && !string.IsNullOrEmpty(propertyName) && !target.HasProperty(propertyName))
			EditorGUILayout.HelpBox("Material does not contain the specified property.", MessageType.Warning);
	}
#endif
}
