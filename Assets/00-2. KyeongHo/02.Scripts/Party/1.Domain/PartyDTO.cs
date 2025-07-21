using System.Collections.Generic;
using Firebase.Firestore;

[FirestoreData]
public class PartyDTO
{
    [FirestoreProperty] public string LeaderUid { get; set; }
    [FirestoreProperty] public List<string> Members { get; set; }
    [FirestoreProperty] public int MaxSize { get; set; }
    [FirestoreProperty] public string State { get; set; }
}
