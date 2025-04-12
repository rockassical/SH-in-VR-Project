using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject bg,loco;

    public void yesClick(){
        Destroy(bg);
        loco.SetActive(true);
    }

    public void noClick(){
        Application.Quit();
    }
}
