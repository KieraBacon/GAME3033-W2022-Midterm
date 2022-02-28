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
    private Camera camera;

    private void Awake()
    {
        camera = Camera.main;
    }

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

    public void OnOverlookCameraSet(bool value)
    {
        overlookCamera.Priority = value ? 11 : 0;
    }

    public bool CanSeeGameObject(GameObject gameObject)
    {
        if (!gameObject || !camera) return false;

        Vector2 viewportPoint = camera.WorldToViewportPoint(gameObject.transform.position);
        Debug.Log("viewportPoint" + viewportPoint);
        if (viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1)
            return true;

        return false;
    }

    public void FocusGameObject(GameObject gameObject)
    {
        cameraTarget.position = gameObject.transform.position;
    }
}
