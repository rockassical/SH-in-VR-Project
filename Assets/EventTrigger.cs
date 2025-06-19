using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition.Attributes;

public class SceneTrigger : MonoBehaviour
{
    public GameObject locomotion;
    public MeshRenderer rend;
    private bool fade;
    void OnTriggerEnter(Collider collider)
    {
        //start cutscene here and begin animations within cinemachine?
        //fade to black sequence so the rest of the objects and camera can move wo a new scene load
        fade = true;
        float transparency;
        List<Material> list = new List<Material>();
        while (fade)
        {
            rend.GetMaterials(list);
            transparency = 0 + Time.deltaTime;
            if (transparency > 1)
                transparency = 1;
            list[0].color = new Color(0, 0, 0, transparency);
            if (transparency == 1)
                fade = false;
        }
        //move objects to their places such as player and npcs


        //undo fade to black and proceed with cinemachine after
        fade = true;
        {
            rend.GetMaterials(list);
            transparency = -Time.deltaTime;
            if (transparency < 0)
                transparency = 0;
            list[0].color = new Color(0, 0, 0, transparency);
            if (transparency == 0)
                fade = false;
        }
        
        //cutscene starts
    }
}
