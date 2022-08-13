using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InexperiencedDeveloper.ActiveRagdoll.Utils
{
    public class FollowPlayer : MonoBehaviour
    {
        [SerializeField] private Transform player;
        private float originalY;

        private void Start()
        {
            originalY = transform.position.y;
        }

        private void Update()
        {
            var newPos = player.position;
            newPos.y = originalY;
            transform.position = newPos;
        }
    }
}

