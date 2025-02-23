using Game.AssetContent;
using StackTower.Code.Game.View;
using VContainer.Unity;

namespace StackTower.Code.Common
{
public class StaticDataDirector : IInitializable
{
    private readonly IResourceManager _resource;

    public CubeShapeData[] Cubes { get; private set; }

    public StaticDataDirector(IResourceManager resource)
    {
        _resource = resource;
    }

    public void Initialize()
    {
        Cubes = _resource.LoadAsset<ShapeConfig>(nameof(ShapeConfig)).Cubes;
    }
}
}