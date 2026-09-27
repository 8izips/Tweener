using System;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class ImageColor : SequenceModule
{
	public static string ModulePath = "UI/Image/Color";
	static readonly string moduleName = "UI/Image Color";
	static readonly string moduleShortName = "IC";
	public override string ModuleName => moduleName;
	public override string ModuleShortName => moduleShortName;
	public override string TargetName => target == null ? "None" : target.name;

	public Image target;
	public Color startColor;
	public Color endColor;

	Image cachedTarget;
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
		target = (Image)EditorGUILayout.ObjectField(target, typeof(Image), true);
	}
#endif
}
