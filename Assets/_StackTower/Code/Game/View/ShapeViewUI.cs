using Game.Pools;

namespace StackTower.Code.Game.View
{
internal abstract class ShapeViewUI<T> : BasePooledObject
{
    public T Model { get; protected set; }
}
}