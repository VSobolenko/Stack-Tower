using System;
using System.Collections.Generic;
using Moq;
using UnityEngine;
using NUnit.Framework;
using StackTower.Code.Common;
using StackTower.Code.Game.Logics;

namespace StackTower.Code.Tests.StackTowerTests
{
[TestFixture]
internal class TowerTests
{
    private const float FloatTolerances = 0.2f;

    [Test]
    public void TryInsertShape_InsertedShapeInFillRectX_ShouldInsertInRightPositionAndReturnTrue()
    {
        var tower = new Tower<IStackableShape>(() => GetRectInCenter(1.5f, float.MaxValue));

        for (float i = 0f, x = 0f, y = 0f; i < 30; i++, y++)
        {
            var realPosition = new Vector2(x, i == 0 ? y : y - 0.1f);
            var shapeMock = new Mock<IStackableShape>()
                .SetupProperty(shape => shape.Rect, new Rect(realPosition, NormalizedVector()));

            var insertResult = tower.TryInsertShape(shapeMock.Object);

            Assert.AreEqual(TowerInsertResponse.InsertSuccess, insertResult, $"Error inside {i} iteration");
            Assert.AreEqual(y, shapeMock.Object.Rect.position.y, $"Error inside {i} iteration");
        }
    }

    [Test]
    public void TryInsertShape_InsertedShapeOutsideFillArea_ShouldSkipInsertAndReturnFalse()
    {
        var tower = new Tower<IStackableShape>(() => GetRectInCenter(float.MaxValue, 0.5f));
        var stack = new[]
        {
            new Mock<IStackableShape>().SetupProperty(x => x.Rect, new Rect(Vector2.zero, NormalizedVector())),
            new Mock<IStackableShape>().SetupProperty(x => x.Rect, new Rect(Vector2.one / 1.1f, NormalizedVector())),
        };

        tower.TryInsertShape(stack[0].Object);
        tower.TryInsertShape(stack[0].Object);
        var insertResult = tower.TryInsertShape(stack[1].Object);

        Assert.AreEqual(TowerInsertResponse.OutsideFillArea, insertResult);
    }

    [Test]
    public void TryInsertShape_NullShape_ShouldThrowException()
    {
        var tower = new Tower<IStackableShape>(() => Rect.zero);

        var ex = Assert.Throws<ArgumentNullException>(() => tower.TryInsertShape(null));
        StringAssert.StartsWith("Null shape Not supported!", ex.Message);
    }

    [Test]
    public void TryInsertShape_WhenNewShapeIntersectsWithLastTowerShape_ShouldInsertInRightPositionAndReturnTrue()
    {
        var tower = new Tower<IStackableShape>(() => GetRectInCenter(float.MaxValue, float.MaxValue));
        var stack = new[]
        {
            new Mock<IStackableShape>().SetupProperty(x => x.Rect, new Rect(Vector2.zero, NormalizedVector())),
            new Mock<IStackableShape>().SetupProperty(x => x.Rect, new Rect(Vector2.one / 2, NormalizedVector())),
        };

        tower.TryInsertShape(stack[0].Object);
        var insertResult = tower.TryInsertShape(stack[1].Object);

        Assert.AreEqual(TowerInsertResponse.InsertSuccess, insertResult);
        Assert.AreEqual(1, stack[1].Object.Rect.position.y);
        Assert.IsTrue(stack[1].Object.Rect.position.x.IsInRange(stack[1].Object.Rect.xMin, stack[1].Object.Rect.xMax));
    }

    [Test]
    public void TryInsertShape_WhenNewShapeNotIntersectsWithLastTowerShape_ShouldSkipInsertAndReturnFalse()
    {
        var tower = new Tower<IStackableShape>(() => GetRectInCenter(float.MaxValue, float.MaxValue));
        var stack = new[]
        {
            new Mock<IStackableShape>().SetupProperty(x => x.Rect, new Rect(Vector2.zero, NormalizedVector())),
            new Mock<IStackableShape>().SetupProperty(x => x.Rect, new Rect(Vector2.one * 2, NormalizedVector())),
        };

        tower.TryInsertShape(stack[0].Object);
        var insertResult = tower.TryInsertShape(stack[1].Object);

        Assert.AreEqual(TowerInsertResponse.NotIntersectsWithTower, insertResult);
    }

    [Test]
    public void TryInsertShape_WhenTowerIsEmpty_ShouldInsertInSameRectPosition()
    {
        var tower = new Tower<IStackableShape>(() => GetRectInCenter(float.MaxValue, float.MaxValue));
        var mockShape = new Mock<IStackableShape>();
        mockShape.SetupProperty(x => x.Rect, new Rect(Vector2.one, Vector2.zero));

        var insertResult = tower.TryInsertShape(mockShape.Object);

        Assert.AreEqual(TowerInsertResponse.InsertSuccess, insertResult);
        Assert.AreEqual(Vector2.one, mockShape.Object.Rect.position);
    }

    [Test]
    public void RemoveShape_FirstShape_ShouldSecondReturnToFirstYPositionAndRecalculateOtherPosition()
    {
        var tower = new Tower<IStackableShape>(() => GetRectInCenter(1.3f, float.MaxValue));
        var stack = new List<Mock<IStackableShape>>(30);
        Mock<IStackableShape> removed = null;
        for (float i = 0f, x = 0f, y = 0f; i < 30; i++, y++)
        {
            var position = new Vector2(x, i == 0 ? y : y - 0.1f);
            var shapeMock = new Mock<IStackableShape>()
                .SetupProperty(shape => shape.Rect, new Rect(position, NormalizedVector()));

            var insertResult = tower.TryInsertShape(shapeMock.Object);

            Assert.AreEqual(TowerInsertResponse.InsertSuccess, insertResult, $"Error inside {i} iteration");

            if (i == 0)
                removed = shapeMock;
            else
                stack.Add(shapeMock);
        }

        var removeResult = tower.TryRemoveShape(removed!.Object);

        Assert.AreEqual(TowerRemoveResponse.RemoveSuccess, removeResult);
        Assert.AreEqual(removed.Object.Rect.position.y, stack[0].Object.Rect.position.y);

        for (var i = 0; i < stack.Count - 1; i++)
        {
            var shape = stack[i].Object;
            var childShape = stack[i + 1].Object;
            var distance = Vector2.Distance(shape.Rect.position, childShape.Rect.position);

            Assert.AreEqual(distance, shape.Rect.height, FloatTolerances, $"Error inside {i} iteration");
        }
    }

    [Test]
    public void RemoveShape_LastShape_ShouldRemoveLastWithoutRecalculation()
    {
        var tower = new Tower<IStackableShape>(() => GetRectInCenter(1.3f, float.MaxValue));
        var stack = new List<Mock<IStackableShape>>(30);
        Mock<IStackableShape> removed = null;
        for (float i = 0f, x = 0f, y = 0f; i < 30; i++, y++)
        {
            var position = new Vector2(x, i == 0 ? y : y - 0.1f);
            var shapeMock = new Mock<IStackableShape>()
                .SetupProperty(shape => shape.Rect, new Rect(position, NormalizedVector()));

            var insertResult = tower.TryInsertShape(shapeMock.Object);

            Assert.AreEqual(TowerInsertResponse.InsertSuccess, insertResult, $"Error inside {i} iteration");

            if (i >= 28.5f)
                removed = shapeMock;
            else
                stack.Add(shapeMock);
        }

        var removeResult = tower.TryRemoveShape(removed!.Object);

        Assert.AreEqual(TowerRemoveResponse.RemoveSuccess, removeResult);

        for (var i = 0; i < stack.Count - 1; i++)
        {
            var shape = stack[i].Object;
            var childShape = stack[i + 1].Object;
            var distance = Vector2.Distance(shape.Rect.position, childShape.Rect.position);

            Assert.AreEqual(distance, shape.Rect.height, FloatTolerances, $"Error inside {i} iteration");
        }
    }

    [Test]
    public void RemoveShape_NullShape_ShouldThrowException()
    {
        var tower = new Tower<IStackableShape>(() => Rect.zero);

        var ex = Assert.Throws<ArgumentNullException>(() => tower.TryRemoveShape(null));
        StringAssert.StartsWith("Null shape Not supported!", ex.Message);
    }

    [Test]
    public void RemoveShape_ShapeFromCenter_ShouldRecalculateShapePosition()
    {
        var tower = new Tower<IStackableShape>(() => GetRectInCenter(1.3f, float.MaxValue));
        var stack = new List<Mock<IStackableShape>>(30);
        Mock<IStackableShape> removed = null;
        for (float i = 0f, x = 0f, y = 0f; i < 30; i++, y++)
        {
            var position = new Vector2(x, i == 0 ? y : y - 0.1f);
            var shapeMock = new Mock<IStackableShape>()
                .SetupProperty(shape => shape.Rect, new Rect(position, NormalizedVector()));

            var insertResult = tower.TryInsertShape(shapeMock.Object);

            Assert.AreEqual(TowerInsertResponse.InsertSuccess, insertResult, $"Error inside {i} iteration");

            if (i > 14)
                removed = shapeMock;
            else
                stack.Add(shapeMock);
        }

        var removeResult = tower.TryRemoveShape(removed!.Object);

        Assert.AreEqual(TowerRemoveResponse.RemoveSuccess, removeResult);

        for (var i = 0; i < stack.Count - 1; i++)
        {
            var shape = stack[i].Object;
            var childShape = stack[i + 1].Object;
            var distance = Vector2.Distance(shape.Rect.position, childShape.Rect.position);

            Assert.AreEqual(distance, shape.Rect.height, FloatTolerances, $"Error inside {i} iteration");
        }
    }

    [Test]
    public void RemoveShape_UnknownShape_ShouldSkipRemoved()
    {
        var tower = new Tower<IStackableShape>(() => Rect.zero);
        var mockShape = new Mock<IStackableShape>();

        var removeResult = tower.TryRemoveShape(mockShape.Object);

        Assert.AreEqual(TowerRemoveResponse.UnknownShape, removeResult);
    }

    private static Rect GetRectInCenter(float width, float height) =>
        new(Vector2.zero, new Vector2(width, height));

    private static Vector3 NormalizedVector() => new(1, 1, 1);
}
}