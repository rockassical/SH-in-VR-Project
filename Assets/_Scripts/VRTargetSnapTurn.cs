using UnityEngine;

public class VRTargetSnapTurn : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform target;

    [Header("Snap Turn Settings")]
    [SerializeField] private float snapAngle = 90f;
    [SerializeField] private float triggerAngle = 90f;

    [Tooltip("Minimum time between automatic snap turns.")]
    [SerializeField] private float snapCooldown = 0.5f;

    private bool autoTurnEnabled = false;
    private float cooldownTimer = 0f;

    private void Update()
    {
        if (!autoTurnEnabled || playerCamera == null || target == null)
            return;

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            return;
        }

        CheckForSnapTurn();
    }

    private void CheckForSnapTurn()
    {
        // Flatten both directions onto the horizontal plane.
        Vector3 forward = playerCamera.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 toTarget = target.position - playerCamera.position;
        toTarget.y = 0f;
        toTarget.Normalize();

        // Signed angle from player's current view to target.
        float angle = Vector3.SignedAngle(forward, toTarget, Vector3.up);

        if (Mathf.Abs(angle) >= triggerAngle)
        {
            // Snap in the direction of the target.
            float turnAmount = Mathf.Sign(angle) * snapAngle;

            transform.RotateAround(
                playerCamera.position,
                Vector3.up,
                turnAmount
            );

            cooldownTimer = snapCooldown;
        }
    }

    /// <summary>
    /// Enables automatic snapping toward the target.
    /// </summary>
    public void EnableAutoTurn()
    {
        autoTurnEnabled = true;
        cooldownTimer = 0f;
    }

    /// <summary>
    /// Disables automatic snapping.
    /// </summary>
    public void DisableAutoTurn()
    {
        autoTurnEnabled = false;
    }

    /// <summary>
    /// Immediately performs a snap turn toward the target.
    /// </summary>
    public void SnapTowardTarget()
    {
        if (playerCamera == null || target == null)
            return;

        Vector3 forward = playerCamera.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 toTarget = target.position - playerCamera.position;
        toTarget.y = 0f;
        toTarget.Normalize();

        float angle = Vector3.SignedAngle(forward, toTarget, Vector3.up);

        if (Mathf.Abs(angle) > 0.01f)
        {
            float turnAmount = Mathf.Sign(angle) * snapAngle;

            transform.RotateAround(
                playerCamera.position,
                Vector3.up,
                turnAmount
            );
        }
    }

    public void SnapTowardTempTarget(Transform tempTarget)
    {
        if (playerCamera == null || tempTarget == null)
            return;

        Vector3 forward = playerCamera.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 toTarget = tempTarget.position - playerCamera.position;
        toTarget.y = 0f;
        toTarget.Normalize();

        float angle = Vector3.SignedAngle(forward, toTarget, Vector3.up);

        if (Mathf.Abs(angle) > 0.01f)
        {
            float turnAmount = Mathf.Sign(angle) * snapAngle;

            transform.RotateAround(
                playerCamera.position,
                Vector3.up,
                turnAmount
            );
        }
    }
}