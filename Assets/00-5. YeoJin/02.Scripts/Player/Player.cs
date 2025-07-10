using System.Collections.Generic;
using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Dictionary<Type, PlayerActivity> _cache = new();

    public T GetActivity<T>() where T : PlayerActivity
    {
        var type = typeof(T);

        if (_cache.TryGetValue(type, out var activity))
            return (T)activity;

        activity = GetComponent<T>() ?? GetComponentInChildren<T>();
        if (activity != null)
        {
            _cache[type] = activity;
            return (T)activity;
        }

        throw new Exception($"Activity {type.Name} not found on {gameObject.name}.");
    }
}
