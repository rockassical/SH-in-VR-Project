using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UImanager : MonoBehaviour
{
    public static UImanager instance;
    public GameObject tutorialRep, tutorialRecord, reportHud, messageHud;
    public InputActionReference report, message;
    void Awake()
    {
        instance = this;
        // tutorialRep.SetActive(false);
        // tutorialRecord.SetActive(false);
        reportHud.SetActive(false);
        messageHud.SetActive(false);
        report.action.Enable();
        report.action.performed += ToggleReportMenu;
        InputSystem.onDeviceChange += onDeviceChange;
        message.action.Enable();
        message.action.performed += ToggleRecordMenu;
        InputSystem.onDeviceChange += onDeviceChange;
    }

    void OnDestroy()
    {
        report.action.Disable();
        report.action.performed -= ToggleReportMenu;
        InputSystem.onDeviceChange -= onDeviceChange;
        message.action.Disable();
        message.action.performed -= ToggleRecordMenu;
        InputSystem.onDeviceChange -= onDeviceChange;
    }

    private void ToggleReportMenu(InputAction.CallbackContext context){
        reportHud.SetActive(true);
    }
    private void ToggleRecordMenu(InputAction.CallbackContext context){
        messageHud.SetActive(true);
    }

    private void onDeviceChange(InputDevice device, InputDeviceChange change){
        switch(change){
            case InputDeviceChange.Disconnected:
                report.action.Disable();
                report.action.performed -= ToggleReportMenu;
                message.action.Disable();
                message.action.performed -= ToggleRecordMenu;
                break;
            case InputDeviceChange.Reconnected:
                report.action.Enable();
                report.action.performed += ToggleReportMenu;
                message.action.Enable();
                message.action.performed += ToggleRecordMenu;
                break;
        }
    }

}
