using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using SWS;

public class JaneTrigger : MonoBehaviour
{
    [Header("Who to pause")]
    [SerializeField] private string targetTag = "Jane";

    splineMove _spline;
    public PlayableDirector _director;

    public bool hasEverPlayed = false;

    [SerializeField] AnimationStateChange stateChange;

    private void Awake()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(targetTag == "Jane")
        {
            
           stateChange = other.GetComponent<AnimationStateChange>(); 

            if(stateChange != null) stateChange.SetFalse();
            
            _spline = other.GetComponentInParent<splineMove>();
            //if (_director != null && _director.time > 0.0001) hasEverPlayed = true;

            if (_spline != null)
            {
                _spline.Pause();

                GetComponent<BoxCollider>().isTrigger = false;
                
            }

            
            if (_director.state == PlayState.Paused && hasEverPlayed)
                _director.Resume();

            if (!hasEverPlayed)
                _director.Play();
                hasEverPlayed = true;

        }
    }


}
