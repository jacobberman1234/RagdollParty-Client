using InexperiencedDeveloper.Extensions;
using InexperiencedDeveloper.Utils;
using InexperiencedDeveloper.Utils.Log;
using UnityEngine;

namespace InexperiencedDeveloper.ActiveRagdoll
{
    public class TorsoMuscles
    {
        private readonly Player player;
        private readonly Ragdoll ragdoll;
        private readonly RagdollMovement movement;

        public TorsoMuscles (Player player, Ragdoll ragdoll, RagdollMovement movement)
        {
            this.player = player;
            this.ragdoll = ragdoll;
            this.movement = movement;
        }

        public Vector3 FeedbackForce;
        private float timeSinceLastJump = 0;

        public void OnFixedUpdate()
        {
            if (!player.Grounded) timeSinceLastJump = 0;
            
            PlayerState state = player.State;
            switch (state)
            {
                case PlayerState.Idle:
                    FeedbackForce = IdleAnimation();
                    break;
                case PlayerState.Run:
                    FeedbackForce = RunAnimation();
                    break;
                case PlayerState.Jump:
                    FeedbackForce = JumpAnimation();
                    break;
            }
        }

        #region Animation Variables

        private float idleAnimationPhase;
        private float idleAnimationDuration = 3f;

        #endregion

        private Vector3 IdleAnimation()
        {
            idleAnimationPhase = MathUtils.NonModuloWrap(idleAnimationPhase + Time.deltaTime / idleAnimationDuration, 1f);
            float torsoBend = Mathf.Lerp(1f, -0.5f, Mathf.Sin(idleAnimationPhase * MathUtils.CHEAP_PI * 2f) / 2f + 0.5f);
            return ApplyTorsoPose(1f, 1f, torsoBend, 1f);
        }

        private Vector3 RunAnimation()
        {
            return ApplyTorsoPose(1f, 1f, 0f, 1f);
        }

        private Vector3 JumpAnimation()
        {
            return ApplyTorsoPose(1f, 1f, 0f, 1f);
        }

        private Vector3 ApplyTorsoPose(float torsoRigidity, float headRigidity, float torsoBend, float lift)
        {
            //DebugLogger.LogWarning($"TODO: Integrate Lift while knocked out");
            lift *= Mathf.Clamp01(timeSinceLastJump * 0.2f + 0.8f);
            headRigidity *= 2f;
            float adjustedWeight = player.Weight * 0.8f * lift;
            float pitchAngle = player.Controls.CameraPitchAngle;
            float yawAngle = player.Controls.CameraYawAngle;
            RagdollMovement.AlignLook(ragdoll.Head, Quaternion.Euler(pitchAngle, yawAngle, 0), 0.25f * headRigidity, 10 * headRigidity);
            if (player.Grounded)
            {
                torsoBend *= 40;
                RagdollMovement.AlignLook(ragdoll.Chest, Quaternion.Euler(pitchAngle + torsoBend, yawAngle, 0), 2f * torsoRigidity, 10 * torsoRigidity);
                RagdollMovement.AlignLook(ragdoll.Waist, Quaternion.Euler(pitchAngle + torsoBend / 2f, yawAngle, 0), torsoRigidity, 15 * torsoRigidity);
                RagdollMovement.AlignLook(ragdoll.Hips, Quaternion.Euler(pitchAngle, yawAngle, 0), 0.5f * torsoRigidity, 20 * torsoRigidity);
            }
            float targetDir = -player.TargetDir.y;
            //DebugLogger.LogWarning($"TODO: Integrate target direction while grabbing");
            Vector3 headMovement = Mathf.Lerp(0.2f, 0f, targetDir) * adjustedWeight * headRigidity * Vector3.up;
            Vector3 chestMovement = Mathf.Lerp(0.6f, 0f, targetDir) * adjustedWeight * torsoRigidity * Vector3.up;
            Vector3 waistMovement = Mathf.Lerp(0.2f, 0.5f, targetDir) * adjustedWeight * torsoRigidity * Vector3.up;
            Vector3 hipMovement = Mathf.Lerp(0f, 0.5f, targetDir) * adjustedWeight * torsoRigidity * Vector3.up;
            ragdoll.Head.Rigidbody.SafeAddForce(headMovement, ForceMode.Force);
            ragdoll.Chest.Rigidbody.SafeAddForce(chestMovement, ForceMode.Force);
            ragdoll.Waist.Rigidbody.SafeAddForce(waistMovement, ForceMode.Force);
            ragdoll.Hips.Rigidbody.SafeAddForce(hipMovement, ForceMode.Force);
            StabilizeHorizontal(ragdoll.Hips.Rigidbody, ragdoll.Ball.Rigidbody, 1f * lift * Mathf.Lerp(1f, 0.25f, Mathf.Abs(targetDir)));
            //StabilizeHorizontal(ragdoll.Head.Rigidbody, ragdoll.Ball.Rigidbody, 0.2f * lift * Mathf.Lerp(1f, 0f, Mathf.Abs(targetDir)));
            return -(headMovement + chestMovement + waistMovement + hipMovement);
        }

        private void StabilizeHorizontal(Rigidbody top, Rigidbody bottom, float multiplier)
        {
            //Need to figure out what this constant does
            float d = 3f;
            Vector3 calc = bottom.position + bottom.velocity * Time.fixedDeltaTime - top.position - top.velocity * Time.fixedDeltaTime * d;
            Vector3 stableForce = calc * top.mass / Time.fixedDeltaTime;
            stableForce *= multiplier;
            top.SafeAddForce(stableForce, ForceMode.Force);
            bottom.SafeAddForce(-stableForce, ForceMode.Force);
        }
    }
}