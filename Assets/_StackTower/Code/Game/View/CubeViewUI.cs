using System;
using DG.Tweening;
using Game.Extensions;
using StackTower.Code.Game.Drag;
using StackTower.Code.Game.Logics;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StackTower.Code.Game.View
{
[RequireComponent(typeof(CanvasGroup))]
internal class CubeViewUI : ShapeViewUI<CubeModel>, IDraggableObject<CubeViewUI>
{
    [SerializeField] private RectTransform _self;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _image;
    [SerializeField] private float _animationDuration = 0.2f;

    public event Action<PointerEventData, CubeViewUI> PointerDown;
    public event Action<PointerEventData, CubeViewUI> PointerDrag;
    public event Action<PointerEventData, CubeViewUI> PointerBeginDrag;
    public event Action<PointerEventData, CubeViewUI> PointerUp;

    public RectTransform SelfTransform => _self;

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    private Tween _animationTween;

    public void Initialize(CubeModel model, CubeShapeData shapeData)
    {
        Model = model;

        _image.sprite = shapeData.Sprite;
        _image.color = shapeData.Color;
    }

    public override void OnRelease()
    {
        KillTweenAnimations();
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
        _canvasGroup.alpha = 1;
    }

    private void OnDestroy() => KillTweenAnimations();

    public void AnimatePutOnTower(Vector2 rectCenter)
    {
        KillTweenAnimations();
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveY(rectCenter.y + Model.Rect.height / 3f, _animationDuration)
                                 .SetEase(Ease.OutQuad))
                .Append(transform.DOMove(rectCenter, _animationDuration).SetEase(Ease.InQuad));
        _animationTween = sequence;
    }

    public void AnimateLowerDown(Vector2 rectCenter, float delay)
    {
        KillTweenAnimations();
        _animationTween = transform.DOMove(rectCenter, _animationDuration).SetEase(Ease.InOutQuad).SetDelay(delay);
    }

    public void AnimateMissedTower(Action<CubeViewUI> fallback)
    {
        KillTweenAnimations();
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(Vector3.zero, _animationDuration).SetEase(Ease.InBack))
                .Join(transform.DORotate(Vector3.forward * 180, _animationDuration, RotateMode.FastBeyond360))
                .OnComplete(() => fallback?.Invoke(this));

        _animationTween = sequence;
    }

    public void AnimateFallToHole(Vector3 holePosition, Action<CubeViewUI> fallback)
    {
        KillTweenAnimations();

        var sequence = DOTween.Sequence();
        sequence.Append(transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360).SetEase(Ease.InQuad))
                .Join(transform.DOMove(holePosition, 1f).SetEase(Ease.InExpo))
                .Join(transform.DOScale(Vector3.one * 0.7f, 1f).SetEase(Ease.InExpo))
                .OnComplete(() => fallback?.Invoke(this));

        _animationTween = sequence;
    }

    private void KillTweenAnimations() => _animationTween?.Kill();

    public void OnPointerDown(PointerEventData eventData) => PointerDown?.Invoke(eventData, this);
    public void OnBeginDrag(PointerEventData eventData) => PointerBeginDrag?.Invoke(eventData, this);
    public void OnDrag(PointerEventData eventData) => PointerDrag?.Invoke(eventData, this);
    public void OnPointerUp(PointerEventData eventData) => PointerUp?.Invoke(eventData, this);

#if UNITY_EDITOR
    private void OnValidate() => this.With(x => x._image = GetComponent<Image>(), _image == null)
                                     .With(x => x._self = GetComponent<RectTransform>(), _self == null)
                                     .With(x => x._canvasGroup = GetComponent<CanvasGroup>(), _canvasGroup == null);

    private void OnDrawGizmos() => this.With(DrawModelRect, Model != null);

    private void DrawModelRect()
    {
        var rect = Model.Rect;
        Handles.color = new Color(0, 0, 1, 0.2f);
        Vector3[] points =
        {
            new Vector3(rect.xMin, rect.yMax, 0),
            new Vector3(rect.xMax, rect.yMax, 0),
            new Vector3(rect.xMax, rect.yMin, 0),
            new Vector3(rect.xMin, rect.yMin, 0)
        };

        Handles.DrawSolidRectangleWithOutline(points, new Color(0, 0, 1, 0.1f), Color.blue);
    }
#endif
}
}