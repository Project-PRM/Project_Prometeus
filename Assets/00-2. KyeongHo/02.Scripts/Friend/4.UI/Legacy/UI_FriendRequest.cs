using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_FriendRequest : MonoBehaviour
{
    public Image ProfileIcon;
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
