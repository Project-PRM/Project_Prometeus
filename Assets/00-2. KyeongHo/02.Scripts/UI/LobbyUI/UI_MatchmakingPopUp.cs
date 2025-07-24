using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MatchmakingPopUp : UI_PopUp
{
    [Header("Matchmaking UI")]
    public TextMeshProUGUI RoomPlayerCountText;
    public TextMeshProUGUI SystemMessageText;
    public Button MatchingStartButton;
    
    void Start()
    {
        EventManager.AddListener<GameStartEvent>(Refresh);
    }
    public void Refresh(GameStartEvent evt = null)
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            RoomPlayerCountText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
        }
    }

    // 매칭 시작 버튼
    public void OnClickMatchingStartButton()
    {
        RoomPlayerCountText.gameObject.SetActive(true);
    
        Debug.Log("[Matching] 매칭 시작");
        
        // 현재 상태 디버그 로그
        string partyName = PartyManager.Instance.GetCurrentPartyName();
        bool isPartyLeader = PartyManager.Instance.IsPartyLeader();
        
        Debug.Log($"[Debug] 현재 파티: {partyName}, 파티 리더: {isPartyLeader}");
    
        // 파티에 속해있는지 확인
        if (!string.IsNullOrEmpty(partyName))
        {
            // 파티에 속해있을 때
            if (isPartyLeader)
            {
                Debug.Log("[파티 매칭] 파티 리더로 매칭 시작");
                PhotonServerManager.Instance.StartPartyMatchmaking();
            }
            else
            {
                Debug.Log("[파티 매칭] 파티 리더가 아님 - 매칭 불가");
                ShowSystemMessage("파티 리더만 매칭을 시작할 수 있습니다.");
            }
        }
        else
        {
            // 파티에 속해있지 않을 때 - 솔로 매칭
            Debug.Log("[솔로 매칭] 개인 매칭 시작");
            PhotonServerManager.Instance.JoinRandomRoom();
        }
    }

    // 매칭 버튼 상태 업데이트
    public void UpdateMatchingButtonState()
    {
        if (MatchingStartButton == null) return;

        bool isInParty = !string.IsNullOrEmpty(PartyManager.Instance.GetCurrentPartyName());
        bool isPartyLeader = PartyManager.Instance.IsPartyLeader();

        MatchingStartButton.interactable = !isInParty || isPartyLeader;
        
        Debug.Log($"[ButtonState] 파티: {isInParty}, 리더: {isPartyLeader}, 버튼 활성화: {MatchingStartButton.interactable}");
    }
   
    public void ShowSystemMessage(string msg)
    {
        Debug.Log($"[SystemMessage] {msg}");
        if (SystemMessageText != null)
        {
            SystemMessageText.gameObject.SetActive(true);
            SystemMessageText.text = msg;
            StartCoroutine(HideSystemMessage());
        }
    }

    private IEnumerator HideSystemMessage()
    {
        yield return new WaitForSeconds(3f);
        if (SystemMessageText != null)
        {
            SystemMessageText.gameObject.SetActive(false);
        }
    }

    public override void Show()
    {
        base.Show();
        UpdateMatchingButtonState();
    }
    public override void Hide()
    {
        base.Hide();
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();    
        }
    }
}