using System;
using System.Collections.Generic;
using Core.Services.PoolSystem;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Core.Services.Audio
{
	[Serializable]
	public class ClipData
	{
		public string Name;
		public AudioMixerGroup _audioMixer;
		public List<AudioClip> Clips;
	}

	[Serializable]
	public class CategoryWithAudioClips
	{
		public string Category;
		public Transform ParentForItems;
		public Pool<AudioSourceHandler> AudioPool;
		public List<ClipData> AudioDatas;
	}

	[Serializable]
	public class AudioPlayingDataSimple
	{
		public bool Loop = false;
		public float Volume = 1f;
		public float Pitch = 1f;
	}
	[Serializable]
    public class AudioPlayingDataDynamic
    {
	    public bool Loop = false;
	    public AnimationCurve Volume;
	    public AnimationCurve Pitch;
    }
	
	
	public class AudioService : MonoBehaviour
	{
		[SerializeField] List<CategoryWithAudioClips> _categoriesWithAudio;
		
		[Inject] IContentProviderService _contentProvider;
		[Inject] FactoryInjector _factoryInjector;

		void Awake()
		{
			foreach (var categoryWithAudioClips in _categoriesWithAudio)
			{
				categoryWithAudioClips.AudioPool.InitPool(categoryWithAudioClips.ParentForItems, _contentProvider, _factoryInjector);
			}
		}

		public void StopAll()
		{
			foreach (var categoryWithAudioClips in _categoriesWithAudio)
			{
				categoryWithAudioClips.AudioPool.ReturnAllObjects();
			}
		}

		public void StopAudioCategory(string category)
		{
			foreach (var categoryWithAudioClips in _categoriesWithAudio)
			{
				if (categoryWithAudioClips.Category == category)
				{
					categoryWithAudioClips.AudioPool.ReturnAllObjects();
				}
			}
		}

		public void StopAudioName(string audioName)
		{
			foreach (var categoryWithAudioClips in _categoriesWithAudio)
			{
				var audioSourceHandlers = categoryWithAudioClips.AudioPool.GetSpawnedItems();
				foreach (var audioSourceHandler in audioSourceHandlers)
				{
					if (audioSourceHandler.AudioName == audioName)
					{
						audioSourceHandler.ReturnToPool();
					}
				}
			}
		}

		public AudioSourceHandler Play(string audioName, AudioPlayingDataSimple audioPlayingDataSimple = default, Action onComplete = null)
		{
			return PlaySimpleOrPlayDynamic(true, audioName, audioPlayingDataSimple ?? new AudioPlayingDataSimple(), null, onComplete);	
		}

		public AudioSourceHandler PlayDynamic(string audioName, AudioPlayingDataDynamic audioPlayingDataDynamic = default, Action onComplete = null)
		{
			return PlaySimpleOrPlayDynamic(false, audioName, null, audioPlayingDataDynamic ?? new AudioPlayingDataDynamic(), onComplete);
		}

		AudioSourceHandler PlaySimpleOrPlayDynamic(bool isSimplePlay, string audioName, AudioPlayingDataSimple audioPlayingDataSimple = default, AudioPlayingDataDynamic audioPlayingDataDynamic = default, Action onComplete = null)
		{
			foreach (var categoryWithAudioClips in _categoriesWithAudio)
			{
				foreach (var audioData in categoryWithAudioClips.AudioDatas)
				{
					if (audioData.Name == audioName && audioData.Clips.Count > 0)
					{
						var audioSourceHandler = categoryWithAudioClips.AudioPool.SpawnItem();
						var clip = audioData.Clips[UnityEngine.Random.Range(0, audioData.Clips.Count)];
						if (isSimplePlay)
						{
							audioSourceHandler.PlaySimple(audioName, clip, audioData._audioMixer, audioPlayingDataSimple, onComplete);
						}
						else
						{
							audioSourceHandler.PlayDynamic(audioName, clip, audioData._audioMixer, audioPlayingDataDynamic, onComplete);
						}
						return audioSourceHandler;
					}
				}
			}

			return default;
		}
	}
}