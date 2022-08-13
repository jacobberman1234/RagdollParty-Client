using InexperiencedDeveloper.Utils.Log;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InexperiencedDeveloper.Extensions
{
    public static class QuaternionExtensions
    {
        public static Quaternion SafeLerp(this Quaternion quat, Quaternion a, Quaternion b, float t)
        {
            t = Mathf.Clamp01(t);
            if (a.x == 0 && a.y == 0 && a.z == 0 && a.w == 0)
            {
                Debug.LogWarning($"Invalid quaternion ({a}).");
                quat = Quaternion.identity;
            }
            else if (b.x == 0 && b.y == 0 && b.z == 0 && b.w == 0)
            {
                Debug.LogWarning($"Invalid quaternion ({a}).");
                quat = Quaternion.identity;
            }
            else
            {
               quat = Quaternion.Lerp(a, b, t);
            }
            return quat;
        }
    }
}

