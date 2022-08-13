using InexperiencedDeveloper.Extensions;
using InexperiencedDeveloper.Core.Settings;
using InexperiencedDeveloper.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace InexperiencedDeveloper.ActiveRagdoll
{
    public class CameraController : MonoBehaviour
    {
        public Player Player;
        public Camera GameCam;
        private Ragdoll ragdoll;

        public Vector3 CloseTargetOffset;
        public float CloseRange;
        public float CloseRangeLookUp;
        public bool IgnoreWalls;
        public float FarRange;
        public LayerMask WallLayers;

        private Vector3 oldTarget;
        private Vector3 smoothTarget;
        private int lastFrame;
        private Vector3 fixedUpdateSmooth;
        private float nearClip = 0.05f;
        private Vector3[] rayStarts = new Vector3[4];
        private float offsetSpeed;
        private float camTransitionPhase;
        private float camTransitionSpeed = 1f;
        private float startFOV;
        private Vector3 startOffset;
        private Quaternion startRot;
        private Vector3 startPlayerCamPos;

        private static Action<string> onCameraSmooth;

        public static float Smoothing_Amount = 0.5f;
        public static float FOV_Adjust;
        [HideInInspector] public float Offset = 4f;


        private void Start()
        {
            ragdoll = Player.Ragdoll;
            //TODO: Water Sensor potentially
            //FOR SETTINGS __ SOMETHIGN TO WORK ON LATER
            if (onCameraSmooth == null)
            {
                onCameraSmooth = new Action<string>(OnCameraSmooth);
            }
        }

        private static void OnCameraSmooth(string param)
        {
            if (!string.IsNullOrEmpty(param))
            {
                param = param.ToLowerInvariant();
                float num;
                if (float.TryParse(param, out num))
                {
                    Options.CameraSmoothing = Mathf.Clamp((int)(num * 20f), 0, 40);
                }
                else if ("off".Equals(param))
                {
                    Options.CameraSmoothing = 0;
                }
                else if ("on".Equals(param))
                {
                    Options.CameraSmoothing = 20;
                }
            }
        }

        public void Scroll(Vector3 offset)
        {
            oldTarget += offset;
            smoothTarget += offset;
            transform.position += offset;
            fixedUpdateSmooth += offset;
        }

        private Vector3 SmoothCamera(Vector3 target, float deltaTime)
        {
            //Add Multiplayer support
            int steps = Mathf.RoundToInt(deltaTime / (Time.fixedDeltaTime / 10f));
            if (steps < 1) steps = 1;
            if (Smoothing_Amount == 0 || deltaTime == 0 || steps > 1000 || (target - oldTarget).magnitude > 10f)
            {
                deltaTime = 0;
                oldTarget = target;
                smoothTarget = target;
                return target;
            }
            float stepTime = deltaTime / (float)steps;
            float maxMagnitude = Offset * 0.1f * Smoothing_Amount;
            Vector3 targetAdjustment = (target - oldTarget) / deltaTime;
            for(int i = 0; i < steps; i++)
            {
                oldTarget += targetAdjustment * stepTime;
                Vector3 newPos = oldTarget + Vector3.ClampMagnitude(smoothTarget - oldTarget, maxMagnitude);
                float smoothing = Mathf.SmoothStep(0.05f, 2f, (newPos - oldTarget).magnitude / maxMagnitude);
                smoothTarget = Vector3.MoveTowards(newPos, oldTarget, stepTime * smoothing);
            }
            deltaTime = 0;
            return smoothTarget;
        }

        private void LateUpdate()
        {
            //Change flag for online
            bool flag = false;
            Vector3 targetOffset;
            float yaw;
            float pitch;
            float camDist;
            float minDist;
            float fov;
            float nearClip;
            //TODO: Add multiple camera modes
            CalculateCloseCam(out targetOffset, out yaw, out pitch, out camDist, out minDist, out fov, out nearClip);
            if(FOV_Adjust != 0)
            {
                // 1 Degree = 0.017453292 radians
                camDist *= Mathf.Tan(MathUtils.DEGREE_IN_RADIANS * fov / 2f) / Mathf.Tan(MathUtils.DEGREE_IN_RADIANS * (fov + FOV_Adjust) / 2f);
                fov += FOV_Adjust;
            }
            //Replace with Variable
            camDist /= 1f;
            camDist = Mathf.Max(camDist, minDist);
            //Add adjustments for Menu Cameras
            Quaternion lookRot = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 camForward = lookRot * Vector3.forward;
            Vector3 targetPos = ((!flag) ? SmoothCamera(ragdoll.Head.transform.position, Time.unscaledDeltaTime) : fixedUpdateSmooth) + targetOffset;
            nearClip *= Mathf.Clamp(GameCam.transform.position.magnitude / 500f, 1f, 2f);
            GameCam.nearClipPlane = nearClip;
            GameCam.fieldOfView = fov;
            float limit = (!IgnoreWalls) ? CompensateForWallsNearPlane(targetPos, lookRot, FarRange * 1.2f, minDist) : 10000f;
            Offset = SpringArm(Offset, camDist, limit, Time.unscaledDeltaTime);
            RaycastHit hit;
            if(limit < Offset && !Physics.SphereCast(targetPos - camForward * Offset, nearClip * 2f, camForward, out hit, Offset - limit, WallLayers, QueryTriggerInteraction.Ignore))
            {
                Offset = limit;
                offsetSpeed = 0;
            }
            ApplyCamera(targetPos, targetPos - camForward * Offset, lookRot, fov);
        }

        private float SpringArm(float current, float target, float limit, float deltaTime)
        {
            int steps = Mathf.RoundToInt(deltaTime / (Time.fixedDeltaTime / 10f));
            if(steps < 1) steps = 1;
            if (deltaTime == 0 || steps > 1000) return target;
            float stepTime = deltaTime / (float)steps;
            if(limit < target)
            {
                target = limit;
                if(target < current)
                {
                    offsetSpeed = 0f;
                    IntegrateDirect(target, ref current, stepTime, steps, 5f, 10f);
                    return current;
                }
            }
            if(target < current)
            {
                IntegrateSpring(target, ref current, offsetSpeed, stepTime, steps, 100f, 10f, 500f);
            }
            else
            {
                IntegrateSpring(target, ref current, offsetSpeed, stepTime, steps, 2f, 1f, 6f);
            }
            return current;
        }

        private void IntegrateSpring(float target, ref float current, float offsetSpeed, float stepTime, int steps, float spring, float damper, float maxForce)
        {
            for (int i = 0; i < steps; i++)
            {
                if(offsetSpeed * (target - current) <= 0f)
                {
                    offsetSpeed = 0;
                }
                offsetSpeed += Mathf.Clamp(spring * (target - current), -maxForce, maxForce) * stepTime;
                offsetSpeed = Mathf.MoveTowards(offsetSpeed, 0, Mathf.Abs(offsetSpeed * damper * stepTime));
                current = Mathf.MoveTowards(current, target, Mathf.Abs(offsetSpeed * stepTime));
            }
        }

        private void IntegrateDirect(float target, ref float current, float stepTime, int steps, float minSpeed, float spring)
        {
            for(int i = 0; i < steps; i++)
            {
                current = Mathf.MoveTowards(current, target, (minSpeed + Mathf.Abs(spring * (target - current))) * stepTime);
            }
        }

        private void CalculateCloseCam(out Vector3 targetOffset, out float yaw, out float pitch, out float camDist, out float minDist, out float fov, out float nearClip)
        {
            fov = 70f;
            nearClip = this.nearClip;
            yaw = Player.Controls.CameraYawAngle;
            pitch = Player.Controls.CameraPitchAngle + 10f;
            if (pitch < 0f)
            {
                pitch *= 0.8f;
            }
            Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
            targetOffset = rot * CloseTargetOffset;
            Vector3 newRot = Quaternion.Euler(Player.Controls.CameraPitchAngle, Player.Controls.CameraYawAngle, 0f) * Vector3.forward;
            camDist = Mathf.Lerp(CloseRange, CloseRangeLookUp, newRot.y);
            minDist = 0.025f;
        }

        private float CompensateForWallsNearPlane(Vector3 targetPos, Quaternion lookRot, float desiredDist, float minDist)
        {
            Vector3 camForward = lookRot * Vector3.forward;
            float nearClipPlane = GameCam.nearClipPlane;
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;
            transform.rotation = lookRot;
            transform.position = targetPos - camForward * (GameCam.nearClipPlane + minDist);
            rayStarts[0] = GameCam.ViewportToWorldPoint(new Vector3(0f, 0f, nearClipPlane));
            rayStarts[1] = GameCam.ViewportToWorldPoint(new Vector3(0f, 1f, nearClipPlane));
            rayStarts[2] = GameCam.ViewportToWorldPoint(new Vector3(1f, 0f, nearClipPlane));
            rayStarts[3] = GameCam.ViewportToWorldPoint(new Vector3(1f, 1f, nearClipPlane));
            float adjDist = desiredDist - nearClipPlane;
            for (int i = 0; i < rayStarts.Length; i++)
            {
                Ray ray = new Ray(rayStarts[i], -camForward);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, adjDist, WallLayers) && hit.distance < adjDist)
                {
                    adjDist = hit.distance;
                }
            }
            adjDist += nearClipPlane + minDist;
            if(adjDist < minDist * 2f)
            {
                adjDist = minDist * 2f;
            }
            return adjDist;
        }

        public void ApplyCamera(Vector3 target, Vector3 pos, Quaternion rot, float fov)
        {
            camTransitionPhase += camTransitionSpeed * Time.deltaTime;
            if (camTransitionPhase >= 1f)
            {
                transform.position = pos;
                transform.rotation = rot;
                GameCam.fieldOfView = fov;
            }
            else
            {
                float transition = Ease.EaseInOutSine(0f, 1f, Mathf.Clamp01(camTransitionPhase));
                transform.position = pos;
                transform.rotation = rot;
                GameCam.fieldOfView = fov;
                Vector3 worldToView = GameCam.WorldToViewportPoint(Player.transform.position);
                Vector3 smoothPos = Vector3.Lerp(startPlayerCamPos, worldToView, camTransitionPhase);
                transform.rotation = Quaternion.Lerp(startRot, rot, camTransitionPhase);
                GameCam.fieldOfView = Mathf.Lerp(startFOV, fov, camTransitionPhase);
                Vector3 smoothPosToWorld = GameCam.ViewportToWorldPoint(smoothPos);
                transform.position += Player.transform.position - smoothPosToWorld;
            }
            bool flag = false; // Grab check
            float clampedDistance = (!flag) ? Mathf.Clamp((target - pos).magnitude, 6f, 6f) : Mathf.Clamp((target - pos).magnitude, 4f, 5f);
        }
    }
}


