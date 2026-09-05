using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class GameObjectEnable : SequenceModule
{
	public static string ModulePath = "GameObject/Enable";
	static readonly string moduleName = "GameObject Enable";
	static readonly string moduleShortName = "GE";
	public override string ModuleName => moduleName;
	public override string ModuleShortName => moduleShortName;
	public override string TargetName => target == null ? "None" : target.name;
	public override bool HasDuration => false;

	public GameObject target;
	public bool isActivating = true;

	bool originActive;
	bool lastActive;

	public override void Init()
	{
		if (!isEnable || target == null)
			return;

		cachedProcess = Process;

		originActive = target.activeSelf;
		lastActive = originActive;
	}

	public override void Reset()
	{
		if (target == null)
			return;

		target.SetActive(originActive);
		lastActive = originActive;
	}

	public override void Process(float rate)
	{
		if (rate < 1f)
			return;

		if (lastActive == isActivating)
			return;

		target.SetActive(isActivating);
		lastActive = isActivating;
	}

#if UNITY_EDITOR
	public override void DrawSequenceDetail(float duration, float easeRate)
	{
		base.DrawSequenceDetail(duration, easeRate);

		EditorGUILayout.Space();
		isActivating = EditorGUILayout.Toggle("Is Activating", isActivating);

		EditorGUILayout.Space();
		target = (GameObject)EditorGUILayout.ObjectField(target, typeof(GameObject), true);
	}
#endif
}