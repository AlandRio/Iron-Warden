using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private float zoomSpeed = 2f;
    private float zoomlerpSpeed = 10f;
    private float minDistance = 3f;
    private float maxDistance = 15f;

    private PlayerControls controls;
    private CinemachineCamera cam;
    private CinemachineOrbitalFollow orbital;
    private Vector2 scrolldelta;
    private float targetZoom;
    private float currentZoom;
    void Start()
    {
        controls = new PlayerControls();
        controls.Enable();
        controls.CameraControls.MouseZoom.performed += OnMouseScroll;
        Cursor.lockState = CursorLockMode.Locked;

        cam = GetComponent<CinemachineCamera>();
        orbital = cam.GetComponent<CinemachineOrbitalFollow>();
        targetZoom = currentZoom = orbital.Radius;

    }

    private void OnMouseScroll(InputAction.CallbackContext context)
    {
        scrolldelta = context.ReadValue<Vector2>();

    }

    // Update is called once per frame
    void Update()
    {
        if (scrolldelta.y != 0)
        {
            if (orbital != null)
            {
                targetZoom = Mathf.Clamp(orbital.Radius - scrolldelta.y * zoomSpeed, minDistance, maxDistance);
                scrolldelta = Vector2.zero;
            }
        }
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomlerpSpeed);
        orbital.Radius = currentZoom;
    }
}
