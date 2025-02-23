using UnityEngine;

namespace StackTower.Code.Game.View.Factories
{
internal interface IShapeViewUIFactory
{
    CubeViewUI CreateCube(int id, Transform parent);
}
}