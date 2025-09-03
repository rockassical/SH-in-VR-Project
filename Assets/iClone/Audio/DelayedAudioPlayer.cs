using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedAudioPlayer : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("The audio clip to play after the delay.")]
    public AudioClip clipToPlay;

    [Tooltip("The delay in seconds before the audio clip is played.")]
    public float delayInSeconds = 3.0f;

    private AudioSource audioSource;

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        // Get the AudioSource component attached to this GameObject.
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update.
    void Start()
    {
        // Start the coroutine that will handle the wait and then play the sound.
        StartCoroutine(PlaySoundAfterDelay());
    }

    /// <summary>
    /// A coroutine that waits for a set duration and then plays the audio clip.
    /// </summary>
    private IEnumerator PlaySoundAfterDelay()
    {
        // First, check if a clip has actually been assigned in the Inspector.
        if (clipToPlay == null)
        {
            Debug.LogWarning("No AudioClip assigned to DelayedAudioPlayer on " + gameObject.name);
            yield break; // Exit the coroutine early if there's no clip.
        }

        // Wait for the specified number of seconds.
        yield return new WaitForSeconds(delayInSeconds);

        // Assign the clip to the AudioSource component.
        audioSource.clip = clipToPlay;

        // Play the audio.
        audioSource.Play();
        Debug.Log("Playing audio clip after " + delayInSeconds + " seconds.");
    }
}
