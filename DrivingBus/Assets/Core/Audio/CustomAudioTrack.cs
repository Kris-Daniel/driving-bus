using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Core.Audio
{
	[TrackColor(0.8f, 0.6f, 0.4f)]
	[TrackBindingType(typeof(AudioSource))]
	[TrackClipType(typeof(CustomAudioClipAsset))]
	public class CustomAudioTrack : TrackAsset
	{
		// public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
		// {
		// 	return ScriptPlayable<CustomAudioMixerBehaviour>.Create(graph, inputCount);
		// }
	}
}