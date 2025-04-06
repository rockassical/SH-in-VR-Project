using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

public class HudCycle : MonoBehaviour
{
    [SerializeField] private List<GameObject> slides;
    private int index = 0;
    [SerializeField]private Button button;
    void Update()
    {
        button = slides[index].GetComponentInChildren<Button>();
    }

    public void onClick(){
        slides[index].SetActive(false);
        index++;
        if(index > slides.Count){
            index = 0;
        }
        slides[index].SetActive(true);
    }
}
