using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class MaterialColor : SequenceModule
{
	public static string ModulePath = "Material/Color";
	static readonly string moduleName = "Material Color";
	static readonly string moduleShortName = "MC";
	public override string ModuleName => moduleName;
	public override string ModuleShortName => moduleShortName;
	public override string TargetName => target == null ? "None" : target.name;

	public Material target;
	public Color startColor;
	public Color endColor;

	Material cachedTarget;
	Color originColor;
	Color diffColor;
	Color tempColor;

	public override void Init()
	{
		cachedTarget = null;
		cachedProcess = ProcessNone;

		if (!isEnable || target == null)
			return;

		cachedTarget = target;
		originColor = cachedTarget.color;
		diffColor = endColor - startColor;
		cachedProcess = Process;
	}

	public override void Reset()
	{
		if (cachedTarget == null)
			return;

		cachedTarget.color = originColor;
	}

	public override void Process(float rate)
	{
		tempColor.r = startColor.r + diffColor.r * rate;
		tempColor.g = startColor.g + diffColor.g * rate;
		tempColor.b = startColor.b + diffColor.b * rate;
		tempColor.a = startColor.a + diffColor.a * rate;
		cachedTarget.color = tempColor;
	}

	void ProcessNone(float rate)
	{
	}

#if UNITY_EDITOR
	public override void DrawSequenceDetail(float duration, float easeRate)
	{
		base.DrawSequenceDetail(duration, easeRate);

		EditorGUILayout.Space();
		startColor = EditorGUILayout.ColorField("From", startColor);
		endColor = EditorGUILayout.ColorField("To", endColor);

		EditorGUILayout.Space();
		target = (Material)EditorGUILayout.ObjectField(target, typeof(Material), false);
	}
#endif
}
