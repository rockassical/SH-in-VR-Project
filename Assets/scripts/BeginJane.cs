using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class BeginJane : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public AudioSource audioSource;
    public Transform targetPoint;
    public float moveSpeed;
    
    void Start()
    {
        moveSpeed = 5;
      
    }
    void PlayAnimationAndAudio()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
    public void ButtonActive()
    {
        gameObject.SetActive(true);
        PlayAnimationAndAudio();
        StartCoroutine(toWalk());
        MoveToTarget();
    }
    void MoveToTarget()
    {
        if (targetPoint == null) return;
        animator.SetTrigger("Walk");

        // Move toward the target
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint.position,
            moveSpeed * Time.deltaTime
        );

        // Stop moving if close enough
        if (Vector3.Distance(transform.position, targetPoint.position) <= 0.1)
        {
            gameObject.SetActive(false);
        }
    }
     IEnumerator toWalk() 
    {
        yield return new WaitForSeconds(10f);
 
    }
}