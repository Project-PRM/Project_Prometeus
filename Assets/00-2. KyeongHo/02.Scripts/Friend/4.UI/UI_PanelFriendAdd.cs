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
            item.GetComponent<UI_FriendRequest>().NicknameText.text = await AccountManager.Instance.GetUserNicknameWithUid(request.SenderUid);
        }
    }
    public async void OnClickFriendSearchButton()
    {
        string inputNickname = NicknameInputField.text;
        if (string.IsNullOrEmpty(inputNickname))
            return;

        // 기존 항목 삭제
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // 닉네임으로 UID 리스트 가져오기
        List<string> uids = await AccountManager.Instance.GetUidsWithNickname(inputNickname);
        if (uids.Count == 0)
            return;

        // 각 UID에 대해 항목 생성
        foreach (string uid in uids)
        {
            GameObject item = Instantiate(requestItemPrefab, contentParent);
            var panel = item.GetComponent<UI_PanelFriendUser>();
            panel.Refresh(inputNickname, uid);
        }
    }
}
