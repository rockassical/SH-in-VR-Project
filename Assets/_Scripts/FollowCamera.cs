using System.Collections;
using System.Collections.Generic;
// FollowCamera.cs
using UnityEngine;

[DefaultExecutionOrder(50)]
public class FollowCamera : MonoBehaviour
{
    public enum OffsetSpace { World, Local }

    [Header("Target")]
    [SerializeField] Transform target;
    [Tooltip("Point on the target to focus (eg. chest/head).")]
    [SerializeField] Vector3 lookAtOffset = new Vector3(0f, 1.6f, 0f);

    [Header("Position")]
    [Tooltip("Camera offset from target (use Local to stay behind character).")]
    [SerializeField] OffsetSpace offsetSpace = OffsetSpace.Local;
    [SerializeField] Vector3 offset = new Vector3(0f, 2f, -5f);
    [Tooltip("Smooth follow time (lower = snappier).")]
    [SerializeField, Min(0f)] float positionSmoothTime = 0.12f;

    [Header("Rotation")]
    [Tooltip("Smoothly rotate to look at the target.")]
    [SerializeField] bool lookAtTarget = true;
    [SerializeField, Range(0f, 20f)] float rotationLerpSpeed = 8f;

    [Header("Collision")]
    [Tooltip("Layers considered as obstacles between camera and target.")]
    [SerializeField] LayerMask obstacleLayers = ~0;
    [Tooltip("Radius for spherecast to avoid clipping through walls.")]
    [SerializeField, Min(0f)] float collisionRadius = 0.2f;
    [Tooltip("Extra distance kept from hit surface.")]
    [SerializeField, Min(0f)] float collisionBuffer = 0.1f;

    [Header("Misc")]
    [Tooltip("If target is missing, script will try to find MainCamera's parent once at runtime.")]
    [SerializeField] bool tryAutoAssignTarget = false;

    Vector3 _velocity;

    void Reset()
    {
        // Try to guess target if placed as child of character
        if (!target && transform.parent) target = transform.parent;
    }

    void Awake()
    {
        if (!target && tryAutoAssignTarget)
        {
            var t = GameObject.FindGameObjectWithTag("Player");
            if (t) target = t.transform;
        }
    }

    void LateUpdate()
    {
        if (!target) return;

        // 1) Compute desired camera position (local or world-space offset)
        Vector3 desiredPos = (offsetSpace == OffsetSpace.Local)
            ? target.TransformPoint(offset)
            : target.position + offset;

        // 2) Maintain line of sight with simple spherecast collision handling
        Vector3 focusPoint = target.position + lookAtOffset;
        Vector3 dir = desiredPos - focusPoint;
        float desiredDist = dir.magnitude;
        Vector3 safePos = desiredPos;

        if (desiredDist > 0.001f)
        {
            dir /= desiredDist; // normalize
            if (Physics.SphereCast(focusPoint, collisionRadius, dir, out RaycastHit hit, desiredDist, obstacleLayers, QueryTriggerInteraction.Ignore))
            {
                safePos = hit.point - dir * (collisionBuffer);
            }
        }

        // 3) Smoothly move camera
        transform.position = Vector3.SmoothDamp(transform.position, safePos, ref _velocity, positionSmoothTime);

        // 4) Smoothly rotate to look at target
        if (lookAtTarget)
        {
            Vector3 toTarget = (focusPoint - transform.position);
            if (toTarget.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 1f - Mathf.Exp(-rotationLerpSpeed * Time.deltaTime));
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!target) return;
        Gizmos.color = Color.cyan;

        Vector3 desiredPos = (offsetSpace == OffsetSpace.Local)
            ? target.TransformPoint(offset)
            : target.position + offset;

        Vector3 focusPoint = target.position + lookAtOffset;
        Gizmos.DrawWireSphere(focusPoint, 0.05f);
        Gizmos.DrawLine(focusPoint, desiredPos);
        Gizmos.DrawWireSphere(desiredPos, collisionRadius);
    }
#endif
}
