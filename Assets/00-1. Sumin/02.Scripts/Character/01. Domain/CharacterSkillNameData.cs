using Firebase.Firestore;

[FirestoreData]
public class CharacterSkillNameData
{
    [FirestoreProperty] public string Name { get; set; }
    [FirestoreProperty] public string Passive { get; set; }
    [FirestoreProperty] public string Skill { get; set; }
    [FirestoreProperty] public string Ultimate { get; set; }

    public CharacterSkillNameData() { } // Firebase용
}