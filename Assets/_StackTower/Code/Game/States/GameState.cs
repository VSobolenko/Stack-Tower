using System;
using Game.Pools;
using StackTower.Code.DI;
using StackTower.Code.Game.Drag;
using StackTower.Code.Game.Logics;
using StackTower.Code.Game.SaveLoad;
using StackTower.Code.Game.States.GameCommands;
using StackTower.Code.Game.View;
using StackTower.Code.Game.View.Factories;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace StackTower.Code.Game.States
{
internal class GameState : IDisposable
{
    private readonly IShapeViewUIFactory _shapeFactory;
    private readonly ITower<CubeModel> _tower;
    private readonly SceneComponents _sceneComponents;
    private readonly TowerInvoker _invoker;
    private readonly PointerDragHandler<CubeViewUI> _pointerDragHandler;
    private readonly IObjectResolver _objectResolver;
    private readonly ISaveLoadDirector _saveLoadDirector;

    private HorizontalScrollDragHandler<CubeViewUI> _scrollDragHandler;

    public GameState(SceneComponents sceneComponents, IObjectPoolManager pool, IShapeViewUIFactory shapeFactory,
                     ITower<CubeModel> tower, TowerInvoker invoker, PointerDragHandler<CubeViewUI> pointerDragHandler, IObjectResolver objectResolver, ISaveLoadDirector saveLoadDirector)
    {
        _sceneComponents = sceneComponents;
        _shapeFactory = shapeFactory;
        _tower = tower;
        _invoker = invoker;
        _pointerDragHandler = pointerDragHandler;
        _objectResolver = objectResolver;
        _saveLoadDirector = saveLoadDirector;
    }

    public void Enter(HorizontalScrollDragHandler<CubeViewUI> scrollDragHandler)
    {
        _scrollDragHandler = scrollDragHandler;

        var insertCommand = _objectResolver.Resolve<InsertCommand>();
        var removeCommand = _objectResolver.Resolve<RemoveCommand>();

        scrollDragHandler.OnItemStartDragAndDrop += OnStartDragNewShape;
        _pointerDragHandler.OnDraggableDown += removeCommand.Execute;
        _pointerDragHandler.OnDraggableUp += insertCommand.Execute;

        _invoker.SetActionCommand(TowerAction.Insert, insertCommand)
                .SetActionCommand(TowerAction.Remove, removeCommand);
    }

    private void OnStartDragNewShape(ShapeViewUI<CubeModel> shapeView, PointerEventData eventData)
    {
        var dragged = CreateCube(shapeView.Model, eventData);
        EventDataControlInterception(dragged, eventData);
        _pointerDragHandler.Listen(dragged);
    }

    private CubeViewUI CreateCube(IStackableShape model, PointerEventData eventData)
    {
        var dragged = _shapeFactory.CreateCube(model.Id, _sceneComponents.dragArea);
        dragged.Position = eventData.position;
        dragged.transform.SetAsLastSibling();

        return dragged;
    }

    private static void EventDataControlInterception<T>(T to, PointerEventData eventData)
        where T : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        eventData.pointerPress = to.gameObject;
        eventData.pointerDrag = to.gameObject;
    }

    public void Dispose()
    {
        _pointerDragHandler.OnDraggableDown -= _invoker.GetActionCommand(TowerAction.Insert).Execute;
        _pointerDragHandler.OnDraggableUp -= _invoker.GetActionCommand(TowerAction.Remove).Execute;
        _scrollDragHandler.OnItemStartDragAndDrop -= OnStartDragNewShape;
        _saveLoadDirector.SaveProgress();
    }
}
}