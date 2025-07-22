using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Lobby : MonoBehaviour
{
    public TextMeshProUGUI TeamNameText;
    public TextMeshProUGUI RoomPlayerCountText;
    public TextMeshProUGUI SystemMessageText;
    public Button MatchingStartButton;
     
    private void Start()
    {
        EventManager.AddListener<GameStartEvent>(Refresh);
    }

    public void Refresh(GameStartEvent evt = null)
    {
        if (evt != null)
        {
            TeamNameText.text = $"TeamName : {PhotonNetwork.NickName} / {PhotonNetwork.LocalPlayer.UserId}";  
        }
        
        if (PhotonNetwork.CurrentRoom != null)
        {
            RoomPlayerCountText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
        }
    }

    // 게임 시작 버튼 (방장만 가능)
    public void OnClickGameStartButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonServerManager.Instance.AssignTeams();
        }
        else
        {
            ShowSystemMessage("방장만 게임을 시작할 수 있습니다.");
        }
    }

    // 매칭 시작 버튼
    public void OnClickMatchingStartButton()
    {
        TeamNameText.gameObject.SetActive(true);
        RoomPlayerCountText.gameObject.SetActive(true);
    
        Debug.Log("[Matching] 매칭 시작");
        
        // 현재 상태 디버그 로그
        string partyName = LobbyChatManager.Instance.GetCurrentPartyName();
        bool isPartyLeader = LobbyChatManager.Instance.IsPartyLeader();
        
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

    // 파티 채팅 참여 (Chat1, Chat2 버튼용)
    public void OnClickChatButton(string partyName)
    {
        Debug.Log($"[PartyJoin] 파티 참여 시도: {partyName}");
        PhotonServerManager.Instance.JoinPartyChat(partyName);
        ShowSystemMessage($"파티 '{partyName}' 참여 중...");
    }

    // 매칭 버튼 상태 업데이트
    public void UpdateMatchingButtonState()
    {
        if (MatchingStartButton == null) return;

        bool isInParty = !string.IsNullOrEmpty(LobbyChatManager.Instance.GetCurrentPartyName());
        bool isPartyLeader = LobbyChatManager.Instance.IsPartyLeader();

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
}