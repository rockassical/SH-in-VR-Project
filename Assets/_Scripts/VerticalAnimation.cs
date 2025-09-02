using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("The maximum vertical distance the object will move up or down from its starting point.")]
    [Range(0.1f, 2f)]
    public float amplitude = 0.5f;

    [Tooltip("The speed of the hover animation. Higher values result in faster movement.")]
    [Range(0.1f, 5f)]
    public float frequency = 1f;

    // The initial position of the GameObject when the script starts.
    private Vector3 _startPosition;

    void Start()
    {
        // Store the object's starting position so our animation is always relative to it.
        _startPosition = transform.position;
    }

    void Update()
    {
        // --- Calculate the new vertical position ---
        // 1. Mathf.Sin() creates a smooth wave that oscillates between -1 and 1.
        // 2. Time.time * frequency controls the speed of the wave.
        // 3. We multiply by 'amplitude' to control the height of the wave.
        float verticalOffset = Mathf.Sin(Time.time * frequency) * amplitude;

        // --- Apply the new position ---
        // We create a new position vector by taking the original starting position
        // and adding our calculated vertical offset to its Y component.
        transform.position = _startPosition + new Vector3(0, verticalOffset, 0);
    }
}
