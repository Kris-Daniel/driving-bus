using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Core.Audio
{
	[System.Serializable]
    public class CustomAudioClipAsset : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] public AudioClip audioClip;
        [SerializeField] public AnimationCurve volumeCurve = AnimationCurve.Linear(0, 1, 1, 1);
        [SerializeField] public AnimationCurve pitchCurve = AnimationCurve.Linear(0, 1, 1, 1); // New field!
    
        [SerializeField] public bool syncStartWithTimeline = false;
    
        public ClipCaps clipCaps => ClipCaps.None;
    
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<CustomAudioBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.audioClip = audioClip;
            behaviour.volumeCurve = volumeCurve;
            behaviour.pitchCurve = pitchCurve; // Pass the curve
            behaviour.syncStartWithTimeline = syncStartWithTimeline;
            return playable;
        }
    }

}