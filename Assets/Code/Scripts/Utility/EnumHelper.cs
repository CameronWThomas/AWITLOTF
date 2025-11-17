using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumHelper
{
    public static IEnumerable<T> GetValues<T>() where T : Enum
    {
        foreach (var enumValue in Enum.GetValues(typeof(T)).Cast<T>())
            yield return enumValue;
    }
}