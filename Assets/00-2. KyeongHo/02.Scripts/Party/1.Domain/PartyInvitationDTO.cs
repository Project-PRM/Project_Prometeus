using Firebase.Firestore;

[FirestoreData]
public class PartyInvitationDTO
{
    [FirestoreProperty] public string PartyId { get; set; }
    [FirestoreProperty] public string InviterUid { get; set; }
}
