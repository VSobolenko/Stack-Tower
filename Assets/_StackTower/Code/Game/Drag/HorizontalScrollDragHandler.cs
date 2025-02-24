using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StackTower.Code.Game.Drag
{
internal class HorizontalScrollDragHandler<T> where T : IDraggableObject<T>
{
    private readonly ScrollRect _scroll;

    public event Action<T, PointerEventData> OnItemStartDragAndDrop;

    public HorizontalScrollDragHandler(ScrollRect scroll) => _scroll = scroll;

    public void Listen(params T[] draggableObjects)
    {
        foreach (var draggableObject in draggableObjects)
            SubscribeToDraggableEvents(draggableObject);
    }

    public void Forget(params T[] draggableObjects)
    {
        foreach (var draggableObject in draggableObjects)
            UnsubscribeToDraggableEvents(draggableObject);
    }

    private void SubscribeToDraggableEvents(T draggableObject)
    {
        draggableObject.PointerBeginDrag += OnPointerBeginDrag;
    }

    private void UnsubscribeToDraggableEvents(T draggableObject)
    {
        draggableObject.PointerBeginDrag -= OnPointerBeginDrag;
        draggableObject.PointerDrag -= OnScrollDrag;
        draggableObject.PointerUp -= OnScrollEndDrag;
    }

    private void OnPointerBeginDrag(PointerEventData data, T draggableObject)
    {
        var angle = Mathf.Abs(180F - ConvertAngle360(Vector2.SignedAngle(Vector2.down, data.delta.normalized)));
        OnScrollPointerDown();

        if (angle < 60f)
        {
            OnItemStartDragAndDrop?.Invoke(draggableObject, data);
        }
        else
        {
            draggableObject.PointerDrag += OnScrollDrag;
            draggableObject.PointerUp += OnScrollEndDrag;

            OnScrollBeginDrag(data);
        }
    }

    private void OnScrollBeginDrag(PointerEventData eventData) => _scroll.OnBeginDrag(eventData);

    private void OnScrollDrag(PointerEventData eventData, T _) => _scroll.OnDrag(eventData);

    private void OnScrollEndDrag(PointerEventData eventData, T draggable) => _scroll.OnEndDrag(eventData);

    private void OnScrollPointerDown() => _scroll.StopMovement();

    private static float ConvertAngle360(float value) => value >= 0 ? value : 360F + value;
}
}