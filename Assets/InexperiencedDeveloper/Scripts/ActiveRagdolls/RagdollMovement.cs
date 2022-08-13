using InexperiencedDeveloper.Core;
using InexperiencedDeveloper.Extensions;
using InexperiencedDeveloper.Utils.Log;
using UnityEngine;

namespace InexperiencedDeveloper.ActiveRagdoll
{
    public class RagdollMovement : MonoBehaviour
    {
        private Player player;
        private Ragdoll ragdoll;
        public TorsoMuscles torso;
        public LegMuscles legs;
        //ADD HANDS

        public void Init()
        {
            if (player != null) return;
            player = GetComponent<Player>();
            ragdoll = player.Ragdoll;
            torso = new TorsoMuscles(player, ragdoll, this);
            legs = new LegMuscles(player, ragdoll, this);
            //ADD HANDS
        }

        public void OnFixedUpdate()
        {
            //ADD HANDS
            if(torso != null && legs != null)
            {
                torso.OnFixedUpdate();
                legs.OnFixedUpdate(torso.FeedbackForce);
            }
            else
            {
                Init();
            }
        }

        #region Alignment
        public static void AlignToVector(BodySegment part, Vector3 alignmentVector, Vector3 target, float spring)
        {
            AlignToVector(part.Rigidbody, alignmentVector, target, spring, spring);
        }

        public static void AlignToVector(Rigidbody rb, Vector3 alignmentVector, Vector3 target, float spring, float maxTorque)
        {
            float multiplier = 0.1f;
            //57.29578 is 1 radian in degrees
            Vector3 cross = Vector3.Cross((Quaternion.AngleAxis(rb.angularVelocity.magnitude * 57.29578f * multiplier, rb.angularVelocity) * alignmentVector.normalized).normalized, target.normalized);
            Vector3 align = cross.normalized * Mathf.Asin(Mathf.Clamp01(cross.magnitude));
            align *= spring;
            rb.SafeAddTorque(Vector3.ClampMagnitude(align, maxTorque), ForceMode.Force);
        }

        public static void AlignLook(BodySegment part, Quaternion targetRot, float spring, float damping)
        {
            float angle;
            Vector3 axis;
            (targetRot * Quaternion.Inverse(part.transform.rotation)).ToAngleAxis(out angle, out axis);
            if (angle > 180)
                angle -= 360;
            if (angle < 180)
                angle += 360;
            part.Rigidbody.SafeAddTorque(axis * angle * spring - part.Rigidbody.angularVelocity * damping, ForceMode.Acceleration);
        }
        #endregion
    }
}

