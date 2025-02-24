using StackTower.Code.Game.Drag;
using StackTower.Code.Game.Logics;
using StackTower.Code.Game.View;
using StackTower.Code.Game.View.Informers;

namespace StackTower.Code.Game.States.GameCommands
{
internal class RemoveCommand : ICommand
{
    private readonly ITower<CubeModel> _tower;
    private readonly PointerDragHandler<CubeViewUI> _pointerDragHandler;
    private readonly IInformer _informer;
    private readonly GameState _gameState;

    public RemoveCommand(ITower<CubeModel> receiver,
                         PointerDragHandler<CubeViewUI> pointerDragHandler,
                         IInformer informer,
                         GameState gameState)
    {
        _tower = receiver;
        _pointerDragHandler = pointerDragHandler;
        _informer = informer;
        _gameState = gameState;
    }

    public void Execute(CubeViewUI cubeView)
    {
        cubeView.transform.SetAsLastSibling();

        var result = _tower.TryRemoveShape(cubeView.Model);
        if (result != TowerRemoveResponse.RemoveSuccess)
            ProcessFailInsert(cubeView);

        _gameState.UpdateTowerView();
        _informer.Inform(result.ToString());
    }

    private void ProcessFailInsert(CubeViewUI cubeView) => _pointerDragHandler.Forget(cubeView);
}
}