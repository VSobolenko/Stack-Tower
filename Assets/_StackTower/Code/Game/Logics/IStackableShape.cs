using UnityEngine;

namespace StackTower.Code.Game.Logics
{
public interface IStackableShape
{
    int Id { get; set; }
    Rect Rect { get; set; }
}

internal class CubeModel : IStackableShape
{
    public CubeModel(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
    public Rect Rect { get; set; }

    public override string ToString() => $"Id={Id}; Rect={Rect}";
}
}