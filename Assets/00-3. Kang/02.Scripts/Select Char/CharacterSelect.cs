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

[Serializable]
public class CharacterSample
{
    public GameObject CharacterPrefab;
    public bool IsCharacterSelect = false;
}

[RequireComponent(typeof(PhotonView))]
public class CharacterSelect : PunSingleton<CharacterSelect>
{
    public float SelectTime = 5f;
    public float DelayTime = 0.5f;
    public CharacterSample[] CharacterSamples;

    public SelectPhase SelectPhase { get; private set; }
    public SelectPhase MyPhase { get; private set; }
    public float FirstTimer { get; private set; }
    public float SecondTimer { get; private set; }
    public float ThirdTimer { get; private set; }
    public bool IsSelect { get; private set; }

    public event Action OnTimerUpdate;
    public event Action OnTimeOver;

    private PhotonView _photonView;

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
        SelectPhase = SelectPhase.First;
        FirstTimer = SelectTime;
        SecondTimer = SelectTime;
        ThirdTimer = SelectTime;

        foreach (var item in CharacterSamples) { item.CharacterPrefab.SetActive(false); }
    }

    private void Update()
    {
        switch (SelectPhase)
        {
            case SelectPhase.Wait:
                break;
            case SelectPhase.First:
                _photonView.RPC(nameof(PhaseTimer), RpcTarget.All, FirstTimer, SelectPhase.Second);
                break;
            case SelectPhase.Second:
                _photonView.RPC(nameof(PhaseTimer), RpcTarget.All, SecondTimer, SelectPhase.Third);
                break;
            case SelectPhase.Third:
                _photonView.RPC(nameof(PhaseTimer), RpcTarget.All, ThirdTimer, SelectPhase.End);
                break;
            case SelectPhase.End:
                break;
        }
    }

    public void SetMyPhase(SelectPhase phase)
    {
        MyPhase = phase;
    }

    private void PhaseTimer(float timer, SelectPhase nextPhase)
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
        SelectPhase = SelectPhase.Wait;
        yield return new WaitForSeconds(DelayTime);
        SelectPhase = nextPhase;
    }

    [PunRPC]
    public void SelectCharacter(int index)
    {
        CharacterSamples[index].IsCharacterSelect = true;

        if(_photonView.IsMine)
        {
            IsSelect = true;
        }

        if(PhotonNetwork.IsMasterClient)
        {
            // ToDo:캐릭터 생성(인게임 캐릭터 오브젝트)
        }
    }
}
