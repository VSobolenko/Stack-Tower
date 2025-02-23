using System.Collections.Generic;

namespace StackTower.Code.Game.Logics
{
internal interface ITower<T> : IEnumerable<T> where T : IStackableShape
{
    TowerInsertResponse TryInsertShape(T shape);
    TowerRemoveResponse TryRemoveShape(T shape);
    bool Contains(T shape);
}

internal enum TowerInsertResponse : byte
{
    Unknown = 0,
    InsertSuccess = 1,
    NotIntersectsWithTower = 2,
    OutsideFillArea = 3,
    HeightLimit = 4,
}

internal enum TowerRemoveResponse : byte
{
    Unknown = 0,
    RemoveSuccess = 1,
    UnknownShape = 2,
}
}