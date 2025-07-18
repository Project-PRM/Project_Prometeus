using TMPro;
using UnityEngine;

public class UI_PanelFriendSlot : MonoBehaviour
{
    public TextMeshProUGUI FriendName;
    public TextMeshProUGUI FriendConnectState;

    public void Refresh(string nickname)
    {
        FriendName.text = nickname;
        FriendConnectState.text = "online"; // TODO : Account랑 firestore에 추후 필드 추가
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
