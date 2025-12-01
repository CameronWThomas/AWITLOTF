using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Random;

public static class RandomHelper
{
    private static bool _initialized = false;

    public static void Init()
    {
        if (_initialized) return;

        InitState((int)(DateTime.UtcNow.Ticks % int.MaxValue));
        _initialized = true;
    }

    public static float Random0_1 => Between(0f, 1f);
    public static float RandomSign => Range(0, 2) == 0 ? 1f : -1f;

    public static Vector3 Direction()
    {
        var vector = new Vector3(RandomSign * Random0_1, RandomSign * Random0_1, RandomSign * Random0_1);
        return vector.normalized;
    }

    public static int Between(int minInclusive, int maxInclusive)
    {
        Init();

        maxInclusive = maxInclusive == int.MaxValue ? int.MaxValue : maxInclusive + 1;
        return UnityEngine.Random.Range(minInclusive, maxInclusive);
    }

    public static float Between(float minInclusive, float maxInclusive)
    {
        Init();
        return UnityEngine.Random.Range(minInclusive, maxInclusive);
    }

    public static T Random<T>() where T : Enum
    {
        Init();
        return EnumHelper.GetValues<T>().Random();
    }

    #region enumerable stuff
    public static IEnumerable<T> Randomize<T>(this IEnumerable<T> enumerable)
    {
        Init();

        var originalList = enumerable.ToList();
        var randomList = new List<T>();

        while (originalList.Any())
        {
            var randomItem = originalList[Range(0, originalList.Count)];
            randomList.Add(randomItem);
            originalList.Remove(randomItem);
        }

        return randomList;
    }

    public static T Random<T>(this IEnumerable<T> objects)
    {
        return objects.Randomize().FirstOrDefault();
    }
    #endregion
}