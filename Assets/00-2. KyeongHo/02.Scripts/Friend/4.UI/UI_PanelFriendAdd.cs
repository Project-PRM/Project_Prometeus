using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UI_PanelFriendAdd : UI_PopUp
{
    public Transform contentParent;
    public GameObject requestItemPrefab;
    public TMP_InputField NicknameInputField;

    private async void Start()
    {
        await LoadFriendRequestList();
    }

    private async Task LoadFriendRequestList()
    {
        string myUid = AccountManager.Instance.MyAccount.UserId;

        List<FriendRequest> requests = await FriendManager.Instance.GetFriendRequests(myUid);

        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var request in requests)
        {
            GameObject item = Instantiate(requestItemPrefab, contentParent);
            // TODO : Icon....
            item.GetComponent<UI_FriendRequest>().NicknameText.text = await AccountManager.Instance.GetUserNicknameWithUID(request.SenderUid);
        }
    }
    public void OnClickFriendSearchButton()
    {
        FriendManager.Instance.GetFriendRequests(NicknameInputField.text);
    }
}
