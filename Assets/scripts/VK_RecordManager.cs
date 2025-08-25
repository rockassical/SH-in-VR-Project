using UnityEngine;
using System.Collections;
using VideoKit;

[RequireComponent(typeof(VideoKitRecorder))]
[DefaultExecutionOrder(-1000)]
public class VK_RecordManager : MonoBehaviour
{
    [SerializeField, Tooltip("Start recording automatically when this GameObject enables.")]
    private bool autoStart = true;

    private VideoKitRecorder recorder;
    private bool started;
    private bool stopping;

    private void Awake()
    {
        recorder = GetComponent<VideoKitRecorder>();
        if (recorder == null)
        {
            Debug.LogError("[VK_RecordManager] VideoKitRecorder component not found on this GameObject.");
        }
    }

    private void OnEnable()
    {
        if (!autoStart) return;

        if (recorder != null && !recorder.enabled)
            recorder.enabled = true;

        StartCoroutine(StartRecordingNextFrame());
    }

    private IEnumerator StartRecordingNextFrame()
    {
        if (!autoStart || recorder == null || started) yield break;

        // Wait one frame so Unity finishes enabling the component internally
        yield return null;

        if (!isActiveAndEnabled || recorder == null || !recorder.enabled) yield break;

        try
        {
            recorder.StartRecording();
            started = true;
            Debug.Log("[VK_RecordManager] Recording started.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[VK_RecordManager] Failed to start recording: {ex}");
        }
    }

    private void OnApplicationQuit()
    {
        StopIfNeeded();
    }

    private void OnDisable()
    {
        if (autoStart)
            StopIfNeeded();
    }

    private void StopIfNeeded()
    {
        if (recorder == null || stopping || !started) return;
        stopping = true;

        try
        {
            recorder.StopRecording();
            Debug.Log("[VK_RecordManager] Recording stopped (finalizing).");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[VK_RecordManager] Error stopping recording: {ex}");
        }
    }
}