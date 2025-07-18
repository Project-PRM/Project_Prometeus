using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UI_FriendList : UI_PopUp
{
    public Transform contentParent;
    public GameObject friendItemPrefab;

    private async void Start()
    {
        await LoadFriendList();
    }

    private async Task LoadFriendList()
    {
        string myUid = AccountManager.Instance.MyAccount.UserId;

        List<string> friendUids = await FriendManager.Instance.GetFriendUids(myUid);

        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (string uid in friendUids)
        {
            GameObject item = Instantiate(friendItemPrefab, contentParent);
            item.GetComponent<UI_PanelFriendSlot>().Refresh(await AccountManager.Instance.GetUserNicknameWithUid(uid));
            item.GetComponentInChildren<TMP_Text>().text = uid;
        }
    }
}