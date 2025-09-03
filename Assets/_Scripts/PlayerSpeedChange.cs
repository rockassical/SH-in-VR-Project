using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;

public class PlayerSpeedChange : MonoBehaviour
{

    [SerializeField] splineMove _splinePlayer;
   public PlayerSplineMove playerSplineControll;

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _splinePlayer = other.gameObject.GetComponentInParent<splineMove>();

            playerSplineControll.minSpeed = 1f;

            this.gameObject.GetComponent<BoxCollider>().enabled = false;
        }
            
        
    }
}
