using System;
using UnityEngine;

namespace StackTower.Code.Common
{
internal static class StackTowerExtensions
{
    public static bool IsInRange<T>(this T value, T min, T max) where T : IComparable<T>
        => value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;

    public static bool IntersectsWith(this Rect rect1, Rect rect2) => rect1.Overlaps(rect2);

    public static bool IsFullyContainedInside(this Rect rect, Rect inArea)
    {
        return inArea.Contains(rect.center) &&
               inArea.Contains(rect.min) &&
               inArea.Contains(rect.max);
    }
}
}