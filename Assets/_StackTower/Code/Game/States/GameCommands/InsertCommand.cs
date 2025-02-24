using Game.Pools;
using StackTower.Code.Common;
using StackTower.Code.DI;
using StackTower.Code.Game.Drag;
using StackTower.Code.Game.Logics;
using StackTower.Code.Game.View;
using StackTower.Code.Game.View.Informers;

namespace StackTower.Code.Game.States.GameCommands
{
internal class InsertCommand : ICommand
{
    private readonly ITower<CubeModel> _tower;
    private readonly SceneComponents _sceneComponents;
    private readonly IObjectPoolManager _pool;
    private readonly PointerDragHandler<CubeViewUI> _pointerDragHandler;
    private readonly IInformer _informer;

    public InsertCommand(ITower<CubeModel> receiver,
                         SceneComponents sceneComponents,
                         IObjectPoolManager pool,
                         PointerDragHandler<CubeViewUI> pointerDragHandler,
                         IInformer informer)
    {
        _tower = receiver;
        _sceneComponents = sceneComponents;
        _pool = pool;
        _pointerDragHandler = pointerDragHandler;
        _informer = informer;
    }

    public void Execute(CubeViewUI cubeView)
    {
        cubeView.Model.Rect = cubeView.SelfTransform.ViewRect(_sceneComponents.canvas.localScale);

        var result = _tower.TryInsertShape(cubeView.Model);
        if (result == TowerInsertResponse.InsertSuccess)
            ProcessSuccessInsert(cubeView);
        else
            ProcessFailInsert(cubeView, result);
    }

    private void ProcessSuccessInsert(CubeViewUI cubeView)
    {
        cubeView.AnimatePutOnTower(cubeView.Model.Rect.center);
        _informer.Inform(TowerInsertResponse.InsertSuccess.ToString());
    }

    private void ProcessFailInsert(CubeViewUI cubeView, TowerInsertResponse reason)
    {
        _pointerDragHandler.Forget(cubeView);

        if (IsShapeOverHole(cubeView))
        {
            var point = _sceneComponents.holePit.position;
            var parent = _sceneComponents.fallHoleParent;

            cubeView.transform.SetParent(parent);
            cubeView.AnimateFallToHole(point, fallback: _pool.Release);
            _informer.Inform("FallToHole");
        }
        else
        {
            cubeView.AnimateMissedTower(fallback: _pool.Release);
            _informer.Inform(reason.ToString());
        }
    }

    private bool IsShapeOverHole(CubeViewUI cubeView) =>
        _sceneComponents.holeRectTransform.ViewRect(_sceneComponents.canvas.localScale)
                        .IsFullInside(cubeView.SelfTransform.ViewRect(_sceneComponents.canvas.localScale));
}
}