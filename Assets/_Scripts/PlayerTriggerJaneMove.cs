using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using SWS;

public class PlayerTriggerJaneMove : MonoBehaviour
{
    [SerializeField] private string targetTag;

    public splineMove _splineJane;
    public splineMove _splinePlayer;
    public float waitTime;
    //public PlayableDirector _director;

    //public bool hasEverPlayed = false;

    [SerializeField] AnimationStateChange stateChange;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_splineJane.IsPaused())
            {
                StartCoroutine(ResumeJaneIntrodution(waitTime));
                GetComponent<BoxCollider>().isTrigger = false;
            }
        }
    }

    IEnumerator ResumeJaneIntrodution(float t)
    {

        yield return new WaitForSeconds(t);
        stateChange.SetTrue();
        _splineJane.Resume();
       
    }

}
