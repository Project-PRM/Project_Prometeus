using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RequestFriendslot : MonoBehaviour
{
    public TextMeshProUGUI SenderNickname;
    public Button AcceptButton;
    public Button DenyButton;
    private string _uid;
    
    public void Refresh(string nickname, string uid)
    {
        SenderNickname.text = nickname;
        _uid = uid;
    }

    public async void OnClickAccept()
    {
        await FriendManager.Instance.AcceptFriendRequest(AccountManager.Instance.MyAccount.UserId, _uid);
        Destroy(gameObject);
    }
    public async void OnClickDecline()
    {
        await FriendManager.Instance.DeclineFriendRequest(AccountManager.Instance.MyAccount.UserId, _uid);
        Destroy(gameObject);
    }
}
