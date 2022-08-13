using System;
using UnityEngine;

namespace InexperiencedDeveloper.Utils
{
    public static class Ease
    {
        public static float EaseInOutSine(float start, float end, float val)
        {
            end -= start;
            return -end / 2f * (Mathf.Cos(MathUtils.CHEAP_PI * val / 1f) - 1f) + start;
        }
    }
}