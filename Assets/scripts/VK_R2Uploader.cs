// VK_R2Uploader.cs
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

[DisallowMultipleComponent]
public class VK_R2Uploader : MonoBehaviour
{
    [Header("Backend API")]
    [SerializeField] string backendUrl = "";

    [Header("Device Tag")]
    [SerializeField] string deviceTag = "QuestPro";

    public IEnumerator UploadVideo(string localFilePath)
    {
        if (string.IsNullOrEmpty(backendUrl))
        {
            Debug.LogError("[VK_R2Uploader] backendUrl is empty.");
            yield break;
        }
        if (!File.Exists(localFilePath))
        {
            Debug.LogError($"[VK_R2Uploader] File not found: {localFilePath}");
            yield break;
        }

        string originalName = Path.GetFileName(localFilePath);
        string ts = System.DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        string finalName = $"{ts}_{deviceTag}_{originalName}";

        string putReqJson = JsonUtility.ToJson(new PresignPutReq { filename = finalName, content_type = "video/mp4" });
        var baseUrl = backendUrl?.TrimEnd('/');
        using (UnityWebRequest req = new UnityWebRequest($"{baseUrl}/presign-put", "POST"))
        {
            byte[] body = System.Text.Encoding.UTF8.GetBytes(putReqJson);
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[VK_R2Uploader] Presign failed: {req.responseCode} {req.error}");
                yield break;
            }

            var presign = JsonUtility.FromJson<PresignPutResp>(req.downloadHandler.text);

#if UNITY_2022_1_OR_NEWER
            using (UnityWebRequest putReq = new UnityWebRequest(presign.put_url, UnityWebRequest.kHttpVerbPUT))
            {
                putReq.uploadHandler = new UploadHandlerFile(localFilePath);
                putReq.downloadHandler = new DownloadHandlerBuffer();
                putReq.SetRequestHeader("Content-Type", "video/mp4");
                putReq.timeout = 300;

                var op = putReq.SendWebRequest();
                while (!op.isDone) { yield return null; }

                if (putReq.result != UnityWebRequest.Result.Success || (putReq.responseCode != 200 && putReq.responseCode != 201))
                {
                    Debug.LogError($"[VK_R2Uploader] Upload failed: {putReq.responseCode} {putReq.error}");
                    yield break;
                }
            }
#else
            byte[] bytes = File.ReadAllBytes(localFilePath);
            using (UnityWebRequest putReq = UnityWebRequest.Put(presign.put_url, bytes))
            {
                putReq.method = UnityWebRequest.kHttpVerbPUT;
                putReq.SetRequestHeader("Content-Type", "video/mp4");
                putReq.timeout = 300;
                yield return putReq.SendWebRequest();

                if (putReq.result != UnityWebRequest.Result.Success || (putReq.responseCode != 200 && putReq.responseCode != 201))
                {
                    Debug.LogError($"[VK_R2Uploader] Upload failed: {putReq.responseCode} {putReq.error}");
                    yield break;
                }
            }
#endif
            Debug.Log($"[VK_R2Uploader] Uploaded OK → {presign.key}");
        }
    }

    [System.Serializable]
    public class PresignPutReq
    {
        public string filename;
        public string content_type;
    }

    [System.Serializable]
    public class PresignPutResp
    {
        public string key;
        public string put_url;
        public string get_url;
    }
}