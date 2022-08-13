using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpdateMode
{
    Update,
    FixedUpdate,
    LateUpdate
}

public class SecondOrderTest : MonoBehaviour
{
    public UpdateMode UpdateMode = UpdateMode.Update;
    //[Range(-1, 10)]
    public float f = 1; //Frequency -- higher is faster vibrations
    //[Range(-1, 5)]
    public float z = 1; // Damping coefficient (0 never stops, >1 will eventually stop)
    //[Range(-1, 3)]
    public float r = 1; // >1 overshoot (responsiveness)
    private float lastF, lastZ, lastR;
    public Transform Target;
    private Vector3 targetLastPos;

    public SecondOrderDynamics dynamics;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        dynamics = new SecondOrderDynamics(f, z, r, Target.position);
        targetLastPos = Target.position;
        transform.position = dynamics.Update(Time.deltaTime, Target.position, targetLastPos);
        lastF = f;
        lastZ = z;
        lastR = r;
    }

    private void Update()
    {
        if (lastF != f || lastZ != z || lastR != r)
        {
            Init();
            return;
        }
        if (UpdateMode == UpdateMode.Update)
        {
            var xd = Target.position - targetLastPos;
            xd /= Time.deltaTime;
            transform.position = dynamics.Update(Time.deltaTime, Target.position, Vector3.zero);
        }
    }
}
