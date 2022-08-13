using UnityEngine;

namespace InexperiencedDeveloper.Utils
{
    public class Helper
    {
        public static float Round(float value, int digits)
        {
            float mult = Mathf.Pow(10.0f, (float)digits);
            return Mathf.Round(value * mult) / mult;
        }
    }
}

