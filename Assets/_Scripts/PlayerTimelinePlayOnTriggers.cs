using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using SWS;

[RequireComponent(typeof(PlayableDirector))]
public class PlayerTimelinePlayOnTriggers : MonoBehaviour
{
    [Serializable]
    public struct TagRule
    {
        [Tooltip("Tag on the trigger collider in the scene.")]
        public string tag;

        [Tooltip("Optional: switch to this TimelineAsset before playing.")]
        public TimelineAsset timeline;
    }

    [Header("Rules (one per trigger tag)")]
    public List<TagRule> rules = new List<TagRule>();

    [Header("Replay")]
    [Tooltip("If true, each individual trigger collider fires only once.")]
    public bool playOnlyOncePerTrigger = true;

    private PlayableDirector _director;
    private readonly HashSet<Collider> _fired = new HashSet<Collider>();
    private splineMove _spline;

    private void Awake()
    {
        _director = GetComponent<PlayableDirector>();
        if (!_director)
            Debug.LogError("[PlayerTimelinePlayOnTriggers] Missing PlayableDirector on the player.");

        _spline = GetComponentInParent<splineMove>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        // Match rule by tag
        int idx = rules.FindIndex(r => !string.IsNullOrEmpty(r.tag) && other.CompareTag(r.tag));
        if (idx < 0) return; // no rule for this tag

        if (playOnlyOncePerTrigger && _fired.Contains(other)) return;

        var rule = rules[idx];

        // Optionally swap TimelineAsset (no reset of time; just play from current director time)
        if (rule.timeline != null && _director.playableAsset != rule.timeline)
        {
            _director.playableAsset = rule.timeline;
            // Note: we intentionally do NOT reset time. Timeline signals will handle pausing.
        }

        _director.Play();        // Just play; your Signal Track should pause it at the right moment
        _fired.Add(other);

        if(_spline != null)
        _spline.Pause();
    }
}
