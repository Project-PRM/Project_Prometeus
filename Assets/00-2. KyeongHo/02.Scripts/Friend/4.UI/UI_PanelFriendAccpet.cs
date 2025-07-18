using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UI_PanelFriendAccpet : UI_PopUp
{
    [SerializeField] private Transform content; // Content 오브젝트
    [SerializeField] private GameObject requestFriendPrefab;

    private async void OnEnable()
    {
        await RefreshRequestList();
    }

    private async Task RefreshRequestList()
    {
        // 기존 프리팹 정리
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        string myUid = AccountManager.Instance.MyAccount.UserId;
        List<FriendRequest> requests = await FriendManager.Instance.GetFriendRequests(myUid);

        foreach (var request in requests)
        {
            GameObject go = Instantiate(requestFriendPrefab, content);
            var ui = go.GetComponent<UI_RequestFriendslot>();
            ui.Refresh(request.SenderUid);
        }
    }
}
