using InexperiencedDeveloper.Core;
using UnityEngine;

namespace InexperiencedDeveloper.Core
{
    [RequireComponent(typeof(Player))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerControllerRigidbody : MonoBehaviour
    {
        private Player player;
        private PlayerAnimationManager anim;
        private Rigidbody rb;
        private CapsuleCollider col;

        //Movement
        [SerializeField] private float speed = 5f;
        [SerializeField] private float speedModifier = 1f;
        [SerializeField] private float decelerationSpeed = 5f;
        public Vector2 MovementInput { get; private set; }

        //Rotation
        [SerializeField] private float turnSmoothTime = 0.1f;
        private float turnSmoothVel;
        private Transform cam;

        //Downward Ray Vars
        [Header("Floating Capsule Settings")]
        [SerializeField] private float rayDist = 2f;
        [SerializeField] private float rideForce = 25f;
        [SerializeField] private float rideHeight = 0.2f;
        [SerializeField] private LayerMask nonPlayerLayers;
        private RaycastHit rayHit;

        private void Awake()
        {
            player = GetComponent<Player>();
            rb = GetComponent<Rigidbody>();
            col = GetComponent<CapsuleCollider>();
            anim = GetComponent<PlayerAnimationManager>();
        }

        private void Start()
        {
            cam = Camera.main.transform;
        }

        private void Update()
        {
            GetInput();
            ApplyRotation();
            anim.SetMoveFloat(MovementInput.x, MovementInput.y);
        }

        //All physics stuff done here
        private void FixedUpdate()
        {
            Float();
            Move();
        }

        private void GetInput()
        {
            MovementInput = player.PlayerInput.InputActions.Player.Movement.ReadValue<Vector2>();
        }

        private void ApplyRotation()
        {
            float targetAngle = cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnSmoothTime);
            Quaternion targetRot = Quaternion.Euler(0, angle, 0);
            rb.MoveRotation(targetRot);
        }

        private void Move()
        {
            if (MovementInput == Vector2.zero || speedModifier == 0)
            {
                if (rb.velocity.magnitude > 0.1f)
                    Decelerate();
                else
                    rb.velocity = Vector3.zero;
                return;
            }
            Vector3 moveDir = MovementInput.x * transform.right + MovementInput.y * transform.forward;
            float moveSpeed = GetMoveSpeed();
            Vector3 currentVel = rb.velocity;
            currentVel.y = 0;
            rb.AddForce((moveDir * moveSpeed) - currentVel, ForceMode.VelocityChange);
        }

        private float GetMoveSpeed()
        {
            return speed * speedModifier;
        }

        private void Decelerate()
        {
            var currentVel = rb.velocity;
            currentVel.y = 0;
            rb.AddForce(-currentVel * decelerationSpeed, ForceMode.Acceleration);
        }

        private void Float()
        {
            Vector3 colliderCenterWorldSpace = transform.TransformPoint(col.center);
            Ray ray = new Ray(colliderCenterWorldSpace, Vector3.down);

            if(Physics.Raycast(ray, out rayHit, rayDist, nonPlayerLayers, QueryTriggerInteraction.Ignore))
            {
                float distToRideHeight = ((col.center.y + rideHeight) * transform.localScale.y) - rayHit.distance;
                if (distToRideHeight == 0) return;
                float liftVal = distToRideHeight * rideForce;
                liftVal -= rb.velocity.y;
                Vector3 liftForce = new Vector3(0, liftVal, 0);
                rb.AddForce(liftForce, ForceMode.VelocityChange);
            }
        }
    }
}

