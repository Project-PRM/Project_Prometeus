using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RequestFriendslot : MonoBehaviour
{
    public TextMeshProUGUI SenderNickname;
    public Button AcceptButton;
    public Button DenyButton;

    public void Refresh(string nickname)
    {
        SenderNickname.text = nickname;
    }

    public void OnClickAccept()
    {
        
    }
    public void OnClickDeny()
    {
        
    }
}
