using System;
using System.Collections.Generic;
using Game.AssetContent;
using Game.AssetContent.Installers;
using Game.Factories.Installers;
using Game.IO.Installers;
using Game.Pools;
using Game.Pools.Installers;
using Game.Repositories;
using Game.Repositories.Installers;
using StackTower.Code.Common;
using StackTower.Code.Game.Drag;
using StackTower.Code.Game.Localizations;
using StackTower.Code.Game.Logics;
using StackTower.Code.Game.SaveLoad;
using StackTower.Code.Game.States;
using StackTower.Code.Game.States.GameCommands;
using StackTower.Code.Game.View;
using StackTower.Code.Game.View.Factories;
using StackTower.Code.Game.View.Informers;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace StackTower.Code.DI
{
public class StackTowerLifetimeScope : LifetimeScope
{
    [SerializeField] private SceneComponents _sceneComponents;
    [SerializeField] private TowerActionInformer _informer;

    protected override void Configure(IContainerBuilder builder)
    {
        var cubeRepository = Application.persistentDataPath + "/UserData/Tower/";
        builder.RegisterInstance<IObjectPoolManager>(ObjectPoolInstaller.Type(FactoryInstaller.Standart()));
        builder.RegisterInstance<IResourceManager>(ResourceManagerInstaller.Addressable());
        builder.RegisterInstance<IRepository<CubeSavesContainer>>(RepositoryInstaller.File<CubeSavesContainer>(cubeRepository, SaveSystemInstaller.FileSaver()));
        builder.Register<StaticDataDirector>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<ShapeViewUIFactory>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.Register<LocalizationDirector>(Lifetime.Singleton).As<ILocalizationDirector>();
        builder.Register<SaveLoadDirector>(Lifetime.Singleton).As<ISaveLoadDirector>();
        builder.RegisterComponent<TowerActionInformer>(_informer).As<IInformer>();
        builder.RegisterInstance<SceneComponents>(_sceneComponents);
        builder.RegisterEntryPoint<EntryPointState>(Lifetime.Scoped);
        builder.Register<GameState>(Lifetime.Scoped).AsSelf().As<IDisposable>();
        builder.Register<InsertCommand>(Lifetime.Transient).AsSelf();
        builder.Register<RemoveCommand>(Lifetime.Transient).AsSelf();
        builder.Register<PointerDragHandler<CubeViewUI>>(Lifetime.Scoped).AsSelf();
        builder.Register<TowerInvoker>(Lifetime.Scoped).AsSelf();
        builder.Register<Tower<CubeModel>>(Lifetime.Scoped).As<ITower<CubeModel>>().As<ITowerListSavable<CubeModel>>()
               .WithParameter<Rect>(_sceneComponents.towerRectTransform.ViewRect());
    }
}

[Serializable]
public class SceneComponents
{
    public ScrollRect scrollRect;
    public RectTransform dragArea;
    public RectTransform towerRectTransform;
    public RectTransform holeRectTransform;
    public Transform fallHoleParent;
    public Transform holePit;
}
}