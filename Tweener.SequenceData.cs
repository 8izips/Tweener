using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class Tweener : MonoBehaviour
{
	public enum ModuleType
	{
		GameObjectEnable,
		TransformPosition,
		TransformRotation,
		TransformScale,
		RectTransformPosition,
		RectTransformRotation,
		RectTransformScale,
		MaterialColor,
		MaterialAlpha,
		CanvasGroupAlpha,
		ImageColor,
		ImageAlpha,
		TextColor,
		TextAlpha,
	}

	[System.Serializable]
	public class SequenceData
	{
		public ModuleType moduleType;

		public float startTime;
		public float endTime;

		public bool useCache;
		public bool isLocal;
		public Vector4 begin;
		public Vector4 end;
		public float beginAlpha;
		public float endAlpha;
		public bool targetEnable;

		public EaseType easeType;

		[SerializeField]
		public GameObject[] targets;

		// caches
		[System.NonSerialized]
		Vector4[] cachedOrigins;

		[System.NonSerialized]
		Transform[] cachedTransforms;

		[System.NonSerialized]
		RectTransform[] cachedRects;

		[System.NonSerialized]
		Material[] cachedMaterials;

		[System.NonSerialized]
		CanvasGroup[] cachedCanvasGroups;

		[System.NonSerialized]
		Image[] cachedImages;

		[System.NonSerialized]
		TMPro.TMP_Text[] cachedTexts;

		public void Init()
		{
			if (targets != null && targets.Length != 0) {
				switch (moduleType) {
					case ModuleType.GameObjectEnable:
					case ModuleType.TransformPosition:
					case ModuleType.TransformRotation:
					case ModuleType.TransformScale:
						cachedTransforms = new Transform[targets.Length];
						for (int i = 0; i < targets.Length; i++) {
							cachedTransforms[i] = targets[i].transform;
						}
						break;
					case ModuleType.RectTransformPosition:
					case ModuleType.RectTransformRotation:
					case ModuleType.RectTransformScale:
						cachedRects = new RectTransform[targets.Length];
						for (int i = 0; i < targets.Length; i++) {
							cachedRects[i] = targets[i].GetComponent<RectTransform>();
						}
						break;
					case ModuleType.MaterialColor:
					case ModuleType.MaterialAlpha:
						cachedMaterials = new Material[targets.Length];
						for (int i = 0; i < targets.Length; i++) {
							var renderer = targets[i].GetComponent<Renderer>();
							if (renderer)
								cachedMaterials[i] = renderer.sharedMaterial;
						}
						break;
					case ModuleType.CanvasGroupAlpha:
						cachedCanvasGroups = new CanvasGroup[targets.Length];
						for (int i = 0; i < targets.Length; i++) {
							cachedCanvasGroups[i] = targets[i].GetComponent<CanvasGroup>();
						}
						break;
					case ModuleType.ImageColor:
					case ModuleType.ImageAlpha:
						cachedImages = new Image[targets.Length];
						for (int i = 0; i < targets.Length; i++) {
							cachedImages[i] = targets[i].GetComponent<Image>();
						}
						break;
					case ModuleType.TextColor:
					case ModuleType.TextAlpha:
						cachedTexts = new TMPro.TMP_Text[targets.Length];
						for (int i = 0; i < targets.Length; i++) {
							cachedTexts[i] = targets[i].GetComponent<TMPro.TMP_Text>();
						}
						break;
				}

				ReleaseCaches(moduleType);

				cachedOrigins = new Vector4[targets.Length];
				switch (moduleType) {
					case ModuleType.GameObjectEnable:
						for (int i = 0; i < targets.Length; i++)
							cachedOrigins[i] = new Vector3(targets[i].activeSelf ? 1.0f : 0.0f, 0f, 0f);
						break;
					case ModuleType.TransformPosition:
						for (int i = 0; i < targets.Length; i++) {
							if (isLocal)
								cachedOrigins[i] = cachedTransforms[i].localPosition;
							else
								cachedOrigins[i] = cachedTransforms[i].position;
						}	
						break;
					case ModuleType.TransformRotation:
						for (int i = 0; i < targets.Length; i++) {
							if (isLocal)
								cachedOrigins[i] = cachedTransforms[i].localRotation.eulerAngles;
							else
								cachedOrigins[i] = cachedTransforms[i].rotation.eulerAngles;
						}
						break;
					case ModuleType.TransformScale:
						for (int i = 0; i < targets.Length; i++)
							cachedOrigins[i] = cachedTransforms[i].localScale;
						break;
					case ModuleType.RectTransformPosition:
						for (int i = 0; i < targets.Length; i++) {
							if (isLocal)
								cachedOrigins[i] = cachedRects[i].localPosition;
							else
								cachedOrigins[i] = cachedRects[i].position;
						}	
						break;
					case ModuleType.RectTransformRotation:
						for (int i = 0; i < targets.Length; i++) {
							if (isLocal)
								cachedOrigins[i] = cachedRects[i].localRotation.eulerAngles;
							else
								cachedOrigins[i] = cachedRects[i].rotation.eulerAngles;
						}	
						break;
					case ModuleType.RectTransformScale:
						for (int i = 0; i < targets.Length; i++)
							cachedOrigins[i] = cachedRects[i].localScale;
						break;
					case ModuleType.MaterialColor:
						for (int i = 0; i < targets.Length; i++)
							cachedOrigins[i] = new Vector4(cachedMaterials[i].color.r, cachedMaterials[i].color.g, cachedMaterials[i].color.b, cachedMaterials[i].color.a);
						break;
					case ModuleType.MaterialAlpha:
						for (int i = 0; i < targets.Length; i++)
							cachedOrigins[i] = new Vector3(cachedMaterials[i].color.a, 0f, 0f);
						break;
					case ModuleType.CanvasGroupAlpha:
						for (int i = 0; i < targets.Length; i++)
							cachedOrigins[i] = new Vector3(cachedCanvasGroups[i].alpha, 0f, 0f);
						break;
					case ModuleType.ImageColor:
						for (int i = 0; i < targets.Length; i++)
							cachedOrigins[i] = new Vector4(cachedImages[i].color.r, cachedImages[i].color.g, cachedImages[i].color.b, cachedImages[i].color.a);
						break;
					case ModuleType.ImageAlpha:
						for (int i = 0; i < targets.Length; i++)
							cachedOrigins[i] = new Vector3(cachedImages[i].color.a, 0f, 0f);
						break;
					case ModuleType.TextColor:
						for (int i = 0; i < targets.Length; i++)
							cachedOrigins[i] = new Vector4(cachedTexts[i].color.r, cachedTexts[i].color.g, cachedTexts[i].color.b, cachedTexts[i].alpha);
						break;
					case ModuleType.TextAlpha:
						for (int i = 0; i < targets.Length; i++)
							cachedOrigins[i] = new Vector3(cachedTexts[i].alpha, 0f, 0f);
						break;
				}
			}
		}

		void ReleaseCaches(ModuleType moduleType)
		{
			switch (moduleType) {
				case ModuleType.GameObjectEnable:
				case ModuleType.TransformPosition:
				case ModuleType.TransformRotation:
				case ModuleType.TransformScale:
					cachedRects = null;
					cachedMaterials = null;
					cachedCanvasGroups = null;
					cachedImages = null;
					cachedTexts = null;
					break;
				case ModuleType.RectTransformPosition:
				case ModuleType.RectTransformRotation:
				case ModuleType.RectTransformScale:
					cachedTransforms = null;
					cachedMaterials = null;
					cachedCanvasGroups = null;
					cachedImages = null;
					cachedTexts = null;
					break;
				case ModuleType.MaterialColor:
				case ModuleType.MaterialAlpha:
					cachedTransforms = null;
					cachedRects = null;
					cachedCanvasGroups = null;
					cachedImages = null;
					cachedTexts = null;
					break;
				case ModuleType.ImageColor:
				case ModuleType.ImageAlpha:
					cachedTransforms = null;
					cachedRects = null;
					cachedMaterials = null;
					cachedCanvasGroups = null;
					cachedTexts = null;
					break;
				case ModuleType.TextColor:
				case ModuleType.TextAlpha:
					cachedTransforms = null;
					cachedRects = null;
					cachedMaterials = null;
					cachedCanvasGroups = null;
					cachedImages = null;
					break;
			}
		}

		public void Update(float curTime)
		{
			if ((object)targets == null || targets.Length == 0)
				return;

			if (moduleType == ModuleType.GameObjectEnable) {
				if (curTime >= startTime) {
					for (int i = 0; i < targets.Length; i++)
						targets[i].SetActive(targetEnable);
				}
			}
			else {
				if (endTime == 0f)
					return;

				var rate = Evaluate(curTime);
				ApplyProgress(rate);
			}
		}

		public void End(bool isPingpong)
		{
			if (endTime == 0f || (object)targets == null || targets.Length == 0)
				return;

			ApplyProgress(isPingpong ? 0f : 1f);
		}

		public void Restore()
		{
			if ((object)targets == null || targets.Length == 0 || (object)cachedOrigins == null)
				return;

			switch (moduleType) {
				case ModuleType.GameObjectEnable:
					for (int i = 0; i < targets.Length; i++)
						targets[i].SetActive(cachedOrigins[i].x == 1f ? true : false);
					break;
				case ModuleType.TransformPosition:
					for (int i = 0; i < targets.Length; i++) {
						if (isLocal)
							cachedTransforms[i].localPosition = cachedOrigins[i];
						else
							cachedTransforms[i].position = cachedOrigins[i];
					}	
					break;
				case ModuleType.TransformRotation:
					for (int i = 0; i < targets.Length; i++) {
						if (isLocal)
							cachedTransforms[i].localRotation = Quaternion.Euler(cachedOrigins[i]);
						else
							cachedTransforms[i].rotation = Quaternion.Euler(cachedOrigins[i]);
					}	
					break;
				case ModuleType.TransformScale:
					for (int i = 0; i < targets.Length; i++)
						cachedTransforms[i].localScale = cachedOrigins[i];
					break;
				case ModuleType.RectTransformPosition:
					for (int i = 0; i < targets.Length; i++) {
						if (isLocal)
							cachedRects[i].localPosition = cachedOrigins[i];
						else
							cachedRects[i].position = cachedOrigins[i];
					}	
					break;
				case ModuleType.RectTransformRotation:
					for (int i = 0; i < targets.Length; i++) {
						if (isLocal)
							cachedRects[i].localRotation = Quaternion.Euler(cachedOrigins[i]);
						else
							cachedRects[i].rotation = Quaternion.Euler(cachedOrigins[i]);
					}	
					break;
				case ModuleType.RectTransformScale:
					for (int i = 0; i < targets.Length; i++)
						cachedRects[i].localScale = cachedOrigins[i];
					break;
				case ModuleType.MaterialColor:
					for (int i = 0; i < targets.Length; i++)
						cachedMaterials[i].SetColor("_Color", new Color(cachedOrigins[i].x, cachedOrigins[i].y, cachedOrigins[i].z, cachedOrigins[i].w));
					break;
				case ModuleType.MaterialAlpha:
					for (int i = 0; i < targets.Length; i++) {
						var color = cachedMaterials[i].color;
						color.a = cachedOrigins[i].x;
						cachedMaterials[i].SetColor("_Color", color);
					}
					break;
				case ModuleType.CanvasGroupAlpha:
					for (int i = 0; i < targets.Length; i++)
						cachedCanvasGroups[i].alpha = cachedOrigins[i].x;
					break;
				case ModuleType.ImageColor:
					for (int i = 0; i < targets.Length; i++)
						cachedImages[i].color = new Color(cachedOrigins[i].x, cachedOrigins[i].y, cachedOrigins[i].z, cachedOrigins[i].w);
					break;
				case ModuleType.ImageAlpha:
					for (int i = 0; i < targets.Length; i++) {
						var color = cachedImages[i].color;
						color.a = cachedOrigins[i].x;
						cachedImages[i].color = color;
					}
					break;
				case ModuleType.TextColor:
					for (int i = 0; i < targets.Length; i++)
						cachedTexts[i].color = new Color(cachedOrigins[i].x, cachedOrigins[i].y, cachedOrigins[i].z, cachedOrigins[i].w);
					break;
				case ModuleType.TextAlpha:
					for (int i = 0; i < targets.Length; i++) {
						cachedTexts[i].alpha = cachedOrigins[i].x;
					}
					break;
			}
		}

		void ApplyProgress(float rate)
		{
			if (rate < 0f)
				return;
				
			switch (moduleType) {
				case ModuleType.TransformPosition:
					var position = Vector3.Lerp(begin, end, rate);
					for (int i = 0; i < targets.Length; i++) {
						if (isLocal) {
							cachedTransforms[i].localPosition = position;
						}
						else {
							cachedTransforms[i].position = position;
						}
					}
					break;
				case ModuleType.TransformRotation:
					var rotX = Mathf.Lerp(begin.x, end.x, rate);
					var rotY = Mathf.Lerp(begin.y, end.y, rate);
					var rotZ = Mathf.Lerp(begin.z, end.z, rate);
					var rotation = Quaternion.Euler(rotX, rotY, rotZ);
					for (int i = 0; i < targets.Length; i++) {
						if (isLocal) {
							cachedTransforms[i].localRotation = rotation;
						}
						else {
							cachedTransforms[i].rotation = rotation;
						}
					}
					break;
				case ModuleType.TransformScale:
					for (int i = 0; i < targets.Length; i++) {
						var scale = Vector3.Lerp(begin, end, rate);
						cachedTransforms[i].localScale = scale;
					}
					break;
				case ModuleType.RectTransformPosition:
					var rectPosition = Vector3.Lerp(begin, end, rate);
					for (int i = 0; i < cachedRects.Length; i++) {
						if (isLocal) {
							cachedRects[i].localPosition = rectPosition;
						}
						else {
							cachedRects[i].position = rectPosition;
						}
					}
					break;
				case ModuleType.RectTransformRotation:
					var rectX = Mathf.Lerp(begin.x, end.x, rate);
					var rectY = Mathf.Lerp(begin.y, end.y, rate);
					var rectZ = Mathf.Lerp(begin.z, end.z, rate);
					var rectRotation = Quaternion.Euler(rectX, rectY, rectZ);
					for (int i = 0; i < targets.Length; i++) {
						if (isLocal) {
							cachedRects[i].localRotation = rectRotation;
						}
						else {
							cachedRects[i].rotation = rectRotation;
						}
					}
					break;
				case ModuleType.RectTransformScale:
					var rectScale = Vector3.Lerp(begin, end, rate);
					for (int i = 0; i < targets.Length; i++) {
						cachedRects[i].localScale = rectScale;
					}
					break;
				case ModuleType.MaterialColor: {
						var color = Color.Lerp(begin, end, rate);
						for (int i = 0; i < targets.Length; i++) {
							cachedMaterials[i].color = color;
						}
					}
					break;
				case ModuleType.MaterialAlpha: {
						var alpha = Mathf.Lerp(beginAlpha, endAlpha, rate);
						for (int i = 0; i < targets.Length; i++) {
							var color = cachedMaterials[i].color;
							color.a = alpha;
							cachedMaterials[i].color = color;
						}
					}
					break;
				case ModuleType.CanvasGroupAlpha: {
						var alpha = Mathf.Lerp(beginAlpha, endAlpha, rate);
						for (int i = 0; i < targets.Length; i++)
							cachedCanvasGroups[i].alpha = alpha;
					}
					break;
				case ModuleType.ImageColor: {
						var color = Color.Lerp(begin, end, rate);
						for (int i = 0; i < targets.Length; i++) {
							cachedImages[i].color = color;
						}
					}
					break;
				case ModuleType.ImageAlpha: {
						var alpha = Mathf.Lerp(beginAlpha, endAlpha, rate);
						for (int i = 0; i < targets.Length; i++) {
							var color = cachedImages[i].color;
							color.a = alpha;
							cachedImages[i].color = color;
						}
					}
					break;
				case ModuleType.TextColor: {
						var color = Color.Lerp(begin, end, rate);
						for (int i = 0; i < targets.Length; i++) {
							cachedTexts[i].color = color;
						}
					}
					break;
				case ModuleType.TextAlpha: {
						var alpha = Mathf.Lerp(beginAlpha, endAlpha, rate);
						for (int i = 0; i < targets.Length; i++) {
							cachedTexts[i].alpha = alpha;
						}
					}
					break;
			}
		}

		float Evaluate(float curTime)
		{
			if (curTime < startTime)
				return -1f;
			if (curTime == 0f)
				return 0f;
			if (curTime > endTime)
				return 1f;

			var progress = (curTime - startTime);
			var duration = (endTime - startTime);
			var t = progress / duration;

			return EaseFunction(easeType, t);
		}
	}
}
