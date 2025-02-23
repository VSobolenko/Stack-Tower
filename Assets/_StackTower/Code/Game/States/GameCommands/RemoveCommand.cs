using StackTower.Code.Game.Drag;
using StackTower.Code.Game.Logics;
using StackTower.Code.Game.View;
using StackTower.Code.Game.View.Informers;
using UnityEngine;

namespace StackTower.Code.Game.States.GameCommands
{
internal class RemoveCommand : ICommand
{
    private readonly ITower<CubeModel> _tower;
    private readonly PointerDragHandler<CubeViewUI> _pointerDragHandler;
    private readonly IInformer _informer;

    public RemoveCommand(ITower<CubeModel> receiver, PointerDragHandler<CubeViewUI> pointerDragHandler, IInformer informer)
    {
        _tower = receiver;
        _pointerDragHandler = pointerDragHandler;
        _informer = informer;
    }

    public void Execute(CubeViewUI cubeView)
    {
        cubeView.transform.SetAsLastSibling();
        
        var result = _tower.TryRemoveShape(cubeView.Model);
        if (result != TowerRemoveResponse.RemoveSuccess)
            ProcessFailInsert(cubeView);
        
        UpdateTowerView();
        _informer.Inform(result.ToString());
    }

    private void ProcessFailInsert(CubeViewUI cubeView) => _pointerDragHandler.Forget(cubeView);

    private void UpdateTowerView()
    {
        var animationDelay = 0f;
        foreach (var cubeModel in _tower)
        {
            var cubeView = _pointerDragHandler.GetFirst(x => x.Model == cubeModel);
            cubeView.AnimateLowerDown(cubeModel.Rect.center, animationDelay);
            animationDelay += 0.05f;
        }
    }
}
}