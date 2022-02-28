using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float cameraPanningSpeed;
    [SerializeField]
    private Transform cameraTarget;
    [SerializeField]
    private float cameraRadius;
    private Vector2 cameraPanningValue;
    [SerializeField]
    private Cinemachine.CinemachineVirtualCamera overlookCamera;
    internal void OnPanCameraPerformed(Vector2 value)
    {
        cameraPanningValue = value;
    }

    private void Update()
    {
        cameraTarget.transform.position = cameraTarget.transform.position + new Vector3(-cameraPanningValue.x, 0, -cameraPanningValue.y) * cameraPanningSpeed * Time.deltaTime;
        if (Vector3.Distance(Vector3.zero, cameraTarget.transform.position) > cameraRadius)
            cameraTarget.transform.position = cameraTarget.transform.position.normalized * cameraRadius;
    }

    internal void OnOverlookCameraSet(bool value)
    {
        overlookCamera.Priority = value ? 11 : 0;
    }
}
