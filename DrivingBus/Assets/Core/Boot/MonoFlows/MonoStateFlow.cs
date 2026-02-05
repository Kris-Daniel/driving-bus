using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Boot.MonoFlows
{

    public abstract class MonoStateFlow : MonoBehaviour
    {
        public abstract UniTask Enter();
        public abstract UniTask Exit();
    }
}