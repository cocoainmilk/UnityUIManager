using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformEx
{
    public static void ForEachRecursively(this Transform parent, System.Action<Transform> action)
    {
        action?.Invoke(parent);
        for(int i = 0; i < parent.childCount; ++i)
        {
            ForEachRecursively(parent.GetChild(i), action);
        }
    }
}

