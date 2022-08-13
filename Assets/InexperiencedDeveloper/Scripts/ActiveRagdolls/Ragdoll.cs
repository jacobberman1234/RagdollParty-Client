using InexperiencedDeveloper.Utils.Log;
using System.Collections.Generic;
using UnityEngine;

namespace InexperiencedDeveloper.ActiveRagdoll
{
    public class Ragdoll : MonoBehaviour
    {
        private bool initialized; //For Multiplayer
        public float HandLength;

        [Tooltip("Amount of body parts found (should be 16, maybe 17 if ball for balancing)")]
        public int BodyPartsFound = 0;

        #region Body Part variables
        public BodySegment Head;
        public BodySegment Chest;
        public BodySegment Waist;
        public BodySegment Hips;
        public BodySegment LeftArm;
        public BodySegment LeftForearm;
        public BodySegment LeftHand;
        public BodySegment RightArm;
        public BodySegment RightForearm;
        public BodySegment RightHand;
        public BodySegment LeftThigh;
        public BodySegment LeftLeg;
        public BodySegment LeftFoot;
        public BodySegment RightThigh;
        public BodySegment RightLeg;
        public BodySegment RightFoot;
        public BodySegment Ball; //For stability
        #endregion

        private void Awake()
        {
            if (initialized) return;

            initialized = true;

            //Get and configure all pieces of the ragdoll
            GetSegments();
            //Remove collision within the object
            SetupColliders();
            HandLength = (LeftArm.transform.position - LeftForearm.transform.position).magnitude + (LeftForearm.transform.position - LeftHand.transform.position).magnitude;
        }

        private void GetSegments()
        {
            Dictionary<string, Transform> dict = new();
            Transform[] transforms = GetComponentsInChildren<Transform>();
            //Organize all children into dict for easy access by name
            for (int i = 0; i < transforms.Length; i++)
                dict.Add(transforms[i].name.ToLower(), transforms[i]);
            Head = FindSegment(dict, "head");
            Chest = FindSegment(dict, "chest");
            Waist = FindSegment(dict, "waist");
            Hips = FindSegment(dict, "hips");
            LeftArm = FindSegment(dict, "arm.l");
            LeftForearm = FindSegment(dict, "forearm.l");
            LeftHand = FindSegment(dict, "hand.l");
            RightArm = FindSegment(dict, "arm.r");
            RightForearm = FindSegment(dict, "forearm.r");
            RightHand = FindSegment(dict, "hand.r");
            LeftThigh = FindSegment(dict, "thigh.l");
            LeftLeg = FindSegment(dict, "leg.l");
            LeftFoot = FindSegment(dict, "foot.l");
            RightThigh = FindSegment(dict, "thigh.r");
            RightLeg = FindSegment(dict, "leg.r");
            RightFoot = FindSegment(dict, "foot.r");
            DebugLogger.Log($"Found {BodyPartsFound} body parts");
            DebugLogger.LogWarning($"TODO: Collision set up for body parts");
            AddAntiStretch(LeftHand, Chest);
            Debug.LogWarning("Added AntiStretch left Hand");
            AddAntiStretch(RightHand, Chest);
            AddAntiStretch(LeftFoot, Hips);
            AddAntiStretch(RightFoot, Hips);
        }

        private void AddAntiStretch(BodySegment seg1, BodySegment seg2)
        {
            ConfigurableJoint joint = seg1.Rigidbody.gameObject.AddComponent<ConfigurableJoint>();
            ConfigurableJoint joint2 = joint;
            ConfigurableJointMotion configurableJointMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = configurableJointMotion;
            joint.yMotion = configurableJointMotion;
            joint.xMotion = configurableJointMotion;
            joint.linearLimit = new SoftJointLimit
            {
                limit = (seg1.transform.position - seg2.transform.position).magnitude
            };
            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = Vector3.zero;
            joint.connectedBody = seg2.Rigidbody;
            joint.connectedAnchor = Vector3.zero;
        }

        public void BindBall(Transform ballTransform)
        {
            Ball = InitializeSegment(ballTransform);
            SpringJoint spring = Ball.Rigidbody.GetComponent<SpringJoint>();
            spring.autoConfigureConnectedAnchor = false;
            spring.connectedAnchor = Hips.transform.InverseTransformPoint(transform.position + Vector3.up * ((Ball.collider as SphereCollider).radius + spring.maxDistance));
            spring.connectedBody = Hips.Rigidbody;
            IgnoreBallCollision();
        }

        private BodySegment FindSegment(Dictionary<string, Transform> children, string name)
        {
            return InitializeSegment(children[name.ToLower()]);
        }

        private BodySegment InitializeSegment(Transform t)
        {
            BodySegment segment = new();
            segment.transform = t;
            segment.collider = t.GetComponent<Collider>();
            segment.Rigidbody = t.GetComponent<Rigidbody>();
            segment.startRot = t.localRotation;

            //FOR DEBUG ONLY
            if(segment.collider == null) DebugLogger.LogError($"{t.name} is missing Collider", true);
            if(segment.Rigidbody == null) DebugLogger.LogError($"{t.name} is missing Rigidbody", true);
            BodyPartsFound++;
            print(t.name);

            return segment;
        }

        private void SetupColliders()
        {
            //Chest
            Physics.IgnoreCollision(Chest.collider, Head.collider);
            Physics.IgnoreCollision(Chest.collider, LeftArm.collider);
            Physics.IgnoreCollision(Chest.collider, LeftForearm.collider);
            Physics.IgnoreCollision(Chest.collider, RightArm.collider);
            Physics.IgnoreCollision(Chest.collider, RightForearm.collider);
            Physics.IgnoreCollision(Chest.collider, Waist.collider);

            //Hips
            Physics.IgnoreCollision(Hips.collider, Chest.collider);
            Physics.IgnoreCollision(Hips.collider, Waist.collider);
            Physics.IgnoreCollision(Hips.collider, LeftThigh.collider);
            Physics.IgnoreCollision(Hips.collider, LeftLeg.collider);
            Physics.IgnoreCollision(Hips.collider, LeftFoot.collider);
            Physics.IgnoreCollision(Hips.collider, RightThigh.collider);
            Physics.IgnoreCollision(Hips.collider, RightLeg.collider);
            Physics.IgnoreCollision(Hips.collider, RightFoot.collider);

            //Left Arm
            Physics.IgnoreCollision(LeftArm.collider, LeftForearm.collider);
            Physics.IgnoreCollision(LeftArm.collider, LeftHand.collider);
            Physics.IgnoreCollision(LeftForearm.collider, LeftHand.collider);
            
            //Right Arm
            Physics.IgnoreCollision(RightArm.collider, RightForearm.collider);
            Physics.IgnoreCollision(RightArm.collider, RightHand.collider);
            Physics.IgnoreCollision(RightForearm.collider, RightHand.collider);

            //Left Leg
            Physics.IgnoreCollision(LeftThigh.collider, LeftLeg.collider);

            //Right Leg
            Physics.IgnoreCollision(RightThigh.collider, RightLeg.collider);
        }

        private void IgnoreBallCollision()
        {
            Physics.IgnoreCollision(Ball.collider, RightFoot.collider);
            Physics.IgnoreCollision(Ball.collider, RightLeg.collider);
            Physics.IgnoreCollision(Ball.collider, LeftFoot.collider);
            Physics.IgnoreCollision(Ball.collider, LeftLeg.collider);
            Physics.IgnoreCollision(Ball.collider, RightHand.collider);
            Physics.IgnoreCollision(Ball.collider, RightForearm.collider);
            Physics.IgnoreCollision(Ball.collider, RightArm.collider);
            Physics.IgnoreCollision(Ball.collider, LeftArm.collider);
            Physics.IgnoreCollision(Ball.collider, LeftForearm.collider);
            Physics.IgnoreCollision(Ball.collider, LeftHand.collider);
            Physics.IgnoreCollision(Ball.collider, Hips.collider);
            Physics.IgnoreCollision(Ball.collider, Chest.collider);
            Physics.IgnoreCollision(Ball.collider, Waist.collider);
        }
    }
}

