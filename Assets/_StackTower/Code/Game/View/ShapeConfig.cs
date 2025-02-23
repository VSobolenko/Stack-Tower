using System;
using Game.Extensions;
using NaughtyAttributes;
using StackTower.Code.Common;
using UnityEngine;

namespace StackTower.Code.Game.View
{
[CreateAssetMenu(menuName = "Stack Tower/ShapeConfig", fileName = nameof(ShapeConfig), order = 0)]
internal class ShapeConfig : ScriptableObject
{
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private CubeShapeData[] _shapes;

    public CubeShapeData[] Cubes => _shapes.With(_shapes.Length, (i, array) => array[i].Sprite = _defaultSprite);

#if UNITY_EDITOR
    [ContextMenu("Auto Fill"), Button("Auto Fill", EButtonEnableMode.Editor)]
    private void AutoFill()
    {
        const int countFill = 20;
        _shapes = new CubeShapeData[countFill];
        for (var i = 0; i < _shapes.Length; i++)
        {
            _shapes[i] = new CubeShapeData
            {
                Color = Color.HSVToRGB((float) i / countFill, 1f, 1f),
                Sprite = _defaultSprite,
            };
        }
    }
#endif
}

[Serializable]
public class CubeShapeData
{
    [field: SerializeField] public Color Color { get; internal set; }
    public Sprite Sprite { get; internal set; }
    public int Id => Color.ToInt();
}
}