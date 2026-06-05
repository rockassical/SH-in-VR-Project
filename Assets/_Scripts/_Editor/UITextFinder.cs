#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UITextFinder : EditorWindow
{
    private string searchText = "";
    private string replaceText = "";
    private bool caseSensitive = false;
    private bool includeInactive = true;

    private Vector2 scroll;
    private readonly List<SearchResult> results = new();

    private GUIStyle wrappedTextAreaStyle;
    private GUIStyle wrappedLabelStyle;

    private readonly Dictionary<int, List<GameObject>> previewActiveChains = new();

    private int activePreviewKey = 0;
    private GameObject activePreviewTarget = null;
    private bool suppressSelectionChanged = false;

    private class SearchResult
    {
        public GameObject gameObject;
        public Component component;
        public string textAtSearchTime;
        public string editableText;
        public string componentType;
    }

    [MenuItem("Tools/Custom/Search UI Text")]
    public static void ShowWindow()
    {
        GetWindow<UITextFinder>("Search UI Text");
    }

    private void OnEnable()
    {
        Selection.selectionChanged += OnEditorSelectionChanged;
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= OnEditorSelectionChanged;
        RestoreActivePreviewIfNeeded();
    }

    private void OnGUI()
    {
        EnsureStyles();

        EditorGUILayout.LabelField("Search Text in Scene UI", EditorStyles.boldLabel);

        searchText = EditorGUILayout.TextField("Find", searchText);
        replaceText = EditorGUILayout.TextField("Replace With", replaceText);

        caseSensitive = EditorGUILayout.Toggle("Case Sensitive", caseSensitive);
        includeInactive = EditorGUILayout.Toggle("Include Inactive", includeInactive);

        EditorGUILayout.Space();

        using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
        {
            if (GUILayout.Button("Search", GUILayout.MinWidth(70), GUILayout.ExpandWidth(true)))
            {
                SearchSceneText();
            }

            GUI.enabled = results.Count > 0 && !string.IsNullOrEmpty(replaceText);

            if (GUILayout.Button("Replace All", GUILayout.MinWidth(90), GUILayout.ExpandWidth(true)))
            {
                if (EditorUtility.DisplayDialog(
                        "Replace All?",
                        $"Replace all matches of \"{searchText}\" with \"{replaceText}\"?",
                        "Replace All",
                        "Cancel"))
                {
                    ReplaceAll();
                    SearchSceneText();
                }
            }

            GUI.enabled = true;
        }

        EditorGUILayout.HelpBox(
            "Each result shows the actual text from the UI object. Edit it and press Replace This for a custom replacement. If you do not edit it, Replace This uses the top-level Find / Replace With fields. The temporary scene preview turns on only the inactive objects needed to reveal the text, then restores them when you end preview or select another object.",
            MessageType.Info
        );

        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Results: {results.Count}", EditorStyles.boldLabel);

        scroll = EditorGUILayout.BeginScrollView(
            scroll,
            GUILayout.ExpandWidth(true),
            GUILayout.ExpandHeight(true)
        );

        foreach (SearchResult result in results)
        {
            DrawResult(result);
        }

        EditorGUILayout.EndScrollView();
    }

    private void EnsureStyles()
    {
        if (wrappedTextAreaStyle == null)
        {
            wrappedTextAreaStyle = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true,
                stretchWidth = true
            };
        }

        if (wrappedLabelStyle == null)
        {
            wrappedLabelStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                stretchWidth = true
            };
        }
    }

    private void OnEditorSelectionChanged()
    {
        if (suppressSelectionChanged)
            return;

        if (activePreviewTarget == null)
            return;

        if (Selection.activeGameObject != activePreviewTarget)
        {
            RestoreActivePreviewIfNeeded();
            Repaint();
        }
    }

    private void SearchSceneText()
    {
        RestoreActivePreviewIfNeeded();

        results.Clear();

        if (string.IsNullOrEmpty(searchText))
            return;

        SearchTMPTextObjects();
        SearchLegacyUITextObjects();

        Repaint();
    }

    private void SearchTMPTextObjects()
    {
        FindObjectsInactive inactiveMode =
            includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude;

        TMP_Text[] textObjects = UnityEngine.Object.FindObjectsByType<TMP_Text>(
            inactiveMode,
            FindObjectsSortMode.None
        );

        foreach (TMP_Text textObject in textObjects)
        {
            if (!IsSceneObject(textObject.gameObject))
                continue;

            if (Contains(textObject.text, searchText))
            {
                AddResult(
                    textObject.gameObject,
                    textObject,
                    textObject.text,
                    textObject.GetType().Name
                );
            }
        }
    }

    private void SearchLegacyUITextObjects()
    {
        FindObjectsInactive inactiveMode =
            includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude;

        Text[] textObjects = UnityEngine.Object.FindObjectsByType<Text>(
            inactiveMode,
            FindObjectsSortMode.None
        );

        foreach (Text textObject in textObjects)
        {
            if (!IsSceneObject(textObject.gameObject))
                continue;

            if (Contains(textObject.text, searchText))
            {
                AddResult(
                    textObject.gameObject,
                    textObject,
                    textObject.text,
                    "UnityEngine.UI.Text"
                );
            }
        }
    }

    private void AddResult(GameObject gameObject, Component component, string currentText, string componentType)
    {
        results.Add(new SearchResult
        {
            gameObject = gameObject,
            component = component,
            textAtSearchTime = currentText,
            editableText = currentText,
            componentType = componentType
        });
    }

    private void DrawResult(SearchResult result)
    {
        if (result.gameObject == null || result.component == null)
            return;

        EnsureStyles();

        EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));

        EditorGUILayout.LabelField(result.componentType, EditorStyles.boldLabel);

        EditorGUILayout.LabelField(
            "Object",
            GetHierarchyPath(result.gameObject),
            wrappedLabelStyle,
            GUILayout.ExpandWidth(true)
        );

        bool wasManuallyEdited = result.editableText != result.textAtSearchTime;
        bool canReplaceThis = wasManuallyEdited || !string.IsNullOrEmpty(replaceText);

        EditorGUILayout.LabelField(wasManuallyEdited ? "Edited Text" : "Current Text");

        result.editableText = EditorGUILayout.TextArea(
            result.editableText,
            wrappedTextAreaStyle,
            GUILayout.MinHeight(80),
            GUILayout.ExpandHeight(false),
            GUILayout.ExpandWidth(true)
        );

        using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
        {
            if (GUILayout.Button("Select", GUILayout.MinWidth(70), GUILayout.ExpandWidth(true)))
            {
                RestoreActivePreviewIfNeeded();

                suppressSelectionChanged = true;
                Selection.activeGameObject = result.gameObject;
                EditorGUIUtility.PingObject(result.gameObject);
                suppressSelectionChanged = false;
            }

            GUI.enabled = canReplaceThis;

            if (GUILayout.Button("Replace This", GUILayout.MinWidth(110), GUILayout.ExpandWidth(true)))
            {
                string newText = wasManuallyEdited
                    ? result.editableText
                    : ReplaceString(result.textAtSearchTime);

                SetTextDirectly(result.component, newText);
                SearchSceneText();
            }

            GUI.enabled = true;
        }

        DrawTemporaryPreviewButton(result);

        if (!canReplaceThis)
        {
            EditorGUILayout.HelpBox(
                "Replace This is disabled because this text was not manually edited and Replace With is blank.",
                MessageType.None
            );
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawTemporaryPreviewButton(SearchResult result)
    {
        int key = result.component.GetInstanceID();
        bool thisPreviewIsActive = activePreviewKey == key && previewActiveChains.ContainsKey(key);

        string previewButtonText = thisPreviewIsActive
            ? "End Temporary Scene Preview"
            : "Temporarily Show and Frame UI";

        if (GUILayout.Button(previewButtonText, GUILayout.ExpandWidth(true)))
        {
            ToggleTemporaryScenePreview(result);
        }

        if (thisPreviewIsActive)
        {
            int changedCount = previewActiveChains.TryGetValue(key, out List<GameObject> changedObjects)
                ? changedObjects.Count
                : 0;

            EditorGUILayout.HelpBox(
                $"Temporary preview is active. The tool turned on {changedCount} inactive object(s). Selecting another object will automatically restore them.",
                MessageType.Info
            );
        }
    }

    private void ToggleTemporaryScenePreview(SearchResult result)
    {
        if (result == null || result.gameObject == null || result.component == null)
            return;

        int key = result.component.GetInstanceID();

        if (activePreviewKey == key && previewActiveChains.ContainsKey(key))
        {
            RestoreActivePreviewIfNeeded();
            return;
        }

        RestoreActivePreviewIfNeeded();

        ShowTemporaryScenePreview(result, key);
    }

    private void ShowTemporaryScenePreview(SearchResult result, int key)
    {
        List<GameObject> objectsTurnedOn = new();

        Transform current = result.gameObject.transform;

        while (current != null)
        {
            GameObject currentObject = current.gameObject;

            if (!currentObject.activeSelf)
            {
                Undo.RecordObject(currentObject, "Temporary UI Preview Active State");
                currentObject.SetActive(true);
                EditorUtility.SetDirty(currentObject);
                objectsTurnedOn.Add(currentObject);
            }

            current = current.parent;
        }

        previewActiveChains[key] = objectsTurnedOn;
        activePreviewKey = key;
        activePreviewTarget = result.gameObject;

        suppressSelectionChanged = true;
        Selection.activeGameObject = result.gameObject;
        EditorGUIUtility.PingObject(result.gameObject);
        suppressSelectionChanged = false;

        MarkSceneDirty(result.gameObject.scene);

        FocusObjectInSceneViewFaceOn(result.gameObject);

        Repaint();
    }

    private void RestoreActivePreviewIfNeeded()
    {
        if (activePreviewKey == 0)
            return;

        if (previewActiveChains.TryGetValue(activePreviewKey, out List<GameObject> objectsToRestore))
        {
            foreach (GameObject obj in objectsToRestore)
            {
                if (obj == null)
                    continue;

                Undo.RecordObject(obj, "Restore Temporary UI Preview Active State");
                obj.SetActive(false);
                EditorUtility.SetDirty(obj);

                if (obj.scene.IsValid() && obj.scene.isLoaded)
                {
                    MarkSceneDirty(obj.scene);
                }
            }

            previewActiveChains.Remove(activePreviewKey);
        }

        activePreviewKey = 0;
        activePreviewTarget = null;
    }

    private void ReplaceAll()
    {
        foreach (SearchResult result in results)
        {
            if (result.component == null)
                continue;

            string currentText = GetText(result.component);
            string replacedText = ReplaceString(currentText);

            SetTextDirectly(result.component, replacedText);
        }
    }

    private string GetText(Component component)
    {
        if (component is TMP_Text tmpText)
            return tmpText.text;

        if (component is Text legacyText)
            return legacyText.text;

        return "";
    }

    private void SetTextDirectly(Component component, string newText)
    {
        Undo.RecordObject(component, "Replace UI Text");

        if (component is TMP_Text tmpText)
        {
            tmpText.text = newText;
            EditorUtility.SetDirty(tmpText);
            MarkSceneDirty(tmpText.gameObject.scene);
        }
        else if (component is Text legacyText)
        {
            legacyText.text = newText;
            EditorUtility.SetDirty(legacyText);
            MarkSceneDirty(legacyText.gameObject.scene);
        }
    }

    private string ReplaceString(string source)
    {
        if (string.IsNullOrEmpty(source))
            return source;

        if (string.IsNullOrEmpty(searchText))
            return source;

        StringComparison comparison =
            caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        int index = source.IndexOf(searchText, comparison);
        if (index < 0)
            return source;

        string result = source;

        while (index >= 0)
        {
            result = result.Remove(index, searchText.Length)
                           .Insert(index, replaceText);

            index = result.IndexOf(
                searchText,
                index + replaceText.Length,
                comparison
            );
        }

        return result;
    }

    private bool Contains(string source, string target)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            return false;

        StringComparison comparison =
            caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        return source.IndexOf(target, comparison) >= 0;
    }

    private bool IsSceneObject(GameObject obj)
    {
        if (obj == null)
            return false;

        Scene scene = obj.scene;

        return scene.IsValid() && scene.isLoaded;
    }

    private void MarkSceneDirty(Scene scene)
    {
        if (scene.IsValid() && scene.isLoaded)
        {
            EditorSceneManager.MarkSceneDirty(scene);
        }
    }

    private void FocusObjectInSceneViewFaceOn(GameObject obj)
    {
        if (obj == null)
            return;

        SceneView sceneView = SceneView.lastActiveSceneView;

        if (sceneView == null && SceneView.sceneViews.Count > 0)
        {
            sceneView = SceneView.sceneViews[0] as SceneView;
        }

        if (sceneView == null)
            return;

        Bounds bounds = GetObjectBounds(obj);

        Vector3 center = bounds.center;
        float size = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

        if (size <= 0.01f)
            size = 1f;

        sceneView.orthographic = true;

        Vector3 forward = obj.transform.forward;
        Vector3 up = obj.transform.up;

        if (forward.sqrMagnitude < 0.001f)
            forward = Vector3.forward;

        if (up.sqrMagnitude < 0.001f)
            up = Vector3.up;

        Quaternion faceOnRotation = Quaternion.LookRotation(forward, up);

        sceneView.LookAt(
            center,
            faceOnRotation,
            size * 1.5f,
            true,
            true
        );

        sceneView.Repaint();
    }

    private Bounds GetObjectBounds(GameObject obj)
    {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            Bounds bounds = new Bounds(corners[0], Vector3.zero);

            for (int i = 1; i < corners.Length; i++)
            {
                bounds.Encapsulate(corners[i]);
            }

            if (bounds.size == Vector3.zero)
            {
                bounds.size = Vector3.one;
            }

            return bounds;
        }

        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer != null)
        {
            return renderer.bounds;
        }

        return new Bounds(obj.transform.position, Vector3.one);
    }

    private string GetHierarchyPath(GameObject obj)
    {
        if (obj == null)
            return "";

        string path = obj.name;
        Transform current = obj.transform.parent;

        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }

        return path;
    }
}
#endif