using TMPro;
using UnityEngine;

public class UI_PanelFriendUser : MonoBehaviour
{
    public TextMeshProUGUI NicknameText;
    public TextMeshProUGUI UidText;
    private string _uid;

    
    public void Refresh(string senderNickname, string uid)
    {
        NicknameText.text = senderNickname;
        UidText.text = uid;
        _uid = uid;
    }
    // TODO : 친구 목록은 검색하면 잘 뜨니 Panel 클릭하면 친구추가 요청 보내는거 구현
    public async void OnRequestFriendSendButtonClicked()
    {
        string recipientUid = await AccountManager.Instance.GetUidWithNickname(NicknameText.text);
        await FriendManager.Instance.SendFriendRequest(
            AccountManager.Instance.MyAccount.UserId, recipientUid);
    }
}
