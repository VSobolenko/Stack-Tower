using Game.IO.Installers;
using Game.Repositories;
using Game.Repositories.Installers;
using GameEditor.ProjectTools;
using StackTower.Code.Game.SaveLoad;
using UnityEditor;
using UnityEngine;

namespace StackTower.Code.Editor
{
public class EditorClearGameProgress : ActionsWindowEditorTool
{
    private static string DataPAth => Application.persistentDataPath + "/UserData/Tower/";

    [MenuItem("Stack Tower/Clear User Data", false)]
    private static void ClearAllGameProgress() => ClearFolderData();

    [MenuItem("Stack Tower/Open Data Manager", false)]
    private static void ShowWindow() => ShowWindow<EditorClearGameProgress>();

    protected override void AddSetups()
    {
        AddButton("Clear user data", ClearFolderData)
            .AddLabel($"User Data: {DataPAth}")
            .AddClearPlayerPrefsButton();
    }

    private static void ClearFolderData()
    {
        var repository = RepositoryInstaller.File<CubeSavesContainer>(DataPAth, SaveSystemInstaller.FileSaver());
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