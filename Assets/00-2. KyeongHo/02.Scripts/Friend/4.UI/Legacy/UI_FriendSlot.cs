using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_FriendSlot : MonoBehaviour
{
    public Image ProfileIcon;
    public TextMeshProUGUI FriendNicknameText;
    public TextMeshProUGUI FriendStatusText;
    
    public void Refresh(string nickname)
    {
        // TODO: Icon과 접속상태(접속상태는 필드 추가해야함)
        FriendNicknameText.text = nickname;
    }

}
