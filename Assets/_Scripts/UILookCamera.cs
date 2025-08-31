using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookCamera : MonoBehaviour
{
    public Transform mCamera;
    private Transform localTrans;
    // Start is called before the first frame update
    void Start()
    {
        localTrans = GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (mCamera != null)
        {
            localTrans.LookAt(2 * localTrans.position - mCamera.position);
        }
    }
}
