using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.AI;
public class WilliamIntri : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Transform targetPoint;
    public float moveSpeed;
    public NavMeshAgent agent;

    public GameObject DWill;
    public GameObject DJess;
    void Start()
    {
        moveSpeed = 5;
        StartCoroutine(toWalk());
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

    public void ButtonActive()
    {
        // StartCoroutine(toWalk());
    }

    IEnumerator toWalk()
    {
        animator.SetTrigger("Intro");
        yield return new WaitForSeconds(20f);
        animator.SetTrigger("Walk");
        yield return new WaitForSeconds(3f);
        // Move toward the target
        agent.SetDestination(targetPoint.position);

        //Stop moving if close enough
        // Wait until agent reaches destination
        while (agent.pathPending)
            yield return null;
        while (agent.remainingDistance > agent.stoppingDistance + 0.05f)
            yield return null;

        gameObject.SetActive(false);
        DJess.SetActive(true);
        DWill.SetActive(true);
    }
}
