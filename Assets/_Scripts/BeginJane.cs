using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using SWS;



public class BeginJane : MonoBehaviour
{

    //public Animator animator;
    public AudioSource audioSource;
    //public Transform targetPoint;
    //public float moveSpeed;
    //public NavMeshAgent agent;
    //public Transform targetTwo;
    //public Transform targetThree;
    // public Transform targetFour;
    //public Transform targetFive;

    public GameObject Jane;
    public GameObject Jessica;
    public GameObject Will;
    public float waitTime;

    public PlayableDirector _directorJane;


    void Start()
    {
        //moveSpeed = 5;
    }
    private void Update()
    {
        /*Vector3 velocity = agent.velocity;

        if (velocity.sqrMagnitude > 0.01f) // make sure it's actually moving
        {
            Quaternion lookRotation = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
        */
    }
    void PlayAnimationAndAudio()
    {
        audioSource.Play();
    }
    public void ButtonActive()
    {
        gameObject.SetActive(true);
        Jessica.SetActive(true);
        Will.SetActive(true);
        //PlayAnimationAndAudio();
        //StartCoroutine(toWalk());
        //gameObject.SetActive(false);
    }


    public void BeginExperience()
    {
        gameObject.SetActive(true);
        StartCoroutine(StartExperience(waitTime));
    }


    IEnumerator StartExperience(float t)
    {
        yield return new WaitForSeconds(t);

        
        Jessica.SetActive(true);
        Will.SetActive(true);
        Jane.SetActive(true); 
        _directorJane.Play(); 


    }



 /*
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
*/

}