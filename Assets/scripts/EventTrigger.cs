using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition.Attributes;
using UnityEngine.UIElements;

public class EventTrigger : MonoBehaviour
{
//<<<<<<< Updated upstream
    //Reece: Commented out to prevent redundancy in activating rotation. 
    //public GameObject rotation;
//=======
    public GameObject player;

    Transform playerTargetPosition;
    Transform npcTargetPosition;

    public GameObject locomotion; //Reece: In the inspector I assigned the Move child instead to allow for rotation while in chair.
//>>>>>>> Stashed changes
    public MeshRenderer rend;
    public Animator npcOneAnimator, npcTwoAnimator;
    public AudioSource cutsceneAudio;
    public Animator jess;
    public Animator william;
    //boolean for testing
    public bool testMode = false;
    // public Transform playerTargetPosition, npcTargetPosition;
    private bool fade;
    void OnTriggerEnter(Collider collider)
    {
        locomotion.SetActive(false);//turn off movement
        //rotation.SetActive(true);
        StartCoroutine(StartCutscene());
    }
    //cutscene starts
    private void Start()
    {
        if (testMode)   
        {
            
            if (cutsceneAudio != null)
                cutsceneAudio.Play();
            william.SetTrigger("scene");
            jess.SetTrigger("scene"); 
        }
    }
    IEnumerator StartCutscene()
    {
        // Fade to black
        yield return StartCoroutine(FadeScreen(0f, 1f, 1f));

        // Play audio
        if (cutsceneAudio != null)
            cutsceneAudio.Play();

        // Play animations
        william.SetTrigger("scene");
        jess.SetTrigger("scene");
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
