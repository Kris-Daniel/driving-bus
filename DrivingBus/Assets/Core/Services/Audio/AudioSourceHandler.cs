using System;
using System.Collections;
using Core.Services.PoolSystem;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Services.Audio
{
	public class AudioSourceHandler : MonoBehaviour, IPoolItem
	{
		[SerializeField] AudioSource _audioSource;
		public AudioSource AudioSource => _audioSource;
		
		IPool _pool;

		string _audioName = "";
		public string AudioName => _audioName;
		
		public void OnSetPool(IPool pool)
		{
			_pool = pool;
		}

		public void ReturnToPool()
		{
			_pool.ReturnObject(this);
		}

		public void AfterReturnToPool()
		{
			
		}

		public float ClipLength()
		{
			if (_audioSource.clip == null)
			{
				return 0f;
			}
			return _audioSource.clip.length;
		}

		public void PlaySimple(string audioName, AudioClip clip, AudioMixerGroup audioDataAudioMixer, AudioPlayingDataSimple audioPlayingDataSimple, Action onComplete)
		{
			_audioName = audioName;
			_audioSource.clip = clip;
			_audioSource.loop = audioPlayingDataSimple.Loop;
			_audioSource.volume = audioPlayingDataSimple.Volume;
			_audioSource.outputAudioMixerGroup = audioDataAudioMixer;
			_audioSource.Play();
			StartCoroutine(ReturnToPoolAfterClip2D(clip.length, onComplete, audioPlayingDataSimple));
		}

		public void PlayDynamic(string audioName, AudioClip clip, AudioMixerGroup audioDataAudioMixer, AudioPlayingDataDynamic audioPlayingData3D, Action onComplete)
		{
			_audioName = audioName;
			_audioSource.clip = clip;
			_audioSource.loop = audioPlayingData3D.Loop;
			_audioSource.outputAudioMixerGroup = audioDataAudioMixer;
			_audioSource.Play();
			StartCoroutine(ReturnToPoolAfterDynamicClip(clip.length, audioPlayingData3D, onComplete));
		}

		void OnDisable()
		{
			_audioSource.clip = null;
			_audioSource.loop = false;
			_audioSource.Stop();
			_audioName = "";
		}

		IEnumerator ReturnToPoolAfterClip2D(float clipLength, Action onComplete, AudioPlayingDataSimple audioPlayingDataSimple)
		{
			yield return new WaitForSeconds(clipLength);

			if (!audioPlayingDataSimple.Loop)
			{
				onComplete?.Invoke();
				ReturnToPool();
			}
		}
		
		IEnumerator ReturnToPoolAfterDynamicClip(float clipLength, AudioPlayingDataDynamic audioPlayingData3D, Action onComplete)
		{
			var delta = 0f;

			if (!audioPlayingData3D.Loop)
			{
				while (_audioSource.isPlaying)
				{
					delta = _audioSource.time / _audioSource.clip.length;
					_audioSource.volume = audioPlayingData3D.Volume.Evaluate(delta);
					_audioSource.pitch = audioPlayingData3D.Pitch.Evaluate(delta);
					yield return null;
				}
			}
			
			while (audioPlayingData3D.Loop)
			{
				delta = _audioSource.time / _audioSource.clip.length;
				_audioSource.volume = audioPlayingData3D.Volume.Evaluate(delta);
				_audioSource.pitch = audioPlayingData3D.Pitch.Evaluate(delta);
				
				yield return null;
			}
			
			onComplete?.Invoke();
			ReturnToPool();
		}
	}
}