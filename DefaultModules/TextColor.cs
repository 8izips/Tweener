using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class TextColor : SequenceModule
{
	public static string ModulePath = "Text/Color";
	static readonly string moduleName = "Text Color";
	static readonly string moduleShortName = "TC";
	public override string ModuleName => moduleName;
	public override string ModuleShortName => moduleShortName;
	public override string TargetName => target == null ? "None" : target.name;

	public Graphic target;
	public Color startColor;
	public Color endColor;

	Graphic cachedTarget;
	Color originColor;
	Color diffColor;
	Color tempColor;

	public override void Init()
	{
		cachedTarget = null;
		cachedProcess = ProcessNone;

		if (!isEnable || target == null)
			return;

		if (!(target is Text) && !(target is TMP_Text))
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
		target = (Graphic)EditorGUILayout.ObjectField(target, typeof(Graphic), true);

		if (target != null && !(target is Text) && !(target is TMP_Text))
			EditorGUILayout.HelpBox("Target must be Unity UI Text or TMP_Text.", MessageType.Warning);
	}
#endif
}
