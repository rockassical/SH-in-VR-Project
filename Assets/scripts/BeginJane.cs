using UnityEngine;
using UnityEngine.UI;

public class BeginJane : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public string animationTrigger = "Click"; 
    public AudioSource audioSource;
    public Button triggerButton;

    void Start()
    {
        if (triggerButton != null)
        {
            triggerButton.onClick.AddListener(PlayAnimationAndAudio);
        }
    }

    void PlayAnimationAndAudio()
    {
        if (animator != null)
        {
            animator.SetTrigger(animationTrigger);
        }

        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
