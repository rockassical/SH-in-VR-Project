using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using SWS;

public class EventTrigger : MonoBehaviour
{
//<<<<<<< Updated upstream
    //Reece: Commented out to prevent redundancy in activating rotation. 
    //public GameObject rotation;
//=======
    public GameObject player;
    public GameObject npc;

    Transform playerTargetPosition;
    Transform npcTargetPosition;

    public GameObject locomotion; //Reece: In the inspector I assigned the Move child instead to allow for rotation while in chair.
                                  //>>>>>>> Stashed changes
    public GameObject startCutsceneUI;
    public GameObject sitHereUI;
    public MeshRenderer rend;
    public Animator npcOneAnimator, npcTwoAnimator;
    public AudioSource cutsceneAudio;
    public Animator jess;
    public Animator william;

    public PlayableDirector jwDirector;

   // public Transform playerTargetPosition, npcTargetPosition;
    //private bool fade;
    void OnTriggerEnter(Collider collider)
    {
        locomotion.SetActive(false);//turn off movement
        //rotation.SetActive(true);
        if (startCutsceneUI.activeSelf == false)
        {
            startCutsceneUI.SetActive(true);
            sitHereUI.SetActive(false);
        }
    }

    //This will be called whenever the StartScenario UI button is pressed.
    public void StartCutscene()
    {
        jwDirector.Resume();
        
        //StartCoroutine(StartCutscene());
    }
    //cutscene starts
    //new changes in progress to test 06/26/25
    /*
    IEnumerator StartCutscene()
    {
        // Fade to black
        //yield return StartCoroutine(FadeScreen(0f, 1f, 1f));

        //// Move characters
        //player.transform.position = playerTargetPosition.position;
        //npc.transform.position = npcTargetPosition.position;

        //// Optional: set rotation
        //player.transform.rotation = playerTargetPosition.rotation;
        //npc.transform.rotation = npcTargetPosition.rotation;

        // Play audio
        if (cutsceneAudio != null)
            cutsceneAudio.Play();

        // Play animations
        william.SetBool("startScene", true);
        jess.SetBool("startScene", true);
        // Fade back in
        yield return StartCoroutine(FadeScreen(1f, 0f, 1f));

       
    }  */

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
