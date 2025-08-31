using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;

public class RotationAdjust : MonoBehaviour
{

    splineMove _spline;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeOrientation()
    {
        transform.Rotate(0f, -90f, 0f, Space.Self);

        _spline = GetComponent<splineMove>();
        _spline.enabled = false;
    }

    public void ChangeOrientationJessica()
    {
        transform.Rotate(0f, 55f, 0f, Space.Self);

        _spline = GetComponent<splineMove>();
        _spline.enabled = false;
    }
}
