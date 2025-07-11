using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CharacterRepository
{
    public CharacterStats BaseStats { get; private set; }

    // 1. 캐릭터 베이스 스탯 불러오기
    public async Task GetBaseCharacterStats()
    {
        await FirebaseInitialize.WaitForInitializationAsync();

        var docRef = FirebaseInitialize.DB.Collection("DefaultCharacterStats").Document("Common");
        var snapshot = await docRef.GetSnapshotAsync();
        if (!snapshot.Exists)
        {
            throw new InvalidOperationException("기본 캐릭터 스탯이 존재하지 않습니다.");
        }
        BaseStats = snapshot.ConvertTo<CharacterStats>();
    }

    // 2. 캐릭터 스킬들 이름 불러오기
    public async Task<CharacterSkillNameData> GetCharacterSkillNames(ECharacterName name)
    {
        await FirebaseInitialize.WaitForInitializationAsync();

        var docRef = FirebaseInitialize.DB.Collection("PlayableCharacters").Document(name.ToString());
        var snapshot = await docRef.GetSnapshotAsync();
        if(!snapshot.Exists)
        {
            throw new InvalidOperationException($"캐릭터 {name}의 스킬 정보가 존재하지 않습니다.");
        }
        return snapshot.ConvertTo<CharacterSkillNameData>();
    }

    // 3. 스킬 스탯 데이터 불러오기
    public async Task<Dictionary<string, SkillData>> GetAllSkillDatas()
    {
        await FirebaseInitialize.WaitForInitializationAsync();

        var collectionRef = FirebaseInitialize.DB.Collection("SkillDatas");
        var snapshot = await collectionRef.GetSnapshotAsync();

        var result = new Dictionary<string, SkillData>();

        foreach (var doc in snapshot.Documents)
        {
            var data = doc.ConvertTo<SkillData>();
            result[doc.Id] = data;
        }

        return result;
    }
}
