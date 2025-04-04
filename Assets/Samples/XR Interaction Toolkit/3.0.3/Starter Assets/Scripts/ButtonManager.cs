using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button yes, no;
    [SerializeField] private GameObject bg;

    public void yesClick(){
        Destroy(bg);
    }

    public void noClick(){
        Application.Quit();
    }
}
