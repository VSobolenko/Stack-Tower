using System.Collections.Generic;
using StackTower.Code.Game.View;

namespace StackTower.Code.Game.States.GameCommands
{
internal enum TowerAction : byte
{
    Unknown = 0,
    Insert = 1,
    Remove = 2,
}

internal class TowerInvoker
{
    private readonly Dictionary<TowerAction, ICommand> _commands = new(2);

    public TowerInvoker SetActionCommand(TowerAction actionType, ICommand command)
    {
        _commands[actionType] = command;
        return this;
    }
    
    public ICommand GetActionCommand(TowerAction actionType) => _commands[actionType];

    public void ExecuteWithCube(TowerAction actionType, CubeViewUI cubeView)
    {
        if (_commands.TryGetValue(actionType, out var commands) == false)
            return;

        commands?.Execute(cubeView);
    }
}
}