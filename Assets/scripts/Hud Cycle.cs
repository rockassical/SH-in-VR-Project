using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

public class HudCycle : MonoBehaviour
{
    [SerializeField] private List<GameObject> slides;
    private int index = 0;
    [SerializeField]private GameObject objectWhole;
    
    public void NextSlide(){
        slides[index].SetActive(false);
        index++;
        if(index > slides.Count-1 && objectWhole.activeSelf){
            index = 0;
            Destroy(objectWhole);
            return;
        }
        slides[index].SetActive(true);
    }
}
