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
    public Transform targetTwo;
    public Transform targetThree;
    public Transform targetFour;
    public Transform targetFive;
    public GameObject Jess;
    public GameObject Will;
    void Start()
    {
        moveSpeed = 5;
    }
    private void Update()
    {
        Vector3 velocity = agent.velocity;

        if (velocity.sqrMagnitude > 0.01f) // make sure it's actually moving
        {
            Quaternion lookRotation = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }
    void PlayAnimationAndAudio()
    {
        audioSource.Play();
    }
    public void ButtonActive()
    {
        gameObject.SetActive(true);
        Jess.SetActive(true);
        Will.SetActive(true);
        PlayAnimationAndAudio();
        StartCoroutine(toWalk());
        //gameObject.SetActive(false);
    }
 
     IEnumerator toWalk() 
    {
        animator.SetTrigger("Walk");
        yield return new WaitForSeconds(11f);

        // Move toward the target
        agent.SetDestination(targetPoint.position);

        //Stop moving if close enough
        // Wait until agent reaches destination
        while (agent.pathPending)
            yield return null;
        while (agent.remainingDistance > agent.stoppingDistance + 0.05f)
            yield return null;

        animator.SetTrigger("Idle");
        yield return new WaitForSeconds(26f);
        agent.SetDestination(targetTwo.position);
        yield return new WaitForSeconds(7f);
        agent.SetDestination(targetThree.position);
        yield return new WaitForSeconds(6f);
        agent.SetDestination(targetFour.position);
        yield return new WaitForSeconds(11f);

        animator.SetTrigger("End");
        agent.SetDestination(targetFive.position);


    }
}