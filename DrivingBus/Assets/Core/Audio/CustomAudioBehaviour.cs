using UnityEngine;
using UnityEngine.Playables;

namespace Core.Audio
{
	public class CustomAudioBehaviour : PlayableBehaviour
{
    public AudioClip audioClip;
    public AnimationCurve volumeCurve = AnimationCurve.Linear(0, 1, 1, 1);
    public AnimationCurve pitchCurve = AnimationCurve.Linear(0, 1, 1, 1); // New!
    public bool syncStartWithTimeline = false;

    private bool hasPlayed = false;
    private AudioSource audioSource;
    MultipliersForAudioSource _multipliersForAudioSource;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (audioSource == null && playerData is AudioSource source)
        {
            audioSource = source;
            _multipliersForAudioSource = audioSource.gameObject.GetComponent<MultipliersForAudioSource>();
        }

        double time = playable.GetTime();
        double duration = playable.GetDuration();

        // Remap timeline progress to [0..1]
        float t = duration > 0 ? (float)(time / duration) : 0f;

        // Evaluate volume and pitch
        float curveVolume = volumeCurve.Evaluate(t);
        float curvePitch = pitchCurve.Evaluate(t);

        float pitchMultiplier = 1f;
        if (_multipliersForAudioSource)
        {
            pitchMultiplier = _multipliersForAudioSource.CarLeftRightPitchMultiplier;
        }

        if (audioSource != null)
        {
            audioSource.volume = curveVolume;
            audioSource.pitch = curvePitch * pitchMultiplier;
        }

        // Play on enter
        if (!hasPlayed && audioSource != null && audioClip != null)
        {
            audioSource.clip = audioClip;

            if (syncStartWithTimeline)
            {
                double clipLocalTime = playable.GetTime();
                double clipDuration = playable.GetDuration();
                float normTime = clipDuration > 0f ? (float)(clipLocalTime / clipDuration) : 0f;
                float audioTime = audioClip.length * Mathf.Clamp01(normTime);
                audioSource.time = audioTime;
            }
            else
            {
                audioSource.time = 0f;
            }

            audioSource.Play();
            hasPlayed = true;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (audioSource != null)
            audioSource.Stop();
        hasPlayed = false;
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        hasPlayed = false;
    }
}
}