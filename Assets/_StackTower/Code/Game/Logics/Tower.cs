using System;
using System.Collections;
using System.Collections.Generic;
using StackTower.Code.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StackTower.Code.Game.Logics
{
internal class Tower<T> : ITower<T>, ITowerListSavable<T> where T : IStackableShape
{
    public LinkedList<T> Chain { get; set; } = new();

    private Rect FillArea => _fillAreaFunc.Invoke();
    private readonly Func<Rect> _fillAreaFunc;

    public Tower(Func<Rect> fillAreaGetter) => _fillAreaFunc = fillAreaGetter;

    public TowerInsertResponse TryInsertShape(T shape)
    {
        if (shape == null)
            throw new ArgumentNullException(nameof(shape), "Null shape Not supported!");

        switch (Chain.Count)
        {
            case 0 when FillArea.IsFullInside(shape.Rect):
                Chain.AddFirst(shape);

                return TowerInsertResponse.InsertSuccess;
            case 0:
                return TowerInsertResponse.OutsideFillArea;
        }

        if (IsAnyIntersectWithTower(shape) == false)
            return TowerInsertResponse.NotIntersectsWithTower;

        var lastNode = Chain.Last;
        var y = GetHeightCenterForInsertShape(lastNode.Value, shape);

        if (FillAreaContainsYPoint(y) == false)
            return TowerInsertResponse.HeightLimit;

        InsertShape(lastNode, shape);

        return TowerInsertResponse.InsertSuccess;
    }

    private bool IsAnyIntersectWithTower(T shape)
    {
        for (var node = Chain.First; node != null; node = node.Next)
            if (shape.Rect.IntersectsWith(node.Value.Rect))
                return true;

        return false;
    }

    public TowerRemoveResponse TryRemoveShape(T shape)
    {
        if (shape == null)
            throw new ArgumentNullException(nameof(shape), "Null shape Not supported!");

        if (Chain.Contains(shape) == false)
            return TowerRemoveResponse.UnknownShape;

        var deletedNode = Chain.Find(shape);

        if (deletedNode == null)
            return TowerRemoveResponse.UnknownShape;

        var recalculateFrom = deletedNode.Next;
        Chain.Remove(deletedNode);
        RecalculateShapeRectUpcastAfterDeleteNode(recalculateFrom, deletedNode, recalculateFrom == Chain.First);

        return TowerRemoveResponse.RemoveSuccess;
    }

    public bool Contains(T shape) => Chain.Contains(shape);

    private void InsertShape(LinkedListNode<T> after, T shape)
    {
        Chain.AddAfter(after, shape);

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
            if (node == Chain.First && deletedWasFirst == false)
                continue;

            var isRecalculatedFirstNode = node == Chain.First && deletedWasFirst;

            var shape = node.Value;
            var prevShape = isRecalculatedFirstNode ? deleted.Value : node.Previous!.Value;

            var x = GetRelativeCenterWeightForShiftable(prevShape, shape);
            var y = isRecalculatedFirstNode ? prevShape.Rect.center.y : GetHeightCenterForInsertShape(prevShape, shape);
            UpdateShapeCenter(shape, x, y);
        }
    }

    private bool FillAreaContainsYPoint(float yPoint) => yPoint.IsInRange(FillArea.yMin, FillArea.yMax);

    private static float GetRandomCanterWeightForInsertShape(T exist) =>
        Random.Range(exist.Rect.xMin, exist.Rect.xMax);

    private static float GetRelativeCenterWeightForShiftable(T exist, T shiftable) =>
        Mathf.Clamp(shiftable.Rect.center.x, exist.Rect.xMin, exist.Rect.xMax);

    private static float GetHeightCenterForInsertShape(T exist, T next) =>
        exist.Rect.yMax + next.Rect.height / 2;

    private static void UpdateShapeCenter(T shape, float centerX, float centerY) =>
        shape.Rect = new Rect(shape.Rect) {center = new Vector2(centerX, centerY)};

    private float ClampRectXInsideFillAreaByWidth(T shape, float x)
    {
        var minX = FillArea.xMin + shape.Rect.width / 2;
        var maxX = FillArea.xMax - shape.Rect.width / 2;

        return Mathf.Clamp(x, minX, maxX);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => Chain.GetEnumerator();
    public IEnumerator GetEnumerator() => Chain.GetEnumerator();
}
}