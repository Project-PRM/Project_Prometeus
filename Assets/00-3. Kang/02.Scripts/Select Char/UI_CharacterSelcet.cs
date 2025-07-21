using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UI_CharacterSample
{
    public GameObject CharacterVisual;
    public Color CharacterColor;
}

[RequireComponent(typeof(PhotonView))]
public class UI_CharacterSelcet : MonoBehaviourPunCallbacks
{
    private PhotonView _photonView;

    [SerializeField] private UI_CharacterSample[] _UI_characterSamples;
    [SerializeField] private Image[] _playerTimerBars;
    [SerializeField] private Image[] _playerProfiles;

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
        CharacterSelect.Instance.OnTimerUpdate += TimerUIUpdate;
        CharacterSelect.Instance.OnTimeOver += AutoRandomSelectCharacter;
    }

    private void TimerUIUpdate()
    {
        _playerTimerBars[0].fillAmount = CharacterSelect.Instance.FirstTimer / CharacterSelect.Instance.SelectTime;
        _playerTimerBars[1].fillAmount = CharacterSelect.Instance.SecondTimer / CharacterSelect.Instance.SelectTime;
        _playerTimerBars[2].fillAmount = CharacterSelect.Instance.ThirdTimer / CharacterSelect.Instance.SelectTime;
    }

    public void BT_SelectCharacter(int index)
    {
        if (CharacterSelect.Instance.IsSelect) return;
        if (CharacterSelect.Instance.SelectPhase != CharacterSelect.Instance.MyPhase) return;
        if (CharacterSelect.Instance.CharacterSamples[index].IsCharacterSelect) return;

        _photonView.RPC(nameof(CharacterSelect.SelectCharacter), RpcTarget.All, index);
        _photonView.RPC(nameof(SelectCharacter), RpcTarget.All, index);
    }

    
    private void AutoRandomSelectCharacter()
    {
        if (CharacterSelect.Instance.IsSelect) return;
        if (CharacterSelect.Instance.SelectPhase != CharacterSelect.Instance.MyPhase) return;

        while (!CharacterSelect.Instance.IsSelect)
        {
            int randomIndex = UnityEngine.Random.Range(0, _UI_characterSamples.Length);

            if (CharacterSelect.Instance.CharacterSamples[randomIndex].IsCharacterSelect) continue;

            _photonView.RPC(nameof(CharacterSelect.SelectCharacter), RpcTarget.All, randomIndex);
            _photonView.RPC(nameof(SelectCharacter), RpcTarget.All, randomIndex);
        }
    }

    [PunRPC]
    private void SelectCharacter(int index)
    {
        Image myCharacterImage = GetCharacterImage();
        myCharacterImage.color = _UI_characterSamples[index].CharacterColor;
        _UI_characterSamples[index].CharacterVisual.SetActive(true);
    }
    private Image GetCharacterImage()
    {
        switch (CharacterSelect.Instance.SelectPhase)
        {
            case SelectPhase.First:
                return _playerProfiles[0];
            case SelectPhase.Second:
                return _playerProfiles[1];
            case SelectPhase.Third:
                return _playerProfiles[2];
            default:
                return null;
        }
    }
}
