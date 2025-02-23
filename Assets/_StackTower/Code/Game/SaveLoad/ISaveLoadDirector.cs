using StackTower.Code.Game.View;

namespace StackTower.Code.Game.SaveLoad
{
internal interface ISaveLoadDirector
{
    void SaveProgress();
    CubeViewUI[] LoadProgress();
    void ClearProgress();
}
}