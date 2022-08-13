using Cinemachine;
using InexperiencedDeveloper.Core;
using UnityEngine;

public class NetworkPlayerCamera : Singleton<NetworkPlayerCamera>
{
    private CinemachineFreeLook vcam;
    protected override void Awake()
    {
        base.Awake();
        vcam = GetComponent<CinemachineFreeLook>();
    }

    public void Follow(Transform transform)
    {
        vcam.Follow = transform;
        vcam.LookAt = transform;
    }

    public Transform GetCamTransform()
    {
        return transform;
    }
}
