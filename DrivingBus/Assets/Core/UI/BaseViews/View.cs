using UnityEngine;

namespace Core.UI.BaseViews
{
    public abstract class View : MonoBehaviour
    {
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
		
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}