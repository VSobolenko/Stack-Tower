using System;
using UnityEngine;

namespace StackTower.Code.Common
{
internal static class StackTowerExtensions
{
    public static bool IsInRange<T>(this T value, T min, T max) where T : IComparable<T>
        => value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;

    public static Rect ViewRect(this RectTransform transform)
    {
        var rect = transform.rect;
        var position = transform.anchoredPosition;

        return GetRect(position, rect, Vector2.one);
    }

    public static bool ContainsPoint(this Rect rect, Vector2 point) =>
        point.x >= rect.xMin && point.x <= rect.xMax &&
        point.y >= rect.yMin && point.y <= rect.yMax;

    public static bool IntersectsWith(this Rect rect1, Rect rect2) => rect1.Overlaps(rect2);

    public static bool IsFullInside(this Rect parent, Rect child) => parent.IsInsideX(child) && parent.IsInsideY(child);

    public static bool IsFullInside(this RectTransform parent, RectTransform child, Vector2 scaler)
    {
        var parentPos = parent.position;
        var childPos = child.position;

        var rectParent = GetRect(parentPos, parent.rect, scaler);
        var rectChild = GetRect(childPos, child.rect, scaler);

        return rectParent.IsFullInside(rectChild);
    }

    private static Rect GetRect(Vector2 position, Rect rect, Vector2 scaler)
    {
        var posX = position.x - rect.width / 2f * scaler.x;
        var posY = position.y - rect.height / 2f * scaler.y;

        return new Rect(posX, posY, rect.width * scaler.x, rect.height * scaler.y);
    }

    public static bool IsInsideX(this Rect outer, Rect inner) => inner.xMin >= outer.xMin && inner.xMax <= outer.xMax;

    public static bool IsInsideY(this Rect outer, Rect inner) => inner.yMin >= outer.yMin && inner.yMax <= outer.yMax;

    public static int ToInt(this Color color)
    {
        var a = Mathf.RoundToInt(color.a * 255) << 24;
        var r = Mathf.RoundToInt(color.r * 255) << 16;
        var g = Mathf.RoundToInt(color.g * 255) << 8;
        var b = Mathf.RoundToInt(color.b * 255);

        return a | r | g | b;
    }

    public static Color ToColor(this int colorInt)
    {
        var a = ((colorInt >> 24) & 255) / 255f;
        var r = ((colorInt >> 16) & 255) / 255f;
        var g = ((colorInt >> 8) & 255) / 255f;
        var b = (colorInt & 255) / 255f;

        return new Color(r, g, b, a);
    }
}
}