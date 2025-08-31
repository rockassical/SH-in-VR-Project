using UnityEngine;
using System.Collections;
using VideoKit;

[RequireComponent(typeof(VideoKitRecorder))]
[DefaultExecutionOrder(-1000)]
public class VK_RecordManager : MonoBehaviour
{
    [SerializeField] bool autoStart = true;

    VideoKitRecorder recorder;
    bool started;

    void Awake()
    {
        recorder = GetComponent<VideoKitRecorder>();
        string expected = System.IO.Path.Combine(Application.persistentDataPath, "VideoKit/recordings");
        Debug.Log($"[VK_RecordManager] Expected recordings folder: {expected}");
    }

    void OnEnable()
    {
        if (!autoStart || recorder == null) return;
        if (!recorder.enabled) recorder.enabled = true;
        StartCoroutine(StartNextFrame());
    }

    void OnApplicationQuit() { if (started && recorder) recorder.StopRecording(); }

    IEnumerator StartNextFrame()
    {
        if (started) yield break;
        yield return null; // wait one frame so component is fully enabled
        if (!isActiveAndEnabled || recorder == null || !recorder.enabled) yield break;

        recorder.StartRecording();
        started = true;
        Debug.Log("[VK_RecordManager] Recording started.");
    }
}