using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public enum SelectPhase
{
    Wait,
    First,
    Second,
    Third,
    End
}

public enum CharacterSample
{
    Character_Red,
    Character_Orange,
    Character_Yellow,
    Character_Green,
    Character_Blue,
}

public class CharacterSelect : PunSingleton<CharacterSelect>
{
    public float SelectTime = 5f;
    public float DelayTime = 0.5f;

    [SerializeField] private SelectPhase _selectPhase;
    [SerializeField] private SelectPhase _myPhase;
    private float _firstTimer = 0f;
    private float _secondTimer = 0f;
    private float _thirdTimer = 0f;

    public SelectPhase SelectPhase => _selectPhase;
    public SelectPhase MyPhase => _myPhase;
    public float FirstTimer => _firstTimer;
    public float SecondTimer => _secondTimer;
    public float ThirdTimer => _thirdTimer;

    [SerializeField] private bool[] _isSelectCharacter = new bool[5] { false, false, false, false, false };
    public bool[] IsSelectCharacter => _isSelectCharacter;

    private bool _isSelect = false;
    public bool IsSelect => _isSelect;

    public event Action OnTimerUpdate;
    public event Action OnTimeOver;
    public event Action OnSelectCharacter;

    private void Start()
    {
        _selectPhase = SelectPhase.First;
        _firstTimer = SelectTime;
        _secondTimer = SelectTime;
        _thirdTimer = SelectTime;
    }

    private void Update()
    {
        switch (_selectPhase)
        {
            case SelectPhase.Wait:
                break;
            case SelectPhase.First:
                Phase(ref _firstTimer, SelectPhase.Second);
                break;
            case SelectPhase.Second:
                Phase(ref _secondTimer, SelectPhase.Third);
                break;
            case SelectPhase.Third:
                Phase(ref _thirdTimer, SelectPhase.End);
                break;
            case SelectPhase.End:
                break;
        }
    }
    private void Phase(ref float timer, SelectPhase nextPhase)
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            OnTimerUpdate?.Invoke();
        }
        else
        {
            OnTimeOver?.Invoke();
            StartCoroutine(SwitchPhase(nextPhase));
        }
    }
    private IEnumerator SwitchPhase(SelectPhase nextPhase)
    {
        _selectPhase = SelectPhase.Wait;
        yield return new WaitForSeconds(DelayTime);
        _selectPhase = nextPhase;
    }

    [PunRPC]
    public void SelectCharacter(int index)
    {
        if (_isSelectCharacter[index]) return;

        // ToDo:캐릭터 생성(인게임 캐릭터 오브젝트)
        _isSelect = true;

        OnSelectCharacter?.Invoke();
    }
}
