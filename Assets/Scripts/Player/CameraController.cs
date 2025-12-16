using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

// handles zooming the camera in and out
public class CameraController : MonoBehaviour
{
    private float zoomSpeed = 2f;
    private float zoomlerpSpeed = 10f; // makes the zoom smooth
    private float minDistance = 3f;
    private float maxDistance = 15f;

    private PlayerControls controls;
    private CinemachineCamera cam;
    private CinemachineOrbitalFollow orbital;
    private Vector2 scrolldelta;
    private float targetZoom;
    private float currentZoom;

    // setting up the input controls
    void Start()
    {
        controls = new PlayerControls();
        controls.Enable();
        // listen for the mouse wheel
        controls.CameraControls.MouseZoom.performed += OnMouseScroll;
        // hide the mouse cursor
        Cursor.lockState = CursorLockMode.Locked;

        cam = GetComponent<CinemachineCamera>();
        orbital = cam.GetComponent<CinemachineOrbitalFollow>();
        // set the start zoom to whatever it is right now
        targetZoom = currentZoom = orbital.Radius;

    }

    // detecting the scroll wheel movement
    private void OnMouseScroll(InputAction.CallbackContext context)
    {
        scrolldelta = context.ReadValue<Vector2>();

    }

    // moving the camera every frame
    void Update()
    {
        // if we are scrolling
        if (scrolldelta.y != 0)
        {
            if (orbital != null)
            {
                // calculate the new zoom level but keep it within limits
                targetZoom = Mathf.Clamp(orbital.Radius - scrolldelta.y * zoomSpeed, minDistance, maxDistance);
                scrolldelta = Vector2.zero; // reset scroll so it doesn't keep zooming
            }
        }
        // smoothly move to the new zoom level
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomlerpSpeed);
        orbital.Radius = currentZoom;
    }
}