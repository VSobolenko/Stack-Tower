namespace StackTower.Code.Game.Logics
{
internal interface ITower<in T> where T : IStackableShape
{
    bool TryInsertShape(T shape);
    bool TryRemoveShape(T shape);
}
}