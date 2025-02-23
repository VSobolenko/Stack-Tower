using System;
using UnityEngine;

namespace StackTower.Code.Game.Logics
{
public interface IStackableShape
{
    int Id { get; }
    Rect Rect { get; set; }
}

public class CubeModel : IStackableShape
{
    public CubeModel(int id)
    {
        Id = id;
    }

    public int Id { get; }
    public Rect Rect { get; set; }

    public override string ToString() => $"Id={Id}; Rect={Rect}";
}
}