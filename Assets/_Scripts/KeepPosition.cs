using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepPosition : MonoBehaviour
{
    [Header("History Settings")]
    [Tooltip("How many frames of position history to store.")]
    [Range(1, 60)]
    public int frameDelay = 30;

    // A Queue is a "First-In, First-Out" collection, perfect for storing a
    // chronological history of positions.
    private Queue<Vector3> positionHistory = new Queue<Vector3>();
    private Queue<Quaternion> rotationHistory = new Queue<Quaternion>();


    void LateUpdate()
    {
        // Record the current position at the end of every frame.
        positionHistory.Enqueue(transform.position);
        rotationHistory.Enqueue(transform.rotation);

        // If the queue has grown larger than our desired history size,
        // remove the oldest entry to maintain the correct delay.
        if (positionHistory.Count > frameDelay)
        {
            positionHistory.Dequeue();
        }

        if (rotationHistory.Count > frameDelay)
        {
            rotationHistory.Dequeue();
        }
    }

    /// <summary>
    /// Instantly moves the GameObject to the position it was at 'frameDelay' frames ago.
    /// </summary>
    public void ApplyPastTransform()
    {
        // Check if there is any history recorded and if the history is full.
        // We check for Count == frameDelay to ensure we have a position from the exact frame we want.
        if (positionHistory.Count == frameDelay)
        {
            // Peek() looks at the oldest item in the queue (the one from 'frameDelay' frames ago)
            // without removing it. We then set our current position to that value.
            transform.position = positionHistory.Peek();
            transform.rotation = rotationHistory.Peek();
            Debug.Log($"Rewinding to past transform. Position: {transform.position}, Rotation: {transform.rotation.eulerAngles}");
        }
        else
        {
            Debug.LogWarning("Cannot apply past position. The position history buffer is not full yet.");
        }
    }

}
