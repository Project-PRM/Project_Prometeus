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
    public void OnRequestFriendSendButtonClicked()
    {
        FriendManager.Instance.SendFriendRequest(AccountManager.Instance.MyAccount.UserId, NicknameText.text);
    }
}
