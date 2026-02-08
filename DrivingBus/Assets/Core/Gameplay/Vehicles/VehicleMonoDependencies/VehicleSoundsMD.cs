using System.Collections.Generic;
using Core.Audio;
using Core.Boot.FlowInterfaces;
using Core.Gameplay.EntityBasedLogic;
using Core.Services;
using UnityEngine;
using UnityEngine.Playables;
using Zenject;

namespace Core.Gameplay.Vehicles.VehicleMonoDependencies
{
    public class VehicleSoundsMD : MonoDependency, IResettable
    {
        [SerializeField] VehicleBase _vehicleBase;
        [SerializeField] AudioSource _audioSourceForStartEngine;
        
        [SerializeField] List<AudioSource> _engineAudioSources = new List<AudioSource>();
        
        [SerializeField] CarParameters _carParameters;
        [SerializeField] PlayableDirector _playableDirector;
        [SerializeField] MultipliersForAudioSource _multipliersForAudioSource;
        [SerializeField] AnimationCurve _acceleration;
        [SerializeField] AnimationCurve _deceleration;
        [SerializeField] float _playableValue;
        [SerializeField] float _curvesMultiplier;
        
        float _duration;

        [Inject] InputService _inputService;

        bool _isAccelerating;
        
        public override void Init()
        {
            _duration = (float) _playableDirector.duration;

            OnChangedEngineIsOn(false);
        }

        public override void Enter()
        {
            _vehicleBase.CarInputs.EngineIsOn.OnValueChanged += OnChangedEngineIsOn;
        }

        public override void Exit()
        {
            _vehicleBase.CarInputs.EngineIsOn.OnValueChanged -= OnChangedEngineIsOn;
        }

        void OnChangedEngineIsOn(bool engineIsOn)
        {
            if (engineIsOn)
            {
                _audioSourceForStartEngine.Play();
                foreach (var engineAudioSource in _engineAudioSources)
                {
                    engineAudioSource.mute = false;
                }
            }
            else
            {
                _audioSourceForStartEngine.Stop();
                foreach (var engineAudioSource in _engineAudioSources)
                {
                    engineAudioSource.mute = true;
                }
            }
        }
        
        void Update()
        {
            if (!_vehicleBase.CarInputs.EngineIsOn.Value)
            {
                _playableValue = 0f;
                _playableDirector.time = 0f;
                return;
            }
            
            var moveRaw = _inputService.Gameplay.MoveRawValue();

            if (_carParameters.IsInAir())
            {
                _playableValue = Mathf.Lerp(_playableValue, 0.3f, Time.deltaTime * 2f);
                _playableDirector.time = _playableValue * _duration;
                _multipliersForAudioSource.CarLeftRightPitchMultiplier = Mathf.Lerp(_multipliersForAudioSource.CarLeftRightPitchMultiplier, 1f, Time.deltaTime * 4f);
                return;
            }

            if (Mathf.Abs(moveRaw.x) > 0)
            {
                _multipliersForAudioSource.CarLeftRightPitchMultiplier = Mathf.Lerp(_multipliersForAudioSource.CarLeftRightPitchMultiplier, 0.85f, Time.deltaTime * 4f);
            }
            else
            {
                _multipliersForAudioSource.CarLeftRightPitchMultiplier = Mathf.Lerp(_multipliersForAudioSource.CarLeftRightPitchMultiplier, 1f, Time.deltaTime * 4f);
            }
			
            if(Mathf.Abs(moveRaw.y) > 0)
            {
                if (!_isAccelerating)
                {
                    _isAccelerating = true;
                }
                // Accelerating
                _playableValue = Mathf.Lerp(_playableValue, _carParameters.NormalizedSpeed(), Time.deltaTime * 6f);
            }
            else
            {
                if (_isAccelerating)
                {
                    _isAccelerating = false;
                }
                // Decelerating
                _playableValue -= _deceleration.Evaluate(_playableValue) * _curvesMultiplier;
                var minValue =  Mathf.Clamp(_carParameters.NormalizedSpeed() - 0.3f, 0f, 1f);
                _playableValue = Mathf.Clamp(_playableValue, minValue, _carParameters.NormalizedSpeed());
            }
			
            _playableDirector.time = _playableValue * _duration;
        }

        public void ResetFull()
        {
            _playableValue = 0f;
            _playableDirector.time = _playableValue;
            Debug.Log("ResetFll");
        }
    }
}