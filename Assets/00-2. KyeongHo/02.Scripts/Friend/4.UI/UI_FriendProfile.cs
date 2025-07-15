using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_FriendProfile : MonoBehaviour
{
    public Image ProfileIcon;
    public TextMeshProUGUI NicknameText;
    
    public void Refresh()
    {
        // TODO: Icon....
        NicknameText.text = AccountManager.Instance.MyAccount.Nickname;
    }
    private void Start()
    {
        Refresh();
    }
}
