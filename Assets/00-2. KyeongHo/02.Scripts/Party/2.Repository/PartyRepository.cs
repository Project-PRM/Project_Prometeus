using Firebase.Firestore;
using System.Threading.Tasks;
using Party.Domain;

namespace Party.Repository
{
    public class PartyRepository
    {
        private readonly FirebaseFirestore _firestore;
        private const string PartiesCollection = "Parties";
        private const string UserAccountsCollection = "UserAccount";

        public PartyRepository(FirebaseFirestore firestore)
        {
            _firestore = firestore;
        }

        public async Task<string> CreatePartyAsync(PartyDTO partyDto)
        {
            DocumentReference docRef = _firestore.Collection(PartiesCollection).Document();
            await docRef.SetAsync(partyDto);
            return docRef.Id;
        }

        public async Task<PartyDTO> GetPartyAsync(string partyId)
        {
            DocumentReference docRef = _firestore.Collection(PartiesCollection).Document(partyId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<PartyDTO>() : null;
        }

        public async Task UpdatePartyAsync(string partyId, PartyDTO partyDto)
        {
            await _firestore.Collection(PartiesCollection).Document(partyId).SetAsync(partyDto, SetOptions.MergeAll);
        }

        public async Task DeletePartyAsync(string partyId)
        {
            await _firestore.Collection(PartiesCollection).Document(partyId).DeleteAsync();
        }

        public async Task SendPartyInvitationAsync(string inviteeUid, string partyId, string inviterUid)
        {
            DocumentReference docRef = _firestore.Collection(UserAccountsCollection).Document(inviteeUid);
            var invitation = new PartyInvitationDTO { PartyId = partyId, InviterUid = inviterUid };
            await docRef.UpdateAsync("PartyInvitations."+partyId, invitation);
        }

        public async Task RemovePartyInvitationAsync(string inviteeUid, string partyId)
        {
            DocumentReference docRef = _firestore.Collection(UserAccountsCollection).Document(inviteeUid);
            await docRef.UpdateAsync("PartyInvitations."+partyId, FieldValue.Delete);
        }
    }
}
