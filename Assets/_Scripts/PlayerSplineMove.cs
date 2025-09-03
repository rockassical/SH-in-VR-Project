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
    public float minSpeed = 0.5f;
    //private float maxSpeed = 5f;

    public Transform target;
    public float rotationSpeed = 5f;
    private bool isRotationAligned = false;

    public bool lockXAxis = false;
    public bool lockYAxis = false;
    public bool lockZAxis = true;

    public GameObject player;
    public SmoothYAxisRotation rotationScript;

    private void OnEnable()
    {
       if (leftThumbstick != null)
        {
            leftThumbstick.action.Enable();
            //leftThumbstick.action.performed += AdjustSpeed;
            
        }
    }

    private void OnDisable()
    {
        if (leftThumbstick != null)
        {
            //leftThumbstick.action.performed -= AdjustSpeed;
            leftThumbstick.action.Disable();
        }
    }


    private void AdjustSpeed(InputAction.CallbackContext context)
    {
        Vector2 thumbstickValue = context.ReadValue<Vector2>(); // Read the full thumbstick value
        float yInput = thumbstickValue.y; // Extract the Y value

        //Debug.Log("yInput is:" + yInput);

        //spline.ChangeSpeed(moveSpeed);
    }

    private void AdjustSpeed2()
    {
        thumbStickInputValue = leftThumbstick.action.ReadValue<Vector2>();   //get controller thumbstick value
        float yInput = thumbStickInputValue.y; // Extract the Y value

        //Debug.Log("yInput is:" + yInput);

        

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

        //RotationAdjustment();
        AdjustSpeed2();

        if (isRotationAligned == true)
        {

        


        }


        
    }


    public void RotationAdjustment()
    {
        // Exit early if no target is assigned to prevent errors.
        if (target == null)
        {
            return;
        }

        
        Vector3 directionToTarget = target.position - transform.position;

        // Ensure the direction is not a zero vector.
        if (directionToTarget == Vector3.zero)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        // --- Step 3: Apply axis locks (optional) ---
        // Get the current rotation in Euler angles.
        Vector3 currentEulerAngles = transform.rotation.eulerAngles;
        // Get the target rotation in Euler angles.
        Vector3 targetEulerAngles = targetRotation.eulerAngles;

        // If an axis is locked, we keep the object's current rotation for that axis.
        float finalX = lockXAxis ? currentEulerAngles.x : targetEulerAngles.x;
        float finalY = lockYAxis ? currentEulerAngles.y : targetEulerAngles.y;
        float finalZ = lockZAxis ? currentEulerAngles.z : targetEulerAngles.z;

        // Recreate the target rotation from the potentially modified Euler angles.
        Quaternion finalRotation = Quaternion.Euler(finalX, finalY, finalZ);

        transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, rotationSpeed * Time.deltaTime);

        isRotationAligned = true;
    }

    public void StartPlayerSplineMove()
    {
        //player.transform.rotation = target.transform.rotation;

        if(rotationScript.isRotationComplete) 
        spline.speed = 0.5f;
        spline.StartMove();
    }


    void LateUpdate()
    {
    }    

}
