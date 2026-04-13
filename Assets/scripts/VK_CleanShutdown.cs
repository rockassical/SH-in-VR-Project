// VK_CleanShutdown.cs
// - Counts down in real time
// - Logs milestones (30/15/5s by default)
// - Stops VideoKitRecorder once the timer elapses (nothing else)

using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System.IO;
using VideoKit;

[DisallowMultipleComponent]
public class VK_CleanShutdown : MonoBehaviour
{
    [Header("Timing")]
    [Tooltip("Seconds from start until recording is stopped.")]
    [SerializeField] float stopAfterSeconds = 35f;

    [Header("Milestone logs (seconds remaining)")]
    [SerializeField] int[] milestoneSeconds = new[] { 30, 15, 5 };

    [Header("R2 Uploader")]
    [SerializeField] VK_R2Uploader uploader; // optional; if assigned, will upload newest MP4 after stop

    VideoKitRecorder recorder;
    bool stopping;
    string _foundPath;

    void Awake()
    {
        recorder = GetComponent<VideoKitRecorder>();
        if (recorder == null) recorder = FindObjectOfType<VideoKitRecorder>();
        StartCoroutine(CountdownAndStop());
    }

    IEnumerator CountdownAndStop()
    {
        if (stopAfterSeconds <= 0f)
        {
            yield return null;
            TriggerStop();
            yield break;
        }

        float t = stopAfterSeconds;
        var milestones = new System.Collections.Generic.HashSet<int>(milestoneSeconds ?? new int[0]);

        while (t > 0f)
        {
            int remaining = Mathf.CeilToInt(t);
            if (milestones.Contains(remaining))
                Debug.Log($"[VK_CleanShutdown] {remaining} seconds remaining…");

            yield return new WaitForSecondsRealtime(1f);
            t -= 1f;
        }

        TriggerStop();
    }

    void TriggerStop()
    {
        if (stopping) return;
        stopping = true;
        Debug.Log("[VK_CleanShutdown] Time elapsed — requesting recorder stop…");
        StartCoroutine(StopRecorderCoroutine());
    }

    IEnumerator StopRecorderCoroutine()
    {
        if (recorder == null)
        {
            Debug.LogWarning("[VK_CleanShutdown] No VideoKitRecorder found.");
            yield break;
        }

        yield return new WaitForEndOfFrame();

        if (recorder.status != VideoKitRecorder.Status.Idle)
        {
            Task stopTask = Task.CompletedTask;
            try
            {
                stopTask = recorder.StopRecordingAsync();
                Debug.Log("[VK_CleanShutdown] Stop requested (finalizing)...");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[VK_CleanShutdown] StopRecordingAsync threw: {ex.Message}");
                yield break;
            }

            while (!stopTask.IsCompleted) yield return null;

            if (stopTask.IsFaulted)
            {
                Debug.LogWarning($"[VK_CleanShutdown] Finalize faulted: {stopTask.Exception?.GetBaseException().Message}");
                yield break;
            }

            Debug.Log("[VK_CleanShutdown] Recording finalized.");
        }
        else
        {
            Debug.Log("[VK_CleanShutdown] Recorder already idle (not recording).");
        }

        if (uploader != null)
        {
            yield return new WaitForSecondsRealtime(0.25f);
            yield return StartCoroutine(FindNewestRecordingPathCoroutine(3f));
            string newest = _foundPath;
            if (!string.IsNullOrEmpty(newest))
            {
                Debug.Log($"[VK_CleanShutdown] Uploading newest recording: {newest}");
                uploader.StartCoroutine(uploader.UploadVideo(newest));
            }
            else
            {
                Debug.LogWarning("[VK_CleanShutdown] No recording found in any default folders.");
            }
        }
    }

    static string GetNewestMp4(string dir)
    {
        if (!Directory.Exists(dir)) return null;
        var files = Directory.GetFiles(dir, "*.mp4");
        if (files.Length == 0) return null;
        System.Array.Sort(files, (a, b) => File.GetLastWriteTimeUtc(b).CompareTo(File.GetLastWriteTimeUtc(a)));
        return files[0];
    }

    IEnumerator FindNewestRecordingPathCoroutine(float maxWaitSeconds)
    {
        float end = Time.realtimeSinceStartup + Mathf.Max(0.5f, maxWaitSeconds);
        string candidate = TryFindNewestRecordingOnce();
        while (string.IsNullOrEmpty(candidate) && Time.realtimeSinceStartup < end)
        {
            yield return new WaitForSecondsRealtime(0.25f);
            candidate = TryFindNewestRecordingOnce();
        }
        _foundPath = candidate;
    }

    string TryFindNewestRecordingOnce()
    {
        var dirs = GetDefaultRecordingDirs();
        string[] patterns = { "*.mp4", "*.webm" };
        string newest = null;
        System.DateTime newestTime = System.DateTime.MinValue;

        foreach (var dir in dirs)
        {
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir)) continue;

            foreach (var pattern in patterns)
            {
                var files = Directory.GetFiles(dir, pattern);
                foreach (var f in files)
                {
                    var t = File.GetLastWriteTimeUtc(f);
                    if (t > newestTime) { newestTime = t; newest = f; }
                }
            }
        }

        if (!string.IsNullOrEmpty(newest))
            Debug.Log($"[VK_CleanShutdown] Found newest recording: {newest}");

        return newest;
    }

    System.Collections.Generic.List<string> GetDefaultRecordingDirs()
    {
        var list = new System.Collections.Generic.List<string>();

#if UNITY_EDITOR
        var projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
        if (!string.IsNullOrEmpty(projectRoot))
        {
            list.Add(Path.Combine(projectRoot, "recordings"));
            list.Add(Path.Combine(projectRoot, "VideoKit", "recordings"));
            list.Add(Path.Combine(projectRoot, "Captures"));
        }
#endif

        list.Add(Path.Combine(Application.persistentDataPath, "recordings"));
        list.Add(Path.Combine(Application.persistentDataPath, "VideoKit", "recordings"));

        Debug.Log("[VK_CleanShutdown] Search dirs: " + string.Join(" | ", list));
        return list;
    }
}