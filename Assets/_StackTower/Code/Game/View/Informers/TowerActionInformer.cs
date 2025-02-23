using System;
using DG.Tweening;
using Game.Extensions;
using StackTower.Code.Game.Localizations;
using TMPro;
using UnityEngine;
using VContainer;

namespace StackTower.Code.Game.View.Informers
{
internal class TowerActionInformer : MonoBehaviour, IInformer
{
    [SerializeField] private TextMeshProUGUI _text;
    
    private ILocalizationDirector _localization;
    private Tween _tween;
    
    [Inject]
    public void Construct(ILocalizationDirector localization) => _localization = localization;

    public void Inform(string localizationKey)
    {
        var translate = _localization.Localize(localizationKey);
        PlayTextAnimation(translate);
    }

    public void Inform(string localizationKey, object value)
    {
        var translate = _localization.Localize(localizationKey);
        translate = string.Format(translate, value);
        PlayTextAnimation(translate);
    }

    private void PlayTextAnimation(string newText)
    {
        _tween?.Kill();
        _tween = transform.DOScale(1.1f, 0.1f).SetEase(Ease.OutQuad)
                       .OnComplete(() =>
                       {
                           _text.text = newText;
                           _tween = transform.transform.DOScale(1f, 0.2f).SetEase(Ease.InQuad);
                       });
    }

    private void OnDestroy() => _tween?.Kill();
    
#if UNITY_EDITOR
    private void OnValidate() => this.With(x => x._text = GetComponentInChildren<TextMeshProUGUI>(), _text == null)
                                     ;
#endif
}
}