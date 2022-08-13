using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAdditions : MonoBehaviour
{
    [SerializeField] private Transform root;

    private void Update()
    {
        var newRot = root.rotation.eulerAngles;
        newRot.y += 10;
        root.rotation = Quaternion.Euler(newRot);
    }
}
