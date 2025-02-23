using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StackTower.Code.Game.Logics;
using StackTower.Code.Game.View;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StackTower.Code.Game.Drag
{
internal class PointerDragHandler<T> : IEnumerable<T> where T : IDraggableObject<T>
{
    private readonly Dictionary<T, Vector2> _offsets = new();
    public event Action<T> OnDraggableUp;
    public event Action<T> OnDraggableDown;
    
    public void Listen(params T[] draggableObjects)
    {
        foreach (var draggableObject in draggableObjects)
        {
            SubscribeToDraggableEvents(draggableObject);
            _offsets[draggableObject] = Vector2.zero;           
        }
    }

    public void Forget(T draggableObject)
    {
        UnsubscribeToDraggableEvents(draggableObject);
    }

    public T GetFirst(Func<T, bool> condition) => _offsets.Keys.First(condition);

    private void SubscribeToDraggableEvents(T draggableObject)
    {
        draggableObject.PointerDown += OnPointerDown;
        draggableObject.PointerBeginDrag += OnPointerDrag;
        draggableObject.PointerDrag += OnPointerDrag;
        draggableObject.PointerUp += OnPointerUp;
    }

    private void UnsubscribeToDraggableEvents(T draggableObject)
    {
        draggableObject.PointerDown -= OnPointerDown;
        draggableObject.PointerBeginDrag -= OnPointerDrag;
        draggableObject.PointerDrag -= OnPointerDrag;
        draggableObject.PointerUp -= OnPointerUp;
    }

    private void OnPointerDown(PointerEventData eventData, T draggable)
    {
        CachePressedPositionOffset(eventData, draggable);
        OnDraggableDown?.Invoke(draggable);
    }

    private void OnPointerDrag(PointerEventData eventData, T draggable)
    {
        if (_offsets.TryGetValue(draggable, out _) == false) 
            CachePressedPositionOffset(eventData, draggable);
        
        draggable.Position = eventData.position + _offsets[draggable];
    }

    private void OnPointerUp(PointerEventData eventData, T draggable)
    {
        _offsets[draggable] = Vector2.zero;
        OnDraggableUp?.Invoke(draggable);
    }

    private void CachePressedPositionOffset(PointerEventData eventData, T draggable)
    {
        var offset = (Vector2) draggable.Position - eventData.pressPosition;
        _offsets[draggable] = offset;
    }

    public IEnumerator<T> GetEnumerator() => _offsets.Select(x => x.Key).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
}