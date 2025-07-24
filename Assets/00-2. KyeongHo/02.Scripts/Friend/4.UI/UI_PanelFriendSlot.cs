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
    public async void OnClickFriendInvite()
    {

        string friendUid = await AccountManager.Instance.GetUidWithNickname(FriendName.text);
        string myNickname = AccountManager.Instance.MyAccount.Nickname;
    
        // 친구 초대 전송
        PartyManager.Instance.SendFriendInvite(friendUid, myNickname);
    }
}
