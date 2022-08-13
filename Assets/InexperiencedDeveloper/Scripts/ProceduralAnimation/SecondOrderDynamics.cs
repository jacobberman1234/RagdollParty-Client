using UnityEngine;

public class SecondOrderDynamics
{
    private Vector3 xp;
    private Vector3 y, yd;
    private float k1, k2, k3;

    private const float PI = Mathf.PI;

    public SecondOrderDynamics(float f, float z, float r, Vector3 x0)
    {
        k1 = z / (PI * f);
        k2 = 1 / (Mathf.Pow((2 * PI * f), 2));
        k3 = r * z / (2 * PI * f);

        xp = x0;
        y = x0;
        yd = Vector3.zero;
    }

    public Vector3 Update(float T, Vector3 x, Vector3 xd)
    {
        if(xd == Vector3.zero)
        {
            xd = (x - xp) / T;
            xp = x;
        }
        y += (T * yd);
        yd += T * (x + (k3 * xd) - y - (k1 * yd)) / 2;
        return y;
    }
}
