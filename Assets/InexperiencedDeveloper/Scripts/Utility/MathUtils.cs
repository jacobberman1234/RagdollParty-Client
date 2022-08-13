using UnityEngine;

namespace InexperiencedDeveloper.Utils
{
    public static class MathUtils
    {
        public const float CHEAP_PI = 3.1415927f;
        public const float DEGREE_IN_RADIANS = 0.017453292f;

        public static float NonModuloWrap(float val, float size)
        {
            return val - Mathf.Floor(val / size) * size;
        }
    }
}

