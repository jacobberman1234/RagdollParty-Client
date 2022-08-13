using Unity.Netcode;
using UnityEngine;

public class PlayerControllerClientPrediction : NetworkBehaviour
{
    private CharacterController controller;

    [SerializeField] private float syncTimer = 2;
    private (int,Vector3) lastPos;
    private (int, Vector3) lastInput;
    private int currTick = 0;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if(IsClient && IsOwner)
        {
            var hor = Input.GetAxis("Horizontal");
            var vert = Input.GetAxis("Vertical");
            Vector3 move = new Vector3(hor, 0, vert);
            controller.Move(move * Time.deltaTime);
            lastPos.Item1 = currTick;
            lastPos.Item2 = transform.position;
        }

    }

    private void FixedUpdate()
    {
        if(IsClient && IsOwner)
        {
            currTick++;
            if(currTick % syncTimer == 0)
            {
                int tick = lastPos.Item1;
                Vector3 pos = lastPos.Item2;
                Vector3 newPos = new Vector3();
                if(!ServerReconciliation.Instance.ValidatePosition(OwnerClientId, tick, pos, out newPos))
                {
                    transform.position = newPos;
                }
            }
            if (currTick >= ServerReconciliation.BUFFER_SIZE)
                currTick = 0;
        }
    }
}
