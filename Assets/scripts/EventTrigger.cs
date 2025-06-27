using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition.Attributes;
using UnityEngine.UIElements;

public class EventTrigger : MonoBehaviour
{
    public GameObject locomotion, player, npc, rotation;
    public MeshRenderer rend;
    public Animator playerAnimator, npcAnimator;
    public AudioSource cutsceneAudio;
    public Transform playerTargetPosition, npcTargetPosition;
    private bool fade;
    void OnTriggerEnter(Collider collider)
    {
        locomotion.SetActive(false);
        rotation.SetActive(true);
        StartCoroutine(StartCutscene());
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
        Material fadeMat = rend.material;
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
