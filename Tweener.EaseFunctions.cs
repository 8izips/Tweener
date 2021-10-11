using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Tweener : MonoBehaviour
{
    public enum EaseType
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseSpring,
        EaseInBack,
        EaseOutBack,
        EaseInBounce,
        EaseOutBounce,
        EaseInOutBounce,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic,
        EaseStep1,
        EaseStep4,
        EaseStep8,
        EaseStep16,
    }

    public static float EaseFunction(EaseType easeType, float t)
    {
        switch (easeType) {
            case EaseType.Linear:
                return EaseLinear(t);
            case EaseType.EaseInQuad:
                return EaseInQuad(t);
            case EaseType.EaseOutQuad:
                return EaseOutQuad(t);
            case EaseType.EaseInOutQuad:
                return EaseInOutQuad(t);
            case EaseType.EaseInCubic:
                return EaseInCubic(t);
            case EaseType.EaseOutCubic:
                return EaseOutCubic(t);
            case EaseType.EaseInOutCubic:
                return EaseInOutCubic(t);
            case EaseType.EaseSpring:
                return EaseSpring(t);
            case EaseType.EaseInBack:
                return EaseInBack(t);
            case EaseType.EaseOutBack:
                return EaseOutBack(t);
            case EaseType.EaseInBounce:
                return EaseInBounce(t);
            case EaseType.EaseOutBounce:
                return EaseOutBounce(t);
            case EaseType.EaseInOutBounce:
                return EaseInOutBounce(t);
            case EaseType.EaseInElastic:
                return EaseInElastic(t);
            case EaseType.EaseOutElastic:
                return EaseOutElastic(t);
            case EaseType.EaseInOutElastic:
                return EaseInOutElastic(t);
            case EaseType.EaseStep1:
                return EaseStep1(t);
            case EaseType.EaseStep4:
                return EaseStep4(t);
            case EaseType.EaseStep8:
                return EaseStep8(t);
            case EaseType.EaseStep16:
                return EaseStep16(t);
        }

        return -1f;
    }

    public static float EaseLinear(float t)
    {
        return t;
    }

    public static float EaseInQuad(float t)
    {
        return t * t;
    }

    public static float EaseOutQuad(float t)
    {
        return 1f - (1f - t) * (1f - t);
    }

    public static float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2 * t * t : 1 - (-2f * t + 2) * (-2f * t + 2) * 0.5f;
    }

    public static float EaseInCubic(float t)
    {
        return t * t * t;
    }

    public static float EaseOutCubic(float t)
    {
        return 1f - (1f - t) * (1f - t) * (1f - t);
    }

    public static float EaseInOutCubic(float t)
    {
        return t < 0.5f ? 4 * t * t * t : 1 - (-2f * t + 2) * (-2f * t + 2) * (-2f * t + 2) * 0.5f;
    }

    public static float EaseSpring(float t)
    {
        return (Mathf.Sin(t * Mathf.PI * (0.2f + 2.5f * t * t * t)) * Mathf.Pow(1f - t, 2.2f) + t) * (1f + (1.2f * (1f - t)));
    }

    public static float EaseInBack(float t)
    {
        return 2.70158f * t * t * t - 1.70158f * t * t;
    }

    public static float EaseOutBack(float t)
    {
        var it = t - 1f;
        return 1f + 2.70158f * it * it * it + 1.70158f * it * it;
    }

    public static float EaseInBounce(float t)
    {
        return 1f - EaseOutBounce(1f - t);
    }

    public static float EaseOutBounce(float t)
    {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        if (t < 1f / d1) {
            return n1 * t * t;
        }
        else if (t < 2f / d1) {
            t -= 1.5f / d1;
            return n1 * t * t + 0.75f;
        }
        else if (t < 2.5f / d1) {
            t -= 2.25f / d1;
            return n1 * t * t + 0.9375f;
        }
        else {
            t -= 2.625f / d1;
            return n1 * t * t + 0.984375f;
        }
    }

    public static float EaseInOutBounce(float t)
    {
        return t < 0.5f
            ? (1 - EaseOutBounce(1f - 2 * t)) * 0.5f
            : (1 + EaseOutBounce(2f * t - 1)) * 0.5f;
    }

    public static float EaseInElastic(float t)
    {
        const float c4 = 2f * Mathf.PI / 3f;
        if (t < Mathf.Epsilon)
            return 0f;
        if (t > 1f - Mathf.Epsilon)
            return 1f;

        return -Mathf.Pow(2f, 10f * t - 10f) * Mathf.Sin((t * 10f - 10.75f) * c4);
    }

    public static float EaseOutElastic(float t)
    {
        const float c4 = 2f * Mathf.PI / 3f;
        if (t < Mathf.Epsilon)
            return 0f;
        if (t > 1f - Mathf.Epsilon)
            return 1f;

        return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
    }

    public static float EaseInOutElastic(float t)
    {
        const float c5 = 2f * Mathf.PI / 4.5f;
        if (t < Mathf.Epsilon)
            return 0f;
        if (t > 1f - Mathf.Epsilon)
            return 1f;

        return t < 0.5f
            ? -Mathf.Pow(2f, 20f * t - 10f) * Mathf.Sin((t * 20f - 11.125f) * c5) * 0.5f
            : Mathf.Pow(2f, -20f * t + 10f) * Mathf.Sin((t * 20f - 11.125f) * c5) * 0.5f + 1f;
    }

    public static float EaseStep1(float t)
    {
        return t <= 0.5f ? 0f : 1f;
    }

    public static float EaseStep4(float t)
    {
        if (t <= 0.25f)
            return 0f;
        if (t <= 0.5f)
            return 0.333333f;
        if (t <= 0.75f)
            return 0.666667f;
        return 1f;
    }

    public static float EaseStep8(float t)
    {
        if (t <= 0.125f)
            return 0f;
        if (t <= 0.250f)
            return 0.142857f;
        if (t <= 0.375f)
            return 0.285714f;
        if (t <= 0.500f)
            return 0.428571f;
        if (t <= 0.625f)
            return 0.571429f;
        if (t <= 0.750f)
            return 0.714286f;
        if (t <= 0.875f)
            return 0.857143f;
        return 1f;
    }

    public static float EaseStep16(float t)
    {
        if (t <= 0.0625f)
            return 0f;
        if (t <= 0.1250f)
            return 0.066667f;
        if (t <= 0.1875f)
            return 0.133333f;
        if (t <= 0.2500f)
            return 0.2f;
        if (t <= 0.3125f)
            return 0.266667f;
        if (t <= 0.3750f)
            return 0.333333f;
        if (t <= 0.4375f)
            return 0.4f;
        if (t <= 0.5000f)
            return 0.466667f;
        if (t <= 0.5625f)
            return 0.533333f;
        if (t <= 0.6250f)
            return 0.6f;
        if (t <= 0.6875f)
            return 0.666667f;
        if (t <= 0.7500f)
            return 0.733333f;
        if (t <= 0.8125f)
            return 0.8f;
        if (t <= 0.8750f)
            return 0.866667f;
        if (t <= 0.9375f)
            return 0.933333f;
        return 1f;
    }
}