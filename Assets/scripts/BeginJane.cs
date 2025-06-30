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
    public NavMeshAgent agent;

    void Start()
    {
        moveSpeed = 5;
    }
    private void Update()
    {
        Vector3 velocity = agent.velocity;

        if (velocity.sqrMagnitude > 0.01f) // make sure it's actually moving
        {
            Quaternion lookRotation = Quaternion.LookRotation(-velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
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
        //gameObject.SetActive(false);
    }
 
     IEnumerator toWalk() 
    {
        animator.SetTrigger("Walk");
        yield return new WaitForSeconds(12.83f);

        // Move toward the target
        agent.SetDestination(targetPoint.position);

        //Stop moving if close enough
        // Wait until agent reaches destination
        while (agent.pathPending)
            yield return null;
        while (agent.remainingDistance > agent.stoppingDistance + 0.05f)
            yield return null;

        gameObject.SetActive(false);
    }
}