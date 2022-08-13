using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InexperiencedDeveloper.Extensions
{
    public static class CollisionExtensions
    {
        public static Vector3 GetNormalTangentVelocitiesAndImpulse(this Collision collision, Rigidbody rb)
        {
            ContactPoint[] contacts = collision.contacts;
            float directForce = 0f;
            float directMagnitude = 0f;
            int contactCounter = 0;
            Vector3 relativeVelocity = collision.relativeVelocity;
            foreach (var contact in contacts)
            {
                float forceDir = Vector3.Dot(contact.normal, relativeVelocity);
                float magnitude = (relativeVelocity - contact.normal * forceDir).magnitude;
                if (forceDir > directForce)
                {
                    directForce = forceDir;
                    directMagnitude = magnitude;
                }
                contactCounter++;
            }
            return new Vector3(directForce, directMagnitude, collision.impulse.magnitude);
        }
    }
}

