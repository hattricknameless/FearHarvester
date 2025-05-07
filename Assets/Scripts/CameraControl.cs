using UnityEngine;
using Cinemachine;

public class CameraControl : MonoBehaviour {
    
    public CinemachineVirtualCamera virtualCamera; // Assign your Cinemachine Virtual Camera
    public Transform centerPoint; // The point around which the camera orbits
    private float rotationSpeed = 1000; // Speed of orbit rotation

    private Vector3 offset; // Offset from the center point
    private bool isDragging = false;

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        if (virtualCamera == null)
        {
            Debug.LogError("Cinemachine Virtual Camera is not assigned.");
            return;
        }

        if (centerPoint == null)
        {
            Debug.LogError("Center point for orbiting is not assigned.");
            return;
        }

        // Calculate the initial offset between the camera and the center point
        offset = virtualCamera.transform.position - centerPoint.position;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(1)) {// Right mouse button pressed
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(1)) {// Right mouse button released
            isDragging = false;
        }

        if (isDragging) {
            // Get horizontal mouse input
            float mouseDeltaX = Input.GetAxis("Mouse X");

            // Calculate rotation angle on the Y-axis
            float angleY = mouseDeltaX * rotationSpeed * Time.deltaTime;

            // Rotate the offset around the Y-axis
            Quaternion rotationY = Quaternion.AngleAxis(angleY, Vector3.up);
            offset = rotationY * offset;

            // Update the camera's position and rotation
            virtualCamera.transform.position = centerPoint.position + offset;
            virtualCamera.transform.LookAt(centerPoint);
        }
    }
}