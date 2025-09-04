// VK_RelocateVideo.cs
// Quest Pro: auto-start record; on pause/quit finalize & copy to /sdcard/Movies/<PublicSubdir>/
// Put this on the SAME GameObject as VideoKitRecorder.

using UnityEngine;
using System.Collections;
using System.IO;
using VideoKit;

[RequireComponent(typeof(VideoKitRecorder))]
[DefaultExecutionOrder(-1000)]
public class VK_RelocateVideo : MonoBehaviour
{
    [Header("Behavior")]
    [SerializeField] bool autoStart = true;
    [SerializeField] float minRecordingSeconds = 0.5f;   // avoid zero-length finalize
    [SerializeField] float finalizeTimeout = 10f;        // seconds to wait for file to appear

    [Header("Public Folder")]
    [SerializeField] string publicSubdir = "SHinVR";     // final path: /sdcard/Movies/SHinVR/

    VideoKitRecorder recorder;
    bool started, finalizing;
    float startedAt;
    string privateDir, publicDir;

    void Awake()
    {
        recorder = GetComponent<VideoKitRecorder>();
        privateDir = Path.Combine(Application.persistentDataPath, "VideoKit/recordings");
        publicDir = Path.Combine("/storage/emulated/0/Movies", publicSubdir);

        Debug.Log($"[VK_RecordAndRelocate] Private: {privateDir}");
        Debug.Log($"[VK_RecordAndRelocate] Public:  {publicDir}");
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
        yield return null; // wait 1 frame so the component is fully enabled
        if (!isActiveAndEnabled || recorder == null || !recorder.enabled) yield break;

        recorder.StartRecording();
        started = true;
        startedAt = Time.realtimeSinceStartup;
        Debug.Log("[VK_RecordAndRelocate] Recording started.");
    }

    void OnApplicationPause(bool paused)
    {
        if (paused) StartFinalizeAndMove();
    }

    void OnApplicationQuit()
    {
        StartFinalizeAndMove();
    }

    void StartFinalizeAndMove()
    {
        if (!finalizing) StartCoroutine(FinalizeAndMove());
    }

    IEnumerator FinalizeAndMove()
    {
        if (finalizing) yield break;
        finalizing = true;

        // Let at least a small duration record (helps encoder)
        if (started)
        {
            float elapsed = Time.realtimeSinceStartup - startedAt;
            if (elapsed < minRecordingSeconds)
                yield return new WaitForSeconds(minRecordingSeconds - elapsed);

            if (recorder != null)
            {
                recorder.StopRecording(); // async finalize inside
                Debug.Log("[VK_RecordAndRelocate] Stop requested (finalizing)...");
            }
        }

        // Wait for newest .mp4 to appear in the private folder
        string newest = null;
        float deadline = Time.realtimeSinceStartup + finalizeTimeout;

        while (Time.realtimeSinceStartup < deadline)
        {
            newest = GetNewestMp4(privateDir);
            if (!string.IsNullOrEmpty(newest)) break;
            yield return new WaitForSeconds(0.25f);
        }

        if (string.IsNullOrEmpty(newest))
        {
            Debug.LogWarning("[VK_RecordAndRelocate] No recording found to move (finalize may have failed or plan blocked).");
            yield break;
        }

        // Ensure public folder exists, then copy
        try
        {
            Directory.CreateDirectory(publicDir);
            string dest = Path.Combine(publicDir, Path.GetFileName(newest));
            File.Copy(newest, dest, true);
            Debug.Log($"[VK_RecordAndRelocate] Copied to public: {dest}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[VK_RecordAndRelocate] Move failed: {ex.Message}");
        }
    }

    static string GetNewestMp4(string dir)
    {
        try
        {
            if (!Directory.Exists(dir)) return null;
            var files = Directory.GetFiles(dir, "*.mp4");
            if (files.Length == 0) return null;
            System.Array.Sort(files, (a, b) => File.GetLastWriteTimeUtc(b).CompareTo(File.GetLastWriteTimeUtc(a)));
            return files[0];
        }
        catch { return null; }
    }
}