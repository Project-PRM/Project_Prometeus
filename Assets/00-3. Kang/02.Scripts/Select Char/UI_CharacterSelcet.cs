using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterSelcet : MonoBehaviourPunCallbacks
{
    [SerializeField] private Image _firstTimerBar;
    [SerializeField] private Image _secondTimerBar;
    [SerializeField] private Image _thirdTimerBar;

    [SerializeField] private Image _firstCharacterImage;
    [SerializeField] private Image _secondCharacterImage;
    [SerializeField] private Image _thirdCharacterImage;

    [SerializeField] private GameObject[] _character;

    //지금은 컬러로 해놧는데 나중에 캐릭터별 이미지로 바꿔야함
    [SerializeField] private Color[] _characterImage;

    private void Start()
    {
        CharacterSelect.Instance.OnTimerUpdate += TimerUIUpdate;
        CharacterSelect.Instance.OnTimeOver += AutoRandomSelectCharacter;
        foreach (var item in _character) { item.SetActive(false); }
    }

    private void TimerUIUpdate()
    {
        _firstTimerBar.fillAmount = CharacterSelect.Instance.FirstTimer / CharacterSelect.Instance.SelectTime;
        _secondTimerBar.fillAmount = CharacterSelect.Instance.SecondTimer / CharacterSelect.Instance.SelectTime;
        _thirdTimerBar.fillAmount = CharacterSelect.Instance.ThirdTimer / CharacterSelect.Instance.SelectTime;
    }

    public void BT_SelectCharacter(int index)
    {
        if (CharacterSelect.Instance.IsSelect
        || CharacterSelect.Instance.MyPhase != CharacterSelect.Instance.SelectPhase) return;

        CharacterSelect.Instance.SelectCharacter(index);

        SelectCharacterRPC(index);
    }


    [PunRPC]
    private void SelectCharacterRPC(int index)
    {
        Image myCharacterImage = null;

        switch (CharacterSelect.Instance.MyPhase)
        {
            case SelectPhase.First:
                myCharacterImage = _firstCharacterImage;
                break;
            case SelectPhase.Second:
                myCharacterImage = _secondCharacterImage;
                break;
            case SelectPhase.Third:
                myCharacterImage = _thirdCharacterImage;
                break;
        }

        switch (CharacterSelect.Instance.MyPhase)
        {
            case SelectPhase.First:
                myCharacterImage.color = _characterImage[index];
                break;
            case SelectPhase.Second:
                myCharacterImage.color = _characterImage[index];
                break;
            case SelectPhase.Third:
                myCharacterImage.color = _characterImage[index];
                break;
        }

        foreach (var item in _character) { item.SetActive(false); }
        _character[index].SetActive(true);
        CharacterSelect.Instance.IsSelectCharacter[index] = true;
    }

    private void AutoRandomSelectCharacter()
    {
        if (CharacterSelect.Instance.MyPhase != CharacterSelect.Instance.SelectPhase) return;

        while (!CharacterSelect.Instance.IsSelect)
        {
            int randomIndex = Random.Range(0, _character.Length);

            if (CharacterSelect.Instance.IsSelectCharacter[randomIndex]) continue;

            BT_SelectCharacter(randomIndex);
        }
    }
}
