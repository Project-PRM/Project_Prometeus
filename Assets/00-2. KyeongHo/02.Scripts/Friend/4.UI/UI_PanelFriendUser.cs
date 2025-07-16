using TMPro;
using UnityEngine;

public class UI_PanelFriendUser : MonoBehaviour
{
    public TextMeshProUGUI NicknameText;
    private string _uid;

    public void Refresh(string senderNickname, string uid)
    {
        NicknameText.text = senderNickname;
        _uid = uid;
    }
    public void OnRequestFriendSendButtonClicked()
    {
        FriendManager.Instance.SendFriendRequest(AccountManager.Instance.MyAccount.UserId, NicknameText.text);
    }
}
