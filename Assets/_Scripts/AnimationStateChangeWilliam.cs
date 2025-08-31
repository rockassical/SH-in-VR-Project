using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateChangeWilliam : MonoBehaviour
{
    public Animator animatorW;
    public string parameterW = "isWalking";

    public void SetTrue()
    {
        if (animatorW != null) animatorW.SetBool(parameterW, true);

    }

    public void SetFalse()
    {
        if (animatorW != null) animatorW.SetBool(parameterW, false);
    }

}
