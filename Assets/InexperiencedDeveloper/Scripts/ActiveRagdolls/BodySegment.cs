using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InexperiencedDeveloper.ActiveRagdoll
{
    public class BodySegment
    {
        public Transform transform;
        public Collider collider;
        public Rigidbody Rigidbody;
        public CollisionSensor sensor;
        public Quaternion startRot;
        public Transform skeleton;
        public BodySegment parent;
    }
}

