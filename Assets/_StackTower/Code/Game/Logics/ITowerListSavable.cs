using System.Collections.Generic;

namespace StackTower.Code.Game.Logics
{
internal interface ITowerListSavable<T> where T : IStackableShape
{
    LinkedList<T> Chain { get; set; }
}
}