using System.Collections.Generic;

public static class GameEventPool<T>
    where T : GameEvent<T>, new()
{
    private static readonly Stack<T> _pool = new();

    public static T Get()
    {
        return _pool.Count > 0 ? _pool.Pop() : new T();
    }

    public static void Release(T evt)
    {
        _pool.Push(evt);
    }
}
