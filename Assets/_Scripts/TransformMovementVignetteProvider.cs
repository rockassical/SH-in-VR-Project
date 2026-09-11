using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;

public class TransformMovementVignetteProvider :
    MonoBehaviour,
    ITunnelingVignetteProvider
{
    [Header("References")]
    [Tooltip("The parent object that moves the XR rig along the spline.")]
    [SerializeField]
    private Transform movementTarget;

    [SerializeField]
    private TunnelingVignetteController vignetteController;

    [Header("Movement Detection")]
    [Tooltip("Minimum movement speed required to activate the vignette.")]
    [SerializeField]
    private float movementSpeedThreshold = 0.01f;

    [Tooltip("Also activate the vignette when the target rotates.")]
    [SerializeField]
    private bool detectRotation = true;

    [Tooltip("Minimum rotational speed in degrees per second.")]
    [SerializeField]
    private float rotationSpeedThreshold = 1f;

    [Tooltip(
        "How long movement must remain below the thresholds before " +
        "the vignette closes.")]
    [SerializeField]
    private float stopDelay = 0.1f;

    [Header("Vignette Settings")]
    [SerializeField]
    private VignetteParameters parameters = new VignetteParameters();

    public VignetteParameters vignetteParameters => parameters;

    private Vector3 previousPosition;
    private Quaternion previousRotation;

    private float timeSinceMovement;
    private bool vignetteActive;
    private bool initialized;

    private void OnEnable()
    {
        InitializeTracking();
    }

    private void Start()
    {
        InitializeTracking();
    }

    private void LateUpdate()
    {
        if (!initialized ||
            movementTarget == null ||
            vignetteController == null)
        {
            return;
        }

        float deltaTime = Time.unscaledDeltaTime;

        if (deltaTime <= 0f)
        {
            return;
        }

        float movementSpeed =
            Vector3.Distance(
                movementTarget.position,
                previousPosition) / deltaTime;

        float rotationSpeed = 0f;

        if (detectRotation)
        {
            rotationSpeed =
                Quaternion.Angle(
                    previousRotation,
                    movementTarget.rotation) / deltaTime;
        }

        bool isMoving =
            movementSpeed >= movementSpeedThreshold ||
            rotationSpeed >= rotationSpeedThreshold;

        if (isMoving)
        {
            timeSinceMovement = 0f;
            BeginVignette();
        }
        else
        {
            timeSinceMovement += deltaTime;

            if (timeSinceMovement >= stopDelay)
            {
                EndVignette();
            }
        }

        previousPosition = movementTarget.position;
        previousRotation = movementTarget.rotation;
    }

    private void OnDisable()
    {
        EndVignette();
        initialized = false;
    }

    private void InitializeTracking()
    {
        if (movementTarget == null)
        {
            movementTarget = transform;
        }

        previousPosition = movementTarget.position;
        previousRotation = movementTarget.rotation;

        timeSinceMovement = 0f;
        initialized = true;
    }

    private void BeginVignette()
    {
        if (vignetteActive || vignetteController == null)
        {
            return;
        }

        vignetteController.BeginTunnelingVignette(this);
        vignetteActive = true;
    }

    private void EndVignette()
    {
        if (!vignetteActive || vignetteController == null)
        {
            return;
        }

        vignetteController.EndTunnelingVignette(this);
        vignetteActive = false;
    }

    public void SetMovementTarget(Transform newTarget)
    {
        EndVignette();

        movementTarget = newTarget;
        InitializeTracking();
    }
}

