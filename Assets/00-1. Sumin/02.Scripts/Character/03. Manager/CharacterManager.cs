using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CharacterManager : Singleton<CharacterManager>
{
    private CharacterRepository _repository = new CharacterRepository();

    public CharacterStats CharacterStats => _repository.BaseStats;

    private Dictionary<ECharacterName, CharacterSkillNameData> _characterMetaDataCache = new();
    private Dictionary<string, SkillData> _skillDataMap;
    public Dictionary<string, SkillData> GetSkillDataMap() => _skillDataMap;

    public bool IsInitialized { get; private set; }

    protected async override void Awake()
    {
        /*base.Awake();
        await _repository.GetBaseCharacterStats();
        await Init();*/
    }

    public async Task Init()
    {
        if (IsInitialized)
        {
            return;
        }
        _skillDataMap = await _repository.GetAllSkillDatas();
        SkillFactory.LoadSkillData(_skillDataMap);

        foreach (ECharacterName characterName in System.Enum.GetValues(typeof(ECharacterName)))
        {
            _characterMetaDataCache[characterName] = await _repository.GetCharacterSkillNames(characterName);
        }

        IsInitialized = true;
    }

    public async Task<CharacterSkillNameData> GetCharacterMetaDataAsync(ECharacterName characterName)
    {
        if (_characterMetaDataCache.TryGetValue(characterName, out var meta))
            return meta;

        // 캐시에 없으면 비동기 로딩
        var newMeta = await _repository.GetCharacterSkillNames(characterName);
        _characterMetaDataCache[characterName] = newMeta;
        return newMeta;
    }
}
