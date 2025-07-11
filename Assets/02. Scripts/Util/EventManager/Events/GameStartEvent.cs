using UnityEngine;

public class GameStartEvent : GameEvent
{
    private string _teamName;
    public string TeamName =>  _teamName;

    public GameStartEvent(string teamName)
    {
        _teamName = teamName;
    }
}
