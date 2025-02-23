using StackTower.Code.Common;
using StackTower.Code.DI;
using StackTower.Code.Game.Drag;
using StackTower.Code.Game.Logics;
using StackTower.Code.Game.SaveLoad;
using StackTower.Code.Game.View;
using StackTower.Code.Game.View.Factories;
using StackTower.Code.Game.View.Informers;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using VContainer.Unity;

namespace StackTower.Code.Game.States
{
internal class EntryPointState : IInitializable
{
    private readonly IShapeViewUIFactory _shapeFactory;
    private readonly StaticDataDirector _staticDataDirector;
    private readonly SceneComponents _sceneComponents;
    private readonly IObjectResolver _resolver;
    private readonly ISaveLoadDirector _saveLoadDirector;
    private readonly PointerDragHandler<CubeViewUI> _pointerDragHandler;
    private readonly IInformer _informer;

    public EntryPointState(IShapeViewUIFactory shapeFactory, StaticDataDirector staticDataDirector,
                           SceneComponents sceneComponents, IObjectResolver resolver, ISaveLoadDirector saveLoadDirector, PointerDragHandler<CubeViewUI> pointerDragHandler, IInformer informer)
    {
        _shapeFactory = shapeFactory;
        _staticDataDirector = staticDataDirector;
        _sceneComponents = sceneComponents;
        _resolver = resolver;
        _saveLoadDirector = saveLoadDirector;
        _pointerDragHandler = pointerDragHandler;
        _informer = informer;
    }

    public void Initialize()
    {
        var cubeViews = _saveLoadDirector.LoadProgress();
        var shapes = FillScrollRect();
        var scrollDragHandler = InitializeScrollDragHandler(shapes);
        _pointerDragHandler.Listen(cubeViews);
        _informer.Inform("OnLoad", cubeViews.Length);
        TransferToGameState(scrollDragHandler);
    }

    private HorizontalScrollDragHandler<CubeViewUI> InitializeScrollDragHandler(CubeViewUI[] shapes)
    {
        var scrollDragHandler = new HorizontalScrollDragHandler<CubeViewUI>(_sceneComponents.scrollRect);
        scrollDragHandler.Listen(shapes);

        return scrollDragHandler;
    }

    private CubeViewUI[] FillScrollRect()
    {
        var root = _sceneComponents.scrollRect.content;
        var shapes = new CubeViewUI[_staticDataDirector.Cubes.Length];
        for (var i = 0; i < _staticDataDirector.Cubes.Length; i++)
        {
            var shape = _staticDataDirector.Cubes[i];
            shapes[i] = _shapeFactory.CreateCube(shape.Id, root);
        }

        return shapes;
    }

    private void TransferToGameState(HorizontalScrollDragHandler<CubeViewUI> scrollDragHandler)
    {
        var gameState = _resolver.Resolve<GameState>();
        gameState.Enter(scrollDragHandler);
    }
}
}