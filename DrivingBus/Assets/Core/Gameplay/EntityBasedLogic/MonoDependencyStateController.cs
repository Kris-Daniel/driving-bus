using System;
using System.Collections.Generic;
using Core.Gameplay.Characters;
using Core.Utils.Observables;
using UnityEngine;

namespace Core.Gameplay.EntityBasedLogic
{
    public interface ITimeInState
    {
        float TimeInState();
    }

    public interface ISetterForCharacterState
    {
        void SetState(ECharacterState newState);
    }
    
    public class MonoDependencyStateController : MonoBehaviour, ITimeInState, ISetterForCharacterState
    {
        ECharacterState _prevState = ECharacterState.None;
        ObservableField<ECharacterState> _currentState = new ObservableField<ECharacterState>(ECharacterState.None);

        float _timeInState = 0f;
        
        Dictionary<ECharacterState, List<MonoDependency>> _monoDependencies = new Dictionary<ECharacterState, List<MonoDependency>>();
        
        void Awake()
        {
            _currentState.OnValueChanged += OnStateChanged;
        }

        void OnStateChanged(ECharacterState newState)
        {
            if (_monoDependencies.ContainsKey(_prevState))
            {
                foreach (var monoDependency in _monoDependencies[_prevState])
                {
                    if (!_monoDependencies[newState].Contains(monoDependency))
                    {
                        monoDependency.Exit();
                    }
                }
            }
            
            _timeInState = 0f;
            
            foreach (var monoDependency in _monoDependencies[newState])
            {
                monoDependency.Enter();
            }

            _prevState = newState;
        }

        public void SetState(ECharacterState newState)
        {
            _currentState.Value = newState;
        }

        public void AddStateMD(ECharacterState state, MonoDependency component)
        {
            if (!_monoDependencies.ContainsKey(state))
            {
                _monoDependencies.Add(state, new List<MonoDependency>());
            }
            
            component.SetupFromStateController(this, this);
            component.Init();
            _monoDependencies[state].Add(component);
        }

        public float TimeInState()
        {
            return _timeInState;
        }
    }
}