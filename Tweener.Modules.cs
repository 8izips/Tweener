using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public partial class Tweener : MonoBehaviour
{
	delegate SequenceModule ModuleCreator();
	static Dictionary<string, ModuleCreator> moduleCreators = new Dictionary<string, ModuleCreator>();
	public static List<string> modulePaths = new List<string>();

	static Tweener()
	{
		var baseType = typeof(SequenceModule);
		var assembly = baseType.Assembly;
		var assemblyTypes = assembly.GetTypes();

		for (int i = 0; i < assemblyTypes.Length; i++) {
			var assemblyType = assemblyTypes[i];
			if (assemblyType.IsAbstract || !baseType.IsAssignableFrom(assemblyType))
				continue;

			var field = assemblyType.GetField("ModulePath", BindingFlags.Static | BindingFlags.Public);
			if (field == null) {
				continue;
			}

			var modulePath = (string)field.GetValue(null);
			if (string.IsNullOrEmpty(modulePath))
				continue;
			
			if (moduleCreators.ContainsKey(modulePath)) {
				Debug.LogWarning($"Tweener : Duplicate module path detected ({modulePath})");
				continue;
			}
						
			var t = assemblyType; // キャプチャ�Jྡྷ
			moduleCreators[modulePath] = () => Activator.CreateInstance(t) as SequenceModule;
			modulePaths.Add(modulePath);
		}

		modulePaths.Sort();
	}

	public static SequenceModule CreateModule(int index)
	{
		if (index >= 0 && index < modulePaths.Count && moduleCreators.TryGetValue(modulePaths[index], out var creator)) {
			return creator.Invoke();
		}
		return null;
	}
}
