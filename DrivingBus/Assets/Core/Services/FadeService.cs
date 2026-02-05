using System;
using Core.UI.BaseViews;
using DG.Tweening;
using UnityEngine;

namespace Core.Services
{
    public interface IFadeService
    {
        void FadeIn();
        void FadeOut();
        Tween FadeInTween();
        Tween FadeOutTween();
        void FadeInAndOutAnimated(Action callbackBetweenFades);
    }

    public class FadeService : MonoBehaviour, IFadeService
    {
        [SerializeField] AnimatedView _fadeView;
        [SerializeField] GameObject _maskOverAll;

        public void FadeIn()
        {
            _fadeView.Show();
        }
 
        public void FadeOut()
        {
            _fadeView.Hide();
        }

        public void FadeInAndOutAnimated(Action callbackBetweenFades)
        {
            FadeInTween().OnComplete(() =>
            {
                callbackBetweenFades?.Invoke();
                FadeOutTween().SetDelay(0.15f);
            });
        }

        public Tween FadeInTween()
        {
            return _fadeView.ShowAnimated(0.3f).SetEase(Ease.InOutSine);
        }

        public Tween FadeOutTween()
        {
            return _fadeView.HideAnimated(0.3f).SetEase(Ease.InOutSine);
        }

        public void ShowMaskOverAll()
        {
            _maskOverAll.SetActive(true);
        }
        
        public void HideMaskOverAll()
        {
            _maskOverAll.SetActive(false);
        }
    }
}