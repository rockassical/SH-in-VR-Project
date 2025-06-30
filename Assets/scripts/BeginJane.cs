using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.AI;
public class BeginJane : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public AudioSource audioSource;
    public Transform targetPoint;
    public float moveSpeed;
    private NavMeshAgent agent;

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
        agent.SetDestination(targetPoint.position);

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