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
        ImageColor,
        ImageAlpha,
    }

    [System.Serializable]
    public class SequenceData
    {
        public ModuleType moduleType;

        public float startTime;
        public float endTime;

        public bool useCache;
        public bool isRelative;
        public Vector3 begin;
        public Vector3 end;
        public Color beginColor;
        public Color endColor;
        public float beginAlpha;
        public float endAlpha;
        public bool beginEnable;
        public bool endEnable;

        public EaseType easeType;

        [SerializeField]
        public GameObject[] targets;

        // caches
        [System.NonSerialized]
        Vector3[] cachedOrigins;

        [System.NonSerialized]
        Transform[] cachedTransforms;

        [System.NonSerialized]
        RectTransform[] cachedRects;

        [System.NonSerialized]
        Image[] cachedImages;

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
                        cachedRects = null;
                        cachedImages = null;
                        break;
                    case ModuleType.RectTransformPosition:
                    case ModuleType.RectTransformRotation:
                    case ModuleType.RectTransformScale:
                        cachedTransforms = null;
                        cachedRects = new RectTransform[targets.Length];
                        for (int i = 0; i < targets.Length; i++) {
                            cachedRects[i] = targets[i].GetComponent<RectTransform>();
                        }
                        cachedImages = null;
                        break;
                    case ModuleType.ImageColor:
                    case ModuleType.ImageAlpha:
                        cachedTransforms = null;
                        cachedRects = null;
                        cachedImages = new Image[targets.Length];
                        for (int i = 0; i < targets.Length; i++) {
                            cachedImages[i] = targets[i].GetComponent<Image>();
                        }
                        break;
                }

                switch (moduleType) {
                    case ModuleType.GameObjectEnable:
                        isRelative = false;
                        break;
                }

                cachedOrigins = new Vector3[targets.Length];
                switch (moduleType) {
                    case ModuleType.GameObjectEnable:
                        for (int i = 0; i < targets.Length; i++)
                            cachedOrigins[i] = new Vector3(targets[i].activeSelf ? 1.0f : 0.0f, 0f, 0f);
                        break;
                    case ModuleType.TransformPosition:
                        for (int i = 0; i < targets.Length; i++)
                            cachedOrigins[i] = cachedTransforms[i].localPosition;
                        break;
                    case ModuleType.TransformRotation:
                        for (int i = 0; i < targets.Length; i++)
                            cachedOrigins[i] = cachedTransforms[i].localRotation.eulerAngles;
                        break;
                    case ModuleType.TransformScale:
                        for (int i = 0; i < targets.Length; i++)
                            cachedOrigins[i] = cachedTransforms[i].localScale;
                        break;
                    case ModuleType.RectTransformPosition:
                        for (int i = 0; i < targets.Length; i++)
                            cachedOrigins[i] = cachedRects[i].localPosition;
                        break;
                    case ModuleType.RectTransformRotation:
                        for (int i = 0; i < targets.Length; i++)
                            cachedOrigins[i] = cachedRects[i].localRotation.eulerAngles;
                        break;
                    case ModuleType.RectTransformScale:
                        for (int i = 0; i < targets.Length; i++)
                            cachedOrigins[i] = cachedRects[i].localScale;
                        break;
                    case ModuleType.ImageColor:
                        for (int i = 0; i < targets.Length; i++)
                            cachedOrigins[i] = new Vector3(cachedImages[i].color.r, cachedImages[i].color.g, cachedImages[i].color.b);
                        break;
                    case ModuleType.ImageAlpha:
                        for (int i = 0; i < targets.Length; i++)
                            cachedOrigins[i] = new Vector3(cachedImages[i].color.a, 0f, 0f);
                        break;
                }
            }
        }

        public void Update(float curTime)
        {
            if (endTime == 0f || (object)targets == null || targets.Length == 0)
                return;

            var rate = Evaluate(curTime);
            ApplyProgress(rate);
        }

        public void End()
        {
            if (endTime == 0f || (object)targets == null || targets.Length == 0)
                return;

            ApplyProgress(1f);
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
                    for (int i = 0; i < targets.Length; i++)
                        cachedTransforms[i].localPosition = cachedOrigins[i];
                    break;
                case ModuleType.TransformRotation:
                    for (int i = 0; i < targets.Length; i++)
                        cachedTransforms[i].localRotation = Quaternion.Euler(cachedOrigins[i]);
                    break;
                case ModuleType.TransformScale:
                    for (int i = 0; i < targets.Length; i++)
                        cachedTransforms[i].localScale = cachedOrigins[i];
                    break;
                case ModuleType.RectTransformPosition:
                    for (int i = 0; i < targets.Length; i++)
                        cachedRects[i].localPosition = cachedOrigins[i];
                    break;
                case ModuleType.RectTransformRotation:
                    for (int i = 0; i < targets.Length; i++)
                        cachedRects[i].localRotation = Quaternion.Euler(cachedOrigins[i]);
                    break;
                case ModuleType.RectTransformScale:
                    for (int i = 0; i < targets.Length; i++)
                        cachedRects[i].localScale = cachedOrigins[i];
                    break;
                case ModuleType.ImageColor:
                    for (int i = 0; i < targets.Length; i++)
                        cachedImages[i].color = new Color(cachedOrigins[i].x, cachedOrigins[i].y, cachedOrigins[i].z);
                    break;
                case ModuleType.ImageAlpha:
                    for (int i = 0; i < targets.Length; i++) {
                        var color = cachedImages[i].color;
                        color.a = cachedOrigins[i].x;
                        cachedImages[i].color = color;
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
                        if (isRelative) {
                            cachedTransforms[i].localPosition = cachedOrigins[i] + position;
                        }
                        else {
                            cachedTransforms[i].localPosition = position;
                        }
                    }
                    break;
                case ModuleType.TransformRotation:
                    var rotX = Mathf.Lerp(begin.x, end.x, rate);
                    var rotY = Mathf.Lerp(begin.y, end.y, rate);
                    var rotZ = Mathf.Lerp(begin.z, end.z, rate);
                    var rotation = Quaternion.Euler(rotX, rotY, rotZ);
                    for (int i = 0; i < targets.Length; i++) {
                        if (isRelative) {
                            cachedTransforms[i].localRotation = Quaternion.Euler(cachedOrigins[i]) * rotation;
                        }
                        else {
                            cachedTransforms[i].localRotation = rotation;
                        }
                    }
                    break;
                case ModuleType.TransformScale:
                    if (isRelative) {
                        for (int i = 0; i < targets.Length; i++) {
                            var scale = Vector3.Lerp(cachedOrigins[i], end, rate);
                            cachedTransforms[i].localScale = scale;
                        }
                    }
                    else {
                        var scale = Vector3.Lerp(begin, end, rate);
                        for (int i = 0; i < targets.Length; i++) {
                            cachedTransforms[i].localScale = scale;
                        }
                    }
                    break;
                case ModuleType.RectTransformPosition:
                    var rectPosition = Vector3.Lerp(begin, end, rate);
                    for (int i = 0; i < cachedRects.Length; i++) {
                        if (isRelative) {
                            cachedRects[i].localPosition = cachedOrigins[i] + rectPosition;
                        }
                        else {
                            cachedRects[i].localPosition = rectPosition;
                        }
                    }
                    break;
                case ModuleType.RectTransformRotation:
                    var rectX = Mathf.Lerp(begin.x, end.x, rate);
                    var rectY = Mathf.Lerp(begin.y, end.y, rate);
                    var rectZ = Mathf.Lerp(begin.z, end.z, rate);
                    var rectRotation = Quaternion.Euler(rectX, rectY, rectZ);
                    for (int i = 0; i < targets.Length; i++) {
                        if (isRelative) {
                            cachedRects[i].localRotation = Quaternion.Euler(cachedOrigins[i]) * rectRotation;
                        }
                        else {
                            cachedRects[i].localRotation = rectRotation;
                        }
                    }
                    break;
                case ModuleType.RectTransformScale:
                    if (isRelative) {
                        for (int i = 0; i < targets.Length; i++) {
                            var rectScale = Vector3.Lerp(cachedOrigins[i], end, rate);
                            cachedRects[i].localScale = rectScale;
                        }
                    }
                    else {
                        var rectScale = Vector3.Lerp(begin, end, rate);
                        for (int i = 0; i < targets.Length; i++) {
                            cachedRects[i].localScale = rectScale;
                        }
                    }
                    break;
                case ModuleType.ImageColor:
                    if (isRelative) {
                        for (int i = 0; i < targets.Length; i++) {
                            var originColor = new Color(cachedOrigins[i].x, cachedOrigins[i].y, cachedOrigins[i].z);
                            var color = Color.Lerp(originColor, endColor, rate);
                            cachedImages[i].color = color;
                        }
                    }
                    else {
                        var color = Color.Lerp(beginColor, endColor, rate);
                        for (int i = 0; i < targets.Length; i++) {
                            cachedImages[i].color = color;
                        }
                    }
                    break;
                case ModuleType.ImageAlpha:
                    if (isRelative) {
                        for (int i = 0; i < targets.Length; i++) {
                            var alpha = Mathf.Lerp(cachedOrigins[i].x, endAlpha, rate);
                            var color = cachedImages[i].color;
                            color.a = alpha;
                            cachedImages[i].color = color;
                        }
                    }
                    else {
                        var alpha = Mathf.Lerp(beginAlpha, endAlpha, rate);
                        for (int i = 0; i < targets.Length; i++) {
                            var color = cachedImages[i].color;
                            color.a = alpha;
                            cachedImages[i].color = color;
                        }
                    }
                    break;
            }
        }

        float Evaluate(float curTime)
        {
            if (curTime < startTime)
                return -1f;
            if (curTime > endTime)
                return -1f;

            var progress = (curTime - startTime);
            var duration = (endTime - startTime);
            var t = progress / duration;

            return EaseFunction(easeType, t);
        }
    }
}
