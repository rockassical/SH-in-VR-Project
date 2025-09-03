using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothYAxisRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("The target angle on the Y-axis to rotate to, in degrees.")]
    public float targetAngleY = 147.0f;

    [Tooltip("Controls the speed of the rotation. Higher values are faster.")]
    public float rotationSpeed = 2.0f;

    [Header("Status")]
    [Tooltip("Is true when the rotation is complete.")]
    public bool isRotationComplete = true;

    private Coroutine rotationCoroutine;

    /// <summary>
    /// Public method to begin the rotation process.
    /// It can be called from other scripts, UI buttons, or events.
    /// </summary>
    public void StartRotation()
    {
        // If a rotation is already in progress, stop it before starting a new one.
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }

        isRotationComplete = false;

        // Start the new rotation coroutine.
        rotationCoroutine = StartCoroutine(RotateToAngle());
    }

    /// <summary>
    /// The coroutine that performs the smooth rotation over several frames.
    /// </summary>
    private IEnumerator RotateToAngle()
    {
        // Define the target rotation as a Quaternion.
        // Quaternion.Euler converts degrees into a rotation.
        Quaternion targetRotation = Quaternion.Euler(0, targetAngleY, 0);

        // Continue looping as long as the object's current rotation is not yet at the target.
        // A small threshold is used to prevent floating point inaccuracies.
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            // Slerp (Spherical Linear Interpolation) provides a smooth rotation from the
            // current rotation towards the target rotation.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Wait for the next frame before continuing the loop.
            yield return null;
        }

        // After the loop, snap to the final rotation to ensure it's precise.
        transform.rotation = targetRotation;
        isRotationComplete = true;
        Debug.Log("Rotation complete.");
    }
}
