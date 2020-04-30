using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject target;


    private Vector3 targetPos;

    [SerializeField]
    private float focusPointDistance;

    [SerializeField]
    private Vector3 offset;

    private void OnValidate()
    {
        if (!target) return;
        transform.position = target.transform.position + offset;
        transform.LookAt(target.transform.position + target.transform.forward * focusPointDistance);
    }

    private void Start()
    {
        targetPos = target.transform.position;
    }

    private void Update()
    {
        targetPos = Vector3.Lerp(targetPos, target.transform.position, Time.deltaTime * 5);
        transform.position = targetPos + offset;
    }
}
