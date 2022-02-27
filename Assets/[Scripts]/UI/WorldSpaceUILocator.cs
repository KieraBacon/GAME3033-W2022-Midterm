using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class WorldSpaceUILocator : MonoBehaviour
{
    public Transform attachmentPoint;
    private Canvas canvas;
    private Camera camera;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        camera = Camera.main;
    }

    private void LateUpdate()
    {
        if (canvas.enabled && !attachmentPoint)
        {
            canvas.enabled = false;
        }
        else
        {
            if (!canvas.enabled)
                canvas.enabled = true;

            transform.position = camera.WorldToViewportPoint(attachmentPoint.position);
        }
    }
}
