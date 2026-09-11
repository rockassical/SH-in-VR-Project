using UnityEngine;
using UnityEngine.InputSystem;

public class UImanager : MonoBehaviour
{
    public static UImanager instance;

    [Header("Tutorial UI")]
    public GameObject tutorialRep;
    public GameObject tutorialRecord;

    [Header("Main UI")]
    public GameObject reportHud;
    public GameObject messageHud;

    [Header("Input Actions")]
    public InputActionReference report;
    public InputActionReference message;

    [Header("Tutorial HUD Cycles")]
    public HudCycle aTutorialCycle;
    public HudCycle bTutorialCycle;

    private bool tutorialOver = false;
    private bool tutorialAButtonEnabled = false;
    private bool tutorialBButtonEnabled = false;

    private void Awake()
    {
        instance = this;

        reportHud.SetActive(false);
        messageHud.SetActive(false);

        SubscribeToInput();
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDestroy()
    {
        UnsubscribeFromInput();
        InputSystem.onDeviceChange -= OnDeviceChange;

        if (instance == this)
        {
            instance = null;
        }
    }

    private void SubscribeToInput()
    {
        if (report != null && report.action != null)
        {
            report.action.performed -= HandleReportButton;
            report.action.performed += HandleReportButton;
            report.action.Enable();
        }

        if (message != null && message.action != null)
        {
            message.action.performed -= HandleMessageButton;
            message.action.performed += HandleMessageButton;
            message.action.Enable();
        }
    }

    private void UnsubscribeFromInput()
    {
        if (report != null && report.action != null)
        {
            report.action.performed -= HandleReportButton;
            report.action.Disable();
        }

        if (message != null && message.action != null)
        {
            message.action.performed -= HandleMessageButton;
            message.action.Disable();
        }
    }

    private void HandleReportButton(InputAction.CallbackContext context)
    {
        if (!tutorialOver)
        {
            if (!tutorialBButtonEnabled)
            {
                return;
            }

            tutorialBButtonEnabled = false;

            if (aTutorialCycle != null)
            {
                bTutorialCycle.NextSlide();
            }

            return;
        }

        reportHud.SetActive(true);
    }

    private void HandleMessageButton(InputAction.CallbackContext context)
    {
        if (!tutorialOver)
        {
            if (!tutorialAButtonEnabled)
            {
                return;
            }

            tutorialAButtonEnabled = false;

            if (bTutorialCycle != null)
            {
                aTutorialCycle.NextSlide();
            }

            return;
        }

        messageHud.SetActive(true);
    }

    private void OnDeviceChange(
        InputDevice device,
        InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Disconnected:
                UnsubscribeFromInput();
                break;

            case InputDeviceChange.Reconnected:
                SubscribeToInput();
                break;
        }
    }

    public void EnableAButtonForTutorial()
    {
        if (tutorialOver)
        {
            return;
        }

        tutorialAButtonEnabled = true;
    }

    public void EnableBButtonForTutorial()
    {
        if (tutorialOver)
        {
            return;
        }

        tutorialBButtonEnabled = true;
    }

    public void TutorialOver()
    {
        tutorialOver = true;

        tutorialAButtonEnabled = false;
        tutorialBButtonEnabled = false;

        if (tutorialRep != null)
        {
            tutorialRep.SetActive(false);
        }

        if (tutorialRecord != null)
        {
            tutorialRecord.SetActive(false);
        }
    }
}
