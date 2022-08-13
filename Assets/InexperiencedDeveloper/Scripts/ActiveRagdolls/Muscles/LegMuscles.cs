using InexperiencedDeveloper.Extensions;
using InexperiencedDeveloper.Utils.Log;
using UnityEngine;

namespace InexperiencedDeveloper.ActiveRagdoll
{
    public class LegMuscles
    {
        private readonly Player player;
        private readonly Ragdoll ragdoll;
        private readonly RagdollMovement movement;
        private float ballRadius;
        private PhysicMaterial ballMat;
        private PhysicMaterial footMat;
        private float ballFriction;
        private float footFriction;

        public LegMuscles (Player player, Ragdoll ragdoll, RagdollMovement movement)
        {
            this.player = player;
            this.ragdoll = ragdoll;
            this.movement = movement;
            ballRadius = (ragdoll.Ball.collider as SphereCollider).radius;
            ballMat = ragdoll.Ball.collider.material;
            footMat = ragdoll.RightFoot.collider.material;
            ballFriction = ballMat.staticFriction;
            footFriction = footMat.staticFriction;
        }

        public void OnFixedUpdate(Vector3 torsoFeedback)
        {
            switch (player.State)
            {
                case PlayerState.Idle:
                    //DebugLogger.LogWarning($"TODO: Check additional movement parameters");
                    IdleAnimation(torsoFeedback, 1f);
                    break;
                case PlayerState.Run:
                    RunAnimation(torsoFeedback, 1f);
                    break;
                case PlayerState.Jump:
                    JumpAnimation(torsoFeedback);
                    break;
            }
        }

        #region Animation Variables
        private float legPhase;
        #endregion

        private void IdleAnimation(Vector3 torsoFeedback, float rigidity)
        {
            RagdollMovement.AlignToVector(ragdoll.LeftThigh, -ragdoll.LeftThigh.transform.up, Vector3.up, 10f * rigidity);
            RagdollMovement.AlignToVector(ragdoll.LeftLeg, -ragdoll.LeftLeg.transform.up, Vector3.up, 10f * rigidity);
            RagdollMovement.AlignToVector(ragdoll.RightThigh, -ragdoll.RightThigh.transform.up, Vector3.up, 10f * rigidity);
            RagdollMovement.AlignToVector(ragdoll.RightLeg, -ragdoll.RightLeg.transform.up, Vector3.up, 10f * rigidity);
            ragdoll.Ball.Rigidbody.SafeAddForce(torsoFeedback * 0.2f, ForceMode.Force);
            ragdoll.LeftFoot.Rigidbody.SafeAddForce(torsoFeedback * 0.4f, ForceMode.Force);
            ragdoll.RightFoot.Rigidbody.SafeAddForce(torsoFeedback * 0.4f, ForceMode.Force);
            ragdoll.RightFoot.Rigidbody.angularVelocity = Vector3.zero;
        }

        private void RunAnimation(Vector3 torsoFeedback, float rigidity)
        {
            legPhase = Time.realtimeSinceStartup * 1.5f;
            torsoFeedback += AnimateLeg(ragdoll.LeftThigh, ragdoll.LeftLeg, ragdoll.LeftFoot, legPhase, torsoFeedback, rigidity);
            torsoFeedback += AnimateLeg(ragdoll.RightThigh, ragdoll.RightLeg, ragdoll.RightFoot, legPhase + 0.5f, torsoFeedback, rigidity);
            ragdoll.Ball.Rigidbody.SafeAddForce(torsoFeedback, ForceMode.Force);
            RotateBall();
            AddWalkForce();
        }

        private void JumpAnimation(Vector3 torsoFeedback)
        {
            DebugLogger.LogError($"Jump Animation function incomplete", false);
        }

        private Vector3 AnimateLeg(BodySegment thigh, BodySegment leg, BodySegment foot, float phase, Vector3 torsoFeedback, float rigidity)
        {
            rigidity *= 1f;
            phase -= Mathf.Floor(phase);
            if(phase < 0.2f)
            {
                RagdollMovement.AlignToVector(thigh, thigh.transform.up, player.Controls.WalkDir + Vector3.down, 3f * rigidity);
                RagdollMovement.AlignToVector(leg, thigh.transform.up, -player.Controls.WalkDir - Vector3.up, rigidity);
                Vector3 force = Vector3.up * 20f;
                foot.Rigidbody.SafeAddForce(force, ForceMode.Force);
                return -force;
            }
            if(phase < 0.5f)
            {
                RagdollMovement.AlignToVector(thigh, thigh.transform.up, player.Controls.WalkDir, 2f * rigidity);
                RagdollMovement.AlignToVector(leg, thigh.transform.up, player.Controls.WalkDir, 3f * rigidity);
            }
            else
            {
                if(phase < 0.7f)
                {
                    Vector3 force = torsoFeedback * 0.2f;
                    foot.Rigidbody.SafeAddForce(force, ForceMode.Force);
                    RagdollMovement.AlignToVector(thigh, thigh.transform.up, player.Controls.WalkDir + Vector3.down, rigidity);
                    RagdollMovement.AlignToVector(leg, thigh.transform.up, Vector3.down, rigidity);
                    return -force;
                }
                if(phase < 0.9f)
                {
                    Vector3 force = torsoFeedback * 0.2f;
                    foot.Rigidbody.SafeAddForce(force, ForceMode.Force);
                    RagdollMovement.AlignToVector(thigh, thigh.transform.up, -player.Controls.WalkDir + Vector3.down, rigidity);
                    RagdollMovement.AlignToVector(leg, thigh.transform.up, -player.Controls.WalkDir + Vector3.down, rigidity);
                    return -force;
                }
                RagdollMovement.AlignToVector(thigh, thigh.transform.up, -player.Controls.WalkDir + Vector3.down, rigidity);
                RagdollMovement.AlignToVector(leg, thigh.transform.up, -player.Controls.WalkDir, rigidity);
            }
            return Vector3.zero;
        }

        private void RotateBall()
        {
            float ballSpeed = player.State != PlayerState.Run ? 1.2f : 2.5f;
            Vector3 inverseDir = new Vector3(player.Controls.WalkDir.z, 0, -player.Controls.WalkDir.x);
            ragdoll.Ball.Rigidbody.angularVelocity = ballSpeed / ballRadius * inverseDir;
        }

        private void AddWalkForce()
        {
            float speed = player.Speed;
            Vector3 force = player.Controls.WalkDir * speed;
            ragdoll.Ball.Rigidbody.SafeAddForce(force, ForceMode.Force);
            //CALCULATE GROUND CHECK
            //if (player.Grounded)
            //    player.GroundManager.DistributeForce(-force, ragdoll.Ball.Rigidbody.position);
            //ADD SEGMENT TO CALCULATE IF GRABBING SOMETHING
        }
    }
}

