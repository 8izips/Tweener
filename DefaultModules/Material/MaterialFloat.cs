using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class MaterialFloat : SequenceModule
{
	public static string ModulePath = "Material/Float";
	static readonly string moduleName = "Material Float";
	static readonly string moduleShortName = "MF";
	public override string ModuleName => moduleName;
	public override string ModuleShortName => moduleShortName;
	public override string TargetName => target == null ? "None" : target.name;

	public Material target;
	public string propertyName;
	public float startValue;
	public float endValue;

	Material cachedTarget;
	int propertyId;
	float originValue;
	float diffValue;

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
		originValue = cachedTarget.GetFloat(propertyId);
		diffValue = endValue - startValue;
		cachedProcess = Process;
	}

	public override void Reset()
	{
		if (cachedTarget == null)
			return;

		cachedTarget.SetFloat(propertyId, originValue);
	}

	public override void Process(float rate)
	{
		cachedTarget.SetFloat(propertyId, startValue + diffValue * rate);
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
		startValue = EditorGUILayout.FloatField("From", startValue);
		endValue = EditorGUILayout.FloatField("To", endValue);

		EditorGUILayout.Space();
		target = (Material)EditorGUILayout.ObjectField(target, typeof(Material), false);

		if (target != null && !string.IsNullOrEmpty(propertyName) && !target.HasProperty(propertyName))
			EditorGUILayout.HelpBox("Material does not contain the specified property.", MessageType.Warning);
	}
#endif
}
