using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowUI : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The transform to follow. Assign the Main Camera from your XR Origin here.")]
    public Transform target;

    [Header("Movement Settings")]
    [Tooltip("The distance in meters to maintain from the target.")]
    [Range(0.5f, 5f)]
    public float distance = 2.0f;

    [Tooltip("A positional offset from the target's viewpoint. Use the Y-axis to move the UI up or down.")]
    public Vector3 positionOffset = Vector3.zero;

    [Tooltip("How smoothly the UI follows the target's position. Lower values are smoother and more delayed.")]
    [Range(0.01f, 1.0f)]
    public float smoothSpeed = 0.125f;

    [Header("Rotation Settings")]
    [Tooltip("A rotational offset in Euler angles. Use this to fine-tune the UI's default orientation.")]
    public Vector3 rotationOffset = Vector3.zero;

    [Tooltip("How smoothly the UI rotates to face the target. Lower values result in a more gentle rotation.")]
    [Range(0.01f, 1.0f)]
    public float rotationSpeed = 0.1f;

    void Start()
    {
        // If no target is assigned in the Inspector, this will attempt to find the Main Camera automatically.
        if (target == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                target = mainCamera.transform;
                Debug.Log("Target for SmoothFollow was not assigned. Automatically assigned to the Main Camera.");
            }
            else
            {
                Debug.LogError("SmoothFollow Error: A target is not assigned and the Main Camera could not be found. Please assign the target manually in the Inspector.", this);
                // Disable the script to prevent errors in LateUpdate.
                enabled = false;
                return;
            }
        }
    }

    /// <summary>
    /// LateUpdate is called after all Update functions have been called.
    /// This is the ideal place to implement camera-following logic, as it ensures
    /// the target (camera) has completed all of its movement for the current frame.
    /// </summary>
    void LateUpdate()
    {
        if (target == null)
        {
            return; // Exit if the target is somehow lost or not assigned.
        }

        // --- POSITION HANDLING ---

        // 1. We define the desired position. This is calculated by taking the target's current position
        //    and adding its forward direction vector multiplied by the desired distance.
        Vector3 desiredPosition = target.position + (target.forward * distance) + positionOffset;

        // 2. We use Vector3.Lerp (Linear Interpolation) to smoothly move from the UI's current position
        //    towards the desired position. 'smoothSpeed' controls how fast this transition happens.
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;


        // --- ROTATION HANDLING ---

        // 3. We create a target rotation that only uses the camera's horizontal (yaw) rotation.
        //    This ensures the UI stays upright and doesn't tilt up or down with the user's head.
        float targetYRotation = target.eulerAngles.y;
        Quaternion targetBaseRotation = Quaternion.Euler(0, targetYRotation, 0);

        // 4. We apply the rotation offset to the base rotation. The offset is converted from Euler angles to a Quaternion.
        Quaternion targetRotation = targetBaseRotation * Quaternion.Euler(rotationOffset);

        // 5. We use Quaternion.Slerp (Spherical Linear Interpolation) to smoothly rotate from the UI's
        //    current rotation to the target rotation. 'rotationSpeed' controls the pace of this turn.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed);
    }
}
