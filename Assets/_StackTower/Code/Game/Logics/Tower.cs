using System;
using System.Collections.Generic;
using StackTower.Code.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StackTower.Code.Game.Logics
{
internal class Tower<T> : ITower<T> where T : IStackableShape
{
    private readonly Rect _fillArea;
    private readonly LinkedList<T> _tower = new();

    public Tower(Rect fillArea)
    {
        _fillArea = fillArea;
    }

    public bool TryInsertShape(T shape)
    {
        if (shape == null)
            throw new ArgumentNullException(nameof(shape), "Null shape Not supported!");

        if (_tower.Count == 0)
        {
            _tower.AddFirst(shape);

            return true;
        }

        var lastNode = _tower.Last;

        if (shape.Rect.IntersectsWith(lastNode.Value.Rect) == false)
            return false;

        var y = GetHeightCenterForInsertShape(lastNode.Value, shape);

        if (FillAreaContainsYPoint(y) == false)
            return false;

        InsertShape(lastNode, shape);

        return true;
    }

    public bool TryRemoveShape(T shape)
    {
        if (shape == null)
            throw new ArgumentNullException(nameof(shape), "Null shape Not supported!");

        if (_tower.Contains(shape) == false)
            return false;

        var deletedNode = _tower.Find(shape);

        if (deletedNode == null)
            return false;

        var recalculateFrom = deletedNode.Next;
        _tower.Remove(deletedNode);
        RecalculateShapeRectUpcastAfterDeleteNode(recalculateFrom, deletedNode, recalculateFrom == _tower.First);

        return true;
    }

    private void InsertShape(LinkedListNode<T> after, T shape)
    {
        _tower.AddAfter(after, shape);

        var x = GetRandomCanterWeightForInsertShape(after.Value);
        var y = GetHeightCenterForInsertShape(after.Value, shape);
        x = ClampRectXInsideFillAreaByWidth(shape, x);
        UpdateShapeCenter(shape, x, y);
    }

    private void RecalculateShapeRectUpcastAfterDeleteNode(LinkedListNode<T> recalculateFrom, LinkedListNode<T> deleted,
                                                           bool deletedWasFirst)
    {
        for (var node = recalculateFrom; node != null; node = node.Next)
        {
            if (node == _tower.First && deletedWasFirst == false)
                continue;

            var isRecalculatedFirstNode = node == _tower.First && deletedWasFirst;

            var shape = node.Value;
            var prevShape = isRecalculatedFirstNode ? deleted.Value : node.Previous!.Value;

            var x = GetRelativeCenterWeightForShiftable(prevShape, shape);
            var y = isRecalculatedFirstNode ? prevShape.Rect.center.y : GetHeightCenterForInsertShape(prevShape, shape);
            UpdateShapeCenter(shape, x, y);
        }
    }

    private bool FillAreaContainsYPoint(float yPoint) => yPoint.IsInRange(_fillArea.yMin, _fillArea.yMax);

    private static float GetRandomCanterWeightForInsertShape(T exist) =>
        Random.Range(exist.Rect.xMin - exist.Rect.width / 2, exist.Rect.xMax - exist.Rect.width / 2);

    private static float GetRelativeCenterWeightForShiftable(T exist, T shiftable) =>
        Mathf.Clamp(shiftable.Rect.center.x, exist.Rect.xMin, exist.Rect.xMax);

    private static float GetHeightCenterForInsertShape(T exist, T next) =>
        exist.Rect.yMax + next.Rect.height / 2;

    private static void UpdateShapeCenter(T shape, float centerX, float centerY) =>
        shape.Rect = new Rect(shape.Rect) {center = new Vector2(centerX, centerY)};

    private float ClampRectXInsideFillAreaByWidth(T shape, float x)
    {
        var minX = _fillArea.xMin + shape.Rect.width / 2;
        var maxX = _fillArea.xMax - shape.Rect.width / 2;

        return Mathf.Clamp(x, minX, maxX);
    }
}
}