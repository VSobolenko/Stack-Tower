using System;
using System.Collections.Generic;
using Game.Localizations;
using StackTower.Code.Game.Logics;

namespace StackTower.Code.Game.Localizations
{
internal class LocalizationDirector : ILocalizationDirector
{
    public event Action OnLanguageUpdate;

    private LanguageType _activeLanguage = LanguageType.English;

    private readonly Dictionary<(string, LanguageType), string> _simpleTranslator =
        new Dictionary<(string, LanguageType), string>
        {
            {(TowerInsertResponse.InsertSuccess.ToString(), LanguageType.English), "Insert Success"},
            {(TowerInsertResponse.NotIntersectsWithTower.ToString(), LanguageType.English), "Not on top of the tower!"},
            {(TowerInsertResponse.OutsideFillArea.ToString(), LanguageType.English), "Out of zone!"},
            {(TowerInsertResponse.HeightLimit.ToString(), LanguageType.English), "Height limit!"},
            {(TowerRemoveResponse.RemoveSuccess.ToString(), LanguageType.English), "Shape remove success!"},
            {(TowerRemoveResponse.UnknownShape.ToString(), LanguageType.English), "Unknown shape!"},
            {("FallToHole", LanguageType.English), "The shape is thrown into the pit!"},
            {("OnLoad", LanguageType.English), "{0} saved shapes loaded"},
            
            {(TowerInsertResponse.InsertSuccess.ToString(), LanguageType.Russian), "Успешно вставлен!"},
            {(TowerInsertResponse.NotIntersectsWithTower.ToString(), LanguageType.Russian), "Нет пересечений с башней!"},
            {(TowerInsertResponse.OutsideFillArea.ToString(), LanguageType.Russian), "Вне зоны башни!"},
            {(TowerInsertResponse.HeightLimit.ToString(), LanguageType.Russian), "Предел высоты!"},
            {(TowerRemoveResponse.RemoveSuccess.ToString(), LanguageType.Russian), "Фигура удалена успешно!"},
            {(TowerRemoveResponse.UnknownShape.ToString(), LanguageType.Russian), "Неизвестная фигура!"},
            {("FallToHole", LanguageType.Russian), "Фигура выброшена в яму!"},
            {("OnLoad", LanguageType.Russian), "Загружено {0} сохраненных фигур"},
        };

    public string Localize(string key) => _simpleTranslator[(key, _activeLanguage)];

    public void SetLanguage(LanguageType language)
    {
        _activeLanguage = language;
        OnLanguageUpdate?.Invoke();
    }
}
}