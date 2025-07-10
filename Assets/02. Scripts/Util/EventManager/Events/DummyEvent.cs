using UnityEngine;

public class DummyEvent : GameEvent
{
    private string _message;
    public string Message => _message;

    public DummyEvent(string message)
    {
        _message = message;
    }
}