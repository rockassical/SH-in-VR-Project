using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using SWS;

public class JessicaTrigger : MonoBehaviour
{
    [Header("Who to pause")]
    [SerializeField] private string targetTag = "Jessica";

    splineMove _spline;


    [SerializeField] AnimationStateChangeJessica stateChangeW;


    private void OnTriggerEnter(Collider other)
    {
        if (targetTag == "Jessica")
        {

            stateChangeW = other.GetComponent<AnimationStateChangeJessica>();

            if (stateChangeW != null) stateChangeW.SetFalse();

            _spline = other.GetComponentInParent<splineMove>();


            if (_spline != null)
            {
                _spline.Pause();

                GetComponent<BoxCollider>().isTrigger = false;

            }


        }
    }

}