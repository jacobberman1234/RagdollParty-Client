using InexperiencedDeveloper.Input;
using UnityEngine;

namespace InexperiencedDeveloper.Core
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : MonoBehaviour
    {
        public PlayerInput PlayerInput { get; private set; }

        private void Awake()
        {
            PlayerInput = GetComponent<PlayerInput>();
        }
    }
}

