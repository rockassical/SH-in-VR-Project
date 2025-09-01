using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject bg,loco;
    public void lockTrigger()
    {
        switchLockLocomotion(loco.activeSelf);
    }
    public void switchLockLocomotion(bool active)
    {
        loco.SetActive(!active);
    }
    public void yesClick(){
        bg.SetActive(false);
        //switchLockLocomotion(loco.activeSelf);
    }

    
    
    public void noClick(){
        Application.Quit();
    }
}
