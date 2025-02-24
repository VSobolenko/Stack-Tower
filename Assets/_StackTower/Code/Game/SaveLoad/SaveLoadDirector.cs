using System;
using System.Linq;
using Game.Repositories;
using StackTower.Code.DI;
using StackTower.Code.Game.Logics;
using StackTower.Code.Game.View;
using StackTower.Code.Game.View.Factories;
using UnityEngine;

namespace StackTower.Code.Game.SaveLoad
{
internal class SaveLoadDirector : ISaveLoadDirector
{
    private readonly ITowerListSavable<CubeModel> _savableTower;
    private readonly IRepository<CubeSavesContainer> _repository;
    private readonly IShapeViewUIFactory _shapeFactory;
    private readonly SceneComponents _sceneComponents;

    public SaveLoadDirector(IShapeViewUIFactory shapeFactory,
                            SceneComponents sceneComponents,
                            ITowerListSavable<CubeModel> savableTower,
                            IRepository<CubeSavesContainer> repository)
    {
        _shapeFactory = shapeFactory;
        _sceneComponents = sceneComponents;
        _savableTower = savableTower;
        _repository = repository;
    }

    public void SaveProgress()
    {
        var container = new CubeSavesContainer
        {
            Chain = _savableTower.Chain.Select(x => new CubeModelSerializable
            {
                Id = x.Id,
                PosX = x.Rect.x,
                PosY = x.Rect.y,
                RectWidth = x.Rect.width,
                RectHeight = x.Rect.height,
            }).ToArray(),
        };

        if (_repository.Read(container.Id) == null)
            _repository.CreateById(container);
        else
            _repository.Update(container);
    }

    public CubeViewUI[] LoadProgress()
    {
        var container = _repository.Read(CubeSavesContainer.SaveVersion);

        if (container == null)
            return Array.Empty<CubeViewUI>();

        var shapes = new CubeViewUI[container.Chain.Length];
        for (var i = 0; i < container.Chain.Length; i++)
        {
            var serializableModel = container.Chain[i];
            shapes[i] = _shapeFactory.CreateCube(serializableModel.Id, _sceneComponents.dragArea);

            var position = new Vector2(serializableModel.PosX, serializableModel.PosY);
            var rectSize = new Vector2(serializableModel.RectWidth, serializableModel.RectHeight);
            var rect = new Rect(position, rectSize);
            shapes[i].AnchorPosition = rect.center;
            shapes[i].Model.Rect = rect;
            _savableTower.Chain.AddLast(shapes[i].Model);
        }

        return shapes;
    }

    public void ClearProgress()
    {
        var container = _repository.Read(CubeSavesContainer.SaveVersion);

        if (container == null)
            return;
        _repository.Delete(container);
    }
}
}