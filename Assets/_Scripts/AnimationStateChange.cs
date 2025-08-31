using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateChange : MonoBehaviour
{
    public Animator animator;
    public string parameter = "isWalking";

    public void SetTrue()
    {
        if (animator != null) animator.SetBool(parameter, true);

        Debug.Log(parameter);
    }

    public void SetFalse()
    {
        if (animator != null) animator.SetBool(parameter, false);
    }

    
}
