using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StackTower.Code.Game.Drag
{
internal interface IDraggableObject<out T> : IPointerDownHandler, IDragHandler, IPointerUpHandler, IBeginDragHandler
{
    public event Action<PointerEventData, T> PointerDown;
    public event Action<PointerEventData, T> PointerBeginDrag;
    public event Action<PointerEventData, T> PointerDrag;
    public event Action<PointerEventData, T> PointerUp;
    Vector3 WorldPosition { get; set; }
}
}