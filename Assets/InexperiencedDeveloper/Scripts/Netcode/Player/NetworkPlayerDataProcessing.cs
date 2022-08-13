using InexperiencedDeveloper.Core;
using InexperiencedDeveloper.Utils;
using InexperiencedDeveloper.Utils.Log;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerDataProcessing : NetworkBehaviour
{
    private PlayerAnimationManager anim;
    private Rigidbody rb;
    private NetworkPlayerControllerRigidbody controller;
    
    [SerializeField] private float lerpTime = 0.1f;
    private NetworkVariable<NetworkPlayerData> posAndRot = new(writePerm: NetworkVariableWritePermission.Server);
    private NetworkVariable<NetworkAnimData> animInfo = new(writePerm: NetworkVariableWritePermission.Server);

    private void Awake()
    {
        anim = GetComponent<PlayerAnimationManager>();
        controller = GetComponent<NetworkPlayerControllerRigidbody>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (IsOwner) TransmitData();
        else ConsumeData();
    }

    private void TransmitData()
    {
        var pos = transform.position;
        pos.y = 0;
        var locData = new NetworkPlayerData
        {
            Pos = pos,
            Rot = transform.rotation.eulerAngles
        };

        var animData = new NetworkAnimData
        {
            Move = new Vector2(controller.MovementInput.x, controller.MovementInput.y)
        };
        
        if (IsServer)
        {
            posAndRot.Value = locData;
            animInfo.Value = animData;
        }

        else
        {
            TransmitDataServerRpc(locData, animData);
        }
    }

    [ServerRpc]
    private void TransmitDataServerRpc(NetworkPlayerData locData, NetworkAnimData animData)
    {
        posAndRot.Value = locData;
        animInfo.Value = animData;
    }

    private Vector3 vel;
    private float rotVel;

    //REDO THIS LATER
    private void ConsumeData()
    {
        //Pos and Rot
        var newPos = Vector3.SmoothDamp(transform.position, posAndRot.Value.Pos, ref vel, lerpTime);
        newPos.y = 0;
        transform.position = Vector3.SmoothDamp(newPos, posAndRot.Value.Pos, ref vel, lerpTime);

        transform.rotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.eulerAngles.y, posAndRot.Value.Rot.y, ref rotVel, lerpTime), 0);
        //Anim
        anim.SetMoveFloat(animInfo.Value.Move.x, animInfo.Value.Move.y);
    }
}
