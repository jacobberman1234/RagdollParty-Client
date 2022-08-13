using InexperiencedDeveloper.Utils.Log;
using UnityEngine;

/// <summary>
/// A lot of inspiration from the guys at Human Fall Flat
/// </summary>

namespace InexperiencedDeveloper.Extensions
{
    public static class RigidbodyExtensions
    {
        public static void SafeAddForce(this Rigidbody rb, Vector3 force, ForceMode mode = ForceMode.Force)
        {
            if(float.IsNaN(force.x) || float.IsNaN(force.y) || float.IsNaN(force.z) || force.sqrMagnitude > 2.5E+11f)
            {
                DebugLogger.LogWarning($"Invalid force ({force}) for {rb.name}.");
            }
            else
            {
                rb.AddForce(force, mode);
            }
        }

        public static void SafeAddForceAtPosition(this Rigidbody rb, Vector3 force, Vector3 pos, ForceMode mode = ForceMode.Force)
        {
            if (float.IsNaN(force.x) || float.IsNaN(force.y) || float.IsNaN(force.z) || force.sqrMagnitude > 2.5E+11f)
            {
                DebugLogger.LogWarning($"Invalid force ({force}) for {rb.name}.");
            }
            else if(pos.x == float.NaN || pos.y == float.NaN || pos.z == float.NaN || pos.sqrMagnitude > 2.5E+11f)
            {
                DebugLogger.LogWarning($"Invalid positon ({pos}) for {rb.name}.");
            }
            else
            {
                rb.AddForceAtPosition(force, pos, mode);
            }
        }

        public static void SafeAddTorque(this Rigidbody rb, Vector3 torque, ForceMode mode = ForceMode.Force)
        {
            if (float.IsNaN(torque.x) || float.IsNaN(torque.y) || float.IsNaN(torque.z) || torque.sqrMagnitude > 1E+10f)
            {
                DebugLogger.LogWarning($"Invalid torque ({torque}) for {rb.name}.");
            }
            else
            {
                rb.AddTorque(torque, mode);
            }
        }
    }
}

