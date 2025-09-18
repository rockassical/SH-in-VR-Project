// VK_RecordManager.cs

using UnityEngine;
using System.Collections;
using VideoKit;
using System.IO;

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

        // Print expected save folders (Editor + Quest defaults)
        string editorRoot = Directory.GetParent(Application.dataPath)?.FullName;
        string[] expectedDirs = new string[]
        {
            Path.Combine(Application.persistentDataPath, "VideoKit/recordings"),
            Path.Combine(Application.persistentDataPath, "recordings"),
            editorRoot != null ? Path.Combine(editorRoot, "Captures") : null,
            editorRoot != null ? Path.Combine(editorRoot, "recordings") : null,
            editorRoot != null ? Path.Combine(editorRoot, "VideoKit", "recordings") : null,
        };

        Debug.Log("[VK_RecordManager] Expected recordings folders:\n  " +
                  string.Join("\n  ", expectedDirs));
    }

    void OnEnable()
    {
        if (!autoStart || recorder == null) return;
        if (!recorder.enabled) recorder.enabled = true;
        StartCoroutine(StartNextFrame());
    }

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