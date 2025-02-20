using UnityEngine;

namespace StackTower.Code.Game.Logics
{
public interface IStackableShape
{
    int Id { get; }
    Rect Rect { get; set; }
}
}