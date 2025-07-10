using Firebase.Firestore;

[FirestoreData]
public class CharacterMetaData
{
    [FirestoreProperty] public string Name { get; set; }
    [FirestoreProperty] public string Basic { get; set; }
    [FirestoreProperty] public string Passive { get; set; }
    [FirestoreProperty] public string Skill { get; set; }
    [FirestoreProperty] public string Ultimate { get; set; }
}