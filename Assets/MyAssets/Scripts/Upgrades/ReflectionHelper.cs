using System;
using System.Reflection;
using UnityEngine;

public static class ReflectionHelper
{
    public static T GetField<T>(object target, string fieldName)
    {
        FieldInfo f = FindField(target.GetType(), fieldName);
        if (f == null) { Debug.LogWarning($"[Reflection] '{fieldName}' not found on {target.GetType().Name}"); return default; }
        return (T)f.GetValue(target);
    }

    public static void SetField(object target, string fieldName, object value)
    {
        FieldInfo f = FindField(target.GetType(), fieldName);
        if (f == null) { Debug.LogWarning($"[Reflection] '{fieldName}' not found on {target.GetType().Name}"); return; }
        f.SetValue(target, value);
    }

    private static FieldInfo FindField(Type type, string fieldName)
    {
        while (type != null && type != typeof(object))
        {
            FieldInfo f = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (f != null) return f;
            type = type.BaseType; // wals up the inheritance chain (needed for Health.currentHealth/isDead)
        }
        return null;
    }
}
