using Firebase.Firestore;

namespace Party.Domain
{
    [FirestoreData]
    public class PartyInvitationDTO
    {
        [FirestoreProperty] public string PartyId { get; set; }
        [FirestoreProperty] public string InviterUid { get; set; }
    }
}
