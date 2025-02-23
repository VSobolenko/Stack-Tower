using StackTower.Code.Game.View;

namespace StackTower.Code.Game.States.GameCommands
{
internal interface ICommand
{
    void Execute(CubeViewUI cubeView);
}
}