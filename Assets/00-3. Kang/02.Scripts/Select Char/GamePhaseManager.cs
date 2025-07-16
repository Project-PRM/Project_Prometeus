using UnityEngine;

public enum GamePhase
{
    SelectCharacter,
    GameStart,
    GameOver,
}

public class GamePhaseManager : MonoBehaviour
{
    public GamePhase CurrentPhase { get; private set; } = GamePhase.SelectCharacter;
    private GamePhase _targetPhase;

    private void Update()
    {
        SwitchPhase();
    }
    private void SwitchPhase()
    {
        if (_targetPhase == CurrentPhase) return;

        CurrentPhase = _targetPhase;
    }
    public void SetPhase(GamePhase phase)
    {
        _targetPhase = phase;
    }
}
