using System;
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

    internal static string UserSavePath => Application.persistentDataPath + "/UserData/Tower/";

    protected override void Configure(IContainerBuilder builder)
    {
        BindGCLManagers(builder);
        BindDirectors(builder);
        BindFactories(builder);
        BindSceneElements(builder);
        BindGameStates(builder);
        BindCommands(builder);
        BindMics(builder);
    }

    private static void BindGCLManagers(IContainerBuilder builder)
    {
        builder.RegisterInstance<IResourceManager>(ResourceManagerInstaller.Addressable());
        builder.RegisterInstance<IObjectPoolManager>(
            ObjectPoolInstaller.Type(
                FactoryInstaller.Standart()
            ));
        builder.RegisterInstance<IRepository<CubeSavesContainer>>(
            RepositoryInstaller.File<CubeSavesContainer>(
                UserSavePath, SaveSystemInstaller.FileSaver()
            ));
    }

    private static void BindDirectors(IContainerBuilder builder)
    {
        builder.Register<LocalizationDirector>(Lifetime.Singleton).As<ILocalizationDirector>();
        builder.Register<SaveLoadDirector>(Lifetime.Singleton).As<ISaveLoadDirector>();
        builder.Register<StaticDataDirector>(Lifetime.Singleton)
               .AsImplementedInterfaces()
               .AsSelf();
    }

    private static void BindFactories(IContainerBuilder builder)
    {
        builder.Register<ShapeViewUIFactory>(Lifetime.Singleton).AsImplementedInterfaces();
    }

    private void BindSceneElements(IContainerBuilder builder)
    {
        builder.RegisterComponent<TowerActionInformer>(_informer).As<IInformer>();
        builder.RegisterInstance<SceneComponents>(_sceneComponents);
    }

    private static void BindGameStates(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<EntryPointState>(Lifetime.Scoped);
        builder.Register<GameState>(Lifetime.Scoped)
               .AsSelf()
               .As<IDisposable>();
    }

    private static void BindCommands(IContainerBuilder builder)
    {
        builder.Register<InsertCommand>(Lifetime.Transient).AsSelf();
        builder.Register<RemoveCommand>(Lifetime.Transient).AsSelf();
        builder.Register<TowerInvoker>(Lifetime.Scoped).AsSelf();
    }

    private void BindMics(IContainerBuilder builder)
    {
        builder.Register<PointerDragHandler<CubeViewUI>>(Lifetime.Scoped).AsSelf();
        builder.Register<Tower<CubeModel>>(Lifetime.Scoped)
               .As<ITower<CubeModel>>()
               .As<ITowerListSavable<CubeModel>>()
               .WithParameter<Func<Rect>>(
                   () => _sceneComponents.towerRectTransform.ViewRect(_sceneComponents.canvas.localScale)
                   );
    }
}

[Serializable]
public class SceneComponents
{
    public Transform canvas;
    public ScrollRect scrollRect;
    public RectTransform dragArea;
    public RectTransform towerRectTransform;
    public RectTransform holeRectTransform;
    public Transform fallHoleParent;
    public Transform holePit;
}
}