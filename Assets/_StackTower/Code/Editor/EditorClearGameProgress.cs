using Game.IO.Installers;
using Game.Repositories.Installers;
using GameEditor.ProjectTools;
using StackTower.Code.DI;
using StackTower.Code.Game.SaveLoad;
using UnityEditor;

namespace StackTower.Code.Editor
{
public class EditorClearGameProgress : ActionsWindowEditorTool
{
    [MenuItem("Stack Tower/Clear User Data", false)]
    private static void ClearAllGameProgress() => ClearFolderData();

    [MenuItem("Stack Tower/Open Data Manager", false)]
    private static void ShowWindow() => ShowWindow<EditorClearGameProgress>();

    protected override void AddSetups()
    {
        AddButton("Clear user data", ClearFolderData)
            .AddLabel($"User Data: {StackTowerLifetimeScope.UserSavePath}")
            .AddClearPlayerPrefsButton();
    }

    private static void ClearFolderData()
    {
        var repository = RepositoryInstaller.File<CubeSavesContainer>(
            StackTowerLifetimeScope.UserSavePath, SaveSystemInstaller.FileSaver()
        );
        var cubeSavesContainer = repository.ReadAll();

        foreach (var container in cubeSavesContainer)
        {
            if (container == null)
                continue;
            repository.Delete(container);
        }
    }
}
}