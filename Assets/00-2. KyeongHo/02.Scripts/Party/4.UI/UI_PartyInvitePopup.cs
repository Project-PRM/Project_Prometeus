using UnityEngine;
using UnityEngine.UI;
public class UI_PartyInvitePopup : MonoBehaviour
{
    public Text inviterNicknameText;
    public Button acceptButton;
    public Button declineButton;

    private string _partyId;
    private string _inviterUid;

    public void Initialize(string partyId, string inviterUid, string inviterNickname)
    {
        _partyId = partyId;
        _inviterUid = inviterUid;
        inviterNicknameText.text = $"{inviterNickname} has invited you to their party.";

        acceptButton.onClick.AddListener(OnAccept);
        declineButton.onClick.AddListener(OnDecline);
    }

    private void OnAccept()
    {
        PartyManager.Instance.AcceptInvitation(_partyId, _inviterUid);
        Destroy(gameObject);
    }

    private void OnDecline()
    {
        // Optional: Notify the inviter that the invitation was declined
        Destroy(gameObject);
    }
}
