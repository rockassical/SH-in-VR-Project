using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using SWS;

public class WilliamTrigger : MonoBehaviour
{
    [Header("Who to pause")]
    [SerializeField] private string targetTag = "William";

    splineMove _spline;
    
        
    [SerializeField] AnimationStateChangeWilliam stateChangeW;


    private void OnTriggerEnter(Collider other)
    {
        if(targetTag == "William")
        {
            
           stateChangeW = other.GetComponent<AnimationStateChangeWilliam>(); 

            if(stateChangeW != null) stateChangeW.SetFalse();
            
            _spline = other.GetComponentInParent<splineMove>();
         

            if (_spline != null)
            {
                _spline.Pause();

                GetComponent<BoxCollider>().isTrigger = false;
                
            }
         
            
        }
    }

}
