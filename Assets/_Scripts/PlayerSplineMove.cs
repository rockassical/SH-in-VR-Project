using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils.Bindings;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using SWS;
using TMPro;

public class PlayerSplineMove : MonoBehaviour
{

    [Header("XR Controller Input")]
    public InputActionReference leftThumbstick;

    [Header("Spline Move Control")]
    public splineMove spline;
    //public float moveSpeed;

    public Vector2 thumbStickInputValue;
    private float minSpeed = 1f;
    //private float maxSpeed = 5f;

    private bool isMoving = false;

    private void OnEnable()
    {
       if (leftThumbstick != null)
        {
            leftThumbstick.action.Enable();
            leftThumbstick.action.performed += AdjustSpeed;
            
        }
    }

    private void OnDisable()
    {
        if (leftThumbstick != null)
        {
            leftThumbstick.action.performed -= AdjustSpeed;
            leftThumbstick.action.Disable();
        }
    }


    private void AdjustSpeed(InputAction.CallbackContext context)
    {
        Vector2 thumbstickValue = context.ReadValue<Vector2>(); // Read the full thumbstick value
        float yInput = thumbstickValue.y; // Extract the Y value

        Debug.Log("yInput is:" + yInput);

        //spline.ChangeSpeed(moveSpeed);
    }

    private void AdjustSpeed2()
    {
        thumbStickInputValue = leftThumbstick.action.ReadValue<Vector2>();   //get controller thumbstick value
        float yInput = thumbStickInputValue.y; // Extract the Y value

        Debug.Log("yInput is:" + yInput);

        if(yInput > 0.1f)
        {
            spline.ChangeSpeed(minSpeed);
            spline.Resume();
        }

        if(yInput < -0.1f)
        {
            spline.ChangeSpeed(-minSpeed);
            spline.Resume();
        }else if (yInput > -0.1f && yInput < 0.1f)
        {
            spline.Pause();
        }


        //moveSpeed = Mathf.Clamp(moveSpeed + yInput / 15, minSpeed, maxSpeed);
        //spline.ChangeSpeed(moveSpeed);
    }

    void Update()
    {
        

        AdjustSpeed2();

        
    }


    public void StartPlayerSplineMove()
    {
        spline.speed = 0.5f;
        spline.StartMove();
    }

}
