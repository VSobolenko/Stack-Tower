using System;
using System.Linq;
using Game.AssetContent;
using Game.Pools;
using StackTower.Code.Common;
using StackTower.Code.Game.Logics;
using UnityEngine;
using VContainer.Unity;

namespace StackTower.Code.Game.View.Factories
{
internal class ShapeViewUIFactory : IShapeViewUIFactory, IInitializable
{
    private readonly IObjectPoolManager _pool;
    private readonly IResourceManager _resource;
    private readonly StaticDataDirector _staticDataDirector;

    public ShapeViewUIFactory(IObjectPoolManager pool, IResourceManager resource, StaticDataDirector staticDataDirector)
    {
        _pool = pool;
        _resource = resource;
        _staticDataDirector = staticDataDirector;
    }

    public void Initialize()
    {
        var cubePrefab = _resource.LoadAsset<GameObject>(nameof(CubeViewUI)).GetComponent<CubeViewUI>();
        _pool.Prepare(cubePrefab, _staticDataDirector.Cubes.Length);
    }

    public CubeViewUI CreateCube(int id, Transform parent)
    {
        var cubeData = _staticDataDirector.Cubes.First(x => x.Id == id);
        var prefab = _resource.LoadAsset<GameObject>(nameof(CubeViewUI)).GetComponent<CubeViewUI>();
        var cubeView = _pool.Get(prefab, parent);

        cubeView.Initialize(new CubeModel(cubeData.Id) {Rect = Rect.zero}, cubeData);

        return cubeView;
    }
}
}