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
        void SetState(string newState);
    }
    
    public class MonoDependencyStateController : MonoBehaviour, ITimeInState, ISetterForCharacterState
    {
        string _prevState = "";
        ObservableField<string> _currentState = new ObservableField<string>("");

        float _timeInState = 0f;
        
        Dictionary<string, List<MonoDependency>> _monoDependencies = new Dictionary<string, List<MonoDependency>>();
        
        void Awake()
        {
            _currentState.OnValueChanged += OnStateChanged;
        }

        void OnStateChanged(string newState)
        {
            if (_monoDependencies.ContainsKey(_prevState))
            {
                foreach (var prevMonoDependency in _monoDependencies[_prevState])
                {
                    if (!_monoDependencies[newState].Contains(prevMonoDependency))
                    {
                        prevMonoDependency.Exit();
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

        public void SetState(string newState)
        {
            if (_currentState.Value != newState)
            {
                _currentState.Value = newState;
            }
        }

        public void AddStateMD(string state, MonoDependency component)
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