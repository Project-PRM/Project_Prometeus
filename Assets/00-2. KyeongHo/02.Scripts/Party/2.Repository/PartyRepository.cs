using Firebase.Firestore;
using System.Threading.Tasks;

public class PartyRepository
{
    private readonly FirebaseFirestore _firestore;
    private const string PARTY_FIELDNAME = "Parties";
    private const string COLLECTION_NAME = "UserAccount";

    public PartyRepository(FirebaseFirestore firestore)
    {
        _firestore = firestore;
    }

    public async Task<string> CreatePartyAsync(PartyDTO partyDto)
    {
        DocumentReference docRef = _firestore.Collection(PARTY_FIELDNAME).Document();
        await docRef.SetAsync(partyDto);
        return docRef.Id;
    }

    public async Task<PartyDTO> GetPartyAsync(string partyId)
    {
        DocumentReference docRef = _firestore.Collection(PARTY_FIELDNAME).Document(partyId);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
        return snapshot.Exists ? snapshot.ConvertTo<PartyDTO>() : null;
    }

    public async Task UpdatePartyAsync(string partyId, PartyDTO partyDto)
    {
        await _firestore.Collection(PARTY_FIELDNAME).Document(partyId).SetAsync(partyDto, SetOptions.MergeAll);
    }

    public async Task DeletePartyAsync(string partyId)
    {
        await _firestore.Collection(PARTY_FIELDNAME).Document(partyId).DeleteAsync();
    }

    public async Task SendPartyInvitationAsync(string inviteeUid, string partyId, string inviterUid)
    {
        DocumentReference docRef = _firestore.Collection(COLLECTION_NAME).Document(inviteeUid);
        var invitation = new PartyInvitationDTO { PartyId = partyId, InviterUid = inviterUid };
        await docRef.UpdateAsync("PartyInvitations."+partyId, invitation);
    }

    public async Task RemovePartyInvitationAsync(string inviteeUid, string partyId)
    {
        DocumentReference docRef = _firestore.Collection(COLLECTION_NAME).Document(inviteeUid);
        await docRef.UpdateAsync("PartyInvitations."+partyId, FieldValue.Delete);
    }
}
