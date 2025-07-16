using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;
using TMPro;

public class CharacterSetter : MonoBehaviour
{
    // 캐릭터를 고를 경우 setcustomproperty
    [SerializeField] private TMP_Dropdown _characterDropdown;

    private void Start()
    {
        // 기존 선택값 유지하거나 초기화할 수 있음
        _characterDropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    private void OnDropdownChanged(int index)
    {
        ECharacterName selected = (ECharacterName)index;

        Hashtable props = new Hashtable
        {
            { "character", (int)selected }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        Debug.Log($"[CharacterSetter] Character set to {selected}");
    }

    // ui 업데이트
}
