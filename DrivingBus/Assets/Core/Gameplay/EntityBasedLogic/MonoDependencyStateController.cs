using System.Collections.Generic;
using Core.Utils.Observables;
using UnityEngine;

namespace Core.Gameplay.EntityBasedLogic
{
    public class MonoDependencyController : MonoBehaviour
    {
        ObservableField<string> _monoState = new ObservableField<string>("");
        
        Dictionary<string, List<MonoDependency>> _monoDependencies = new Dictionary<string, List<MonoDependency>>();
        
        List<MonoDependency> _commonDependencies = new List<MonoDependency>();
        
        public void AddCommonMD(MonoDependency component)
        {
            _commonDependencies.Add(component);
        }

        public void AddStateMD(string state, MonoDependency component)
        {
            if (!_monoDependencies.ContainsKey(state))
            {
                _monoDependencies.Add(state, new List<MonoDependency>());
            }
            
            _monoDependencies[state].Add(component);
        }
    }
}