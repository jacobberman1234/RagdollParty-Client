using UnityEngine;


namespace InexperiencedDeveloper.Core
{
    public class PlayerAnimationManager : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        public Animator Anim => anim;

        private void Awake()
        {
            if(anim == null) anim = GetComponent<Animator>();
        }

        public void SetMoveBool(bool isMoving)
        {
            Anim.SetBool("isMoving", isMoving);
        }

        public void SetMoveFloat(float x, float z)
        {
            Anim.SetFloat("hor", x);
            Anim.SetFloat("vert", z);
        }
    }

}
