public class PartyInvitation
{
    public string PartyId { get; private set; }
    public string InviterUid { get; private set; }
    public string InviteeUid { get; private set; }

    public PartyInvitation(string partyId, string inviterUid, string inviteeUid)
    {
        PartyId = partyId;
        InviterUid = inviterUid;
        InviteeUid = inviteeUid;
    }
}
