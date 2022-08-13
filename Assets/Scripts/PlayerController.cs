using InexperiencedDeveloper.Core;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayControllerRigidbody : NetworkBehaviour
{
    private CharacterController controller;
    private Transform cam;
    private PlayerAnimationManager anim;

    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private Vector2 defaultPosRange = new Vector2(-4, 4);
    private float turnSmoothVel;
    private Vector3 move;
    private Vector2 playerVel;
    public Vector2 PlayerVel => playerVel;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) Destroy(this);
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<PlayerAnimationManager>();
    }

    private void Start()
    {
        transform.position = new Vector3(Random.Range(defaultPosRange.x, defaultPosRange.y), 0, Random.Range(defaultPosRange.x, defaultPosRange.y));
        NetworkPlayerCamera.Instance.Follow(transform);
        cam = NetworkPlayerCamera.Instance.GetCamTransform();
    }

    private void Update()
    {
        move = GetInput();
        Move();
    }

    private void Move()
    {
        if (move.magnitude >= 0.1f)
        {
            float targetAngle = cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            controller.Move(move.normalized * speed * Time.deltaTime);
        }
        playerVel = new Vector2(controller.velocity.x, controller.velocity.z);
        anim.SetMoveFloat(playerVel.x, playerVel.y);
    }

    private Vector3 GetInput()
    {
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        return move.normalized;
    }


}
