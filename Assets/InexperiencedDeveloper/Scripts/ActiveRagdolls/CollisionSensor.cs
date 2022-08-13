using InexperiencedDeveloper.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InexperiencedDeveloper.ActiveRagdoll
{
    public class CollisionSensor : MonoBehaviour
    {
        public Player Player;

        private Transform myTransform;
        private Rigidbody myRB;

        public Action<GameObject, Vector3, PhysicMaterial, Vector3> OnCollideTap;
        public Action<CollisionSensor, Collision> OnStayTap;

        private Vector3 entryTangentVelocityImpulse;
        private Vector3 normalTangentVelocityImpulse;

        //ADD ISGRABBED
        private void OnEnable()
        {
            myTransform = transform;
            myRB = GetComponent<Rigidbody>();
            //ADD GRAB MANAGER
            //ADD GROUND MANAGER
            Player = GetComponentInParent<Player>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.contacts.Length == 0) return;
            entryTangentVelocityImpulse = (normalTangentVelocityImpulse = collision.GetNormalTangentVelocitiesAndImpulse(myRB));
            HandleCollision(collision, true);
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.contacts.Length == 0) return;
            normalTangentVelocityImpulse = collision.GetNormalTangentVelocitiesAndImpulse(myRB);
            HandleCollision(collision, false);
            if(OnStayTap != null)
            {
                OnStayTap(this, collision);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            return;
            //Ground Check
        }

        private void HandleCollision(Collision collision, bool enter)
        {
            if (collision.contacts.Length == 0) return;
            if(myTransform != null)
            {
                Transform transform = collision.transform;
                if (transform.root != myTransform.root)
                {
                    Rigidbody rb = collision.rigidbody;
                    Collider collider = collision.collider;
                    ContactPoint[] contacts = collision.contacts;
                    //Grab check
                    //Ground check
                    if (enter && OnCollideTap != null)
                    {
                        OnCollideTap(gameObject, contacts[0].point, collider.sharedMaterial, normalTangentVelocityImpulse);
                    }
                }

            }
        }

        private void FixedUpdate()
        {
            //Grab checks
        }
    }
}

