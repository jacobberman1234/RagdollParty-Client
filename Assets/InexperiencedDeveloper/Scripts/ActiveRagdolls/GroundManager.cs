using InexperiencedDeveloper.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InexperiencedDeveloper.ActiveRagdoll
{
    public class GroundManager : MonoBehaviour
    {
        private static List<GroundManager> all = new List<GroundManager>();
        private List<Rigidbody> groundRigids = new List<Rigidbody>();

        private void OnEnable()
        {
            all.Add(this);
        }

        private void OnDisable()
        {
            all.Remove(this);
        }

        public void DistributeForce(Vector3 force, Vector3 pos)
        {
            for (int i = 0; i < groundRigids.Count; i++)
            {
                Rigidbody rb = groundRigids[i];
                if (rb != null)
                    rb.SafeAddForceAtPosition(Vector3.ClampMagnitude(force / (float)groundRigids.Count, rb.mass / Time.fixedDeltaTime * 10f), pos, ForceMode.Force);
            }
        }
    }
}

