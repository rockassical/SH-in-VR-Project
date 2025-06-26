using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition.Attributes;

public class EventTrigger : MonoBehaviour
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
    }
    //cutscene starts
    //new changes in progress to test 06/26/25

    IEnumerator StartCutscene()
    {
        // Fade to black
        yield return StartCoroutine(FadeScreen(0f, 1f, 1f));

        // Move characters
        player.transform.position = playerTargetPosition.position;
        npc.transform.position = npcTargetPosition.position;

        // Optional: set rotation
        player.transform.rotation = playerTargetPosition.rotation;
        npc.transform.rotation = npcTargetPosition.rotation;

        // Play audio
        if (cutsceneAudio != null)
            cutsceneAudio.Play();

        // Play animations
        if (playerAnimator != null)
            playerAnimator.SetTrigger("CutsceneStart");

        if (npcAnimator != null)
            npcAnimator.SetTrigger("CutsceneStart");

        // Fade back in
        yield return StartCoroutine(FadeScreen(1f, 0f, 1f));

       
    }

    IEnumerator FadeScreen(float from, float to, float duration)
    {
        float elapsed = 0f;
        Material fadeMat = fadeRenderer.material;
        Color color = fadeMat.color;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            fadeMat.color = new Color(color.r, color.g, color.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        fadeMat.color = new Color(color.r, color.g, color.b, to);
    }
}
