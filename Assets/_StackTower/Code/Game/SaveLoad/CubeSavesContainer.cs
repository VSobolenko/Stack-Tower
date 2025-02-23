using System;
using Game.Repositories;

namespace StackTower.Code.Game.SaveLoad
{
[Serializable]
internal class CubeSavesContainer : IHasBasicId
{
    public const int SaveVersion = 1000;

    public int Id { get; set; } = SaveVersion;
    public CubeModelSerializable[] Chain { get; set; }
}

[Serializable]
internal class CubeModelSerializable
{
    public int Id { get; set; }
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float RectWidth { get; set; }
    public float RectHeight { get; set; }
}
}