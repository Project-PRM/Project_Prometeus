using UnityEngine;

public class UI_FriendPanelButtons : MonoBehaviour
{
    public UI_PanelFriendAdd Panel_FriendAdd;
    public UI_PanelFriendAccpet Panel_FriendAccept;
    
    public void OnClickFriendAddPanelOpenButton()
    {
        Panel_FriendAdd.Show();
    }
    public void OnClickFriendAcceptPanelOpenButton()
    {
        Panel_FriendAccept.Show();
    }
}
