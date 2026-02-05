using DG.Tweening;
using UnityEngine;

namespace Core.UI.BaseViews
{
    public class AnimatedView : View
    {
        [SerializeField] protected CanvasGroup _container;
        
        public override void Hide() => DOFadeAndInteract(0, 0);
        public override void Show() => DOFadeAndInteract(1, 0);

        public Tween ShowAnimated(float duration = 0.3f)
        {
            return DOFadeAndInteract(1f, duration);
        }

        public Tween HideAnimated(float duration = 0.3f)
        {
            return DOFadeAndInteract(0f, duration);
        }

        Tween DOFadeAndInteract(float alpha, float duration)
        {
            _container.DOKill();
			
            bool interactable = alpha > 0.001f;
			
            _container.interactable = interactable;
            _container.blocksRaycasts = interactable;

            if (duration < 0.001f)
            {
                _container.alpha = alpha;
            }

            return _container.DOFade(alpha, duration).SetUpdate(true).OnComplete(() => _container.alpha = alpha);
        }

    }
}