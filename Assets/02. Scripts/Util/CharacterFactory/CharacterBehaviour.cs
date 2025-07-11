using UnityEngine;
using UnityEngine.Rendering;

public class CharacterBehaviour : MonoBehaviour
{
    [SerializeField] private ECharacterName _characterName;

    private CharacterBase _character;

    private bool _isInitialized = false;

    // 캐릭터 스킬 조합
    private async void Start()
    {
        if (!CharacterManager.Instance.IsInitialized) 
        {
            await CharacterManager.Instance.Init();
        }
        var skills = await CharacterManager.Instance.GetCharacterMetaDataAsync(_characterName);

        _character = new CharacterBase(
            this,
            _characterName.ToString(),
            SkillFactory.Create(ESkillType.BasicAttack.ToString()),
            SkillFactory.Create(skills.Passive),
            SkillFactory.Create(skills.Skill),
            SkillFactory.Create(skills.Ultimate),
            CharacterManager.Instance.CharacterStats
        );

        _isInitialized = true;
    }
    
    private void Update()
    {
        if(_isInitialized == false)
        {
            return;
        }
        //_character.Update();

        if (Input.GetMouseButtonDown(0))
        {
            _character.UseSkill(ESkillType.BasicAttack);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _character.UseSkill(ESkillType.Skill);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _character.UseSkill(ESkillType.Ultimate);
        }
    }
}
