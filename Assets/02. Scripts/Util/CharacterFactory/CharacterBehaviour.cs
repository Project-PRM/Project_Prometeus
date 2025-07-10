using UnityEngine;

public class CharacterBehaviour : MonoBehaviour
{
    [SerializeField] private ECharacterName _characterName;

    private CharacterBase _character;

    private void Awake()
    {
        // 예시: 나중엔 Firebase나 Json에서 가져와야 함
        /*_character = CharacterFactory.CreateCharacter(
            _characterName.ToString(), "MagicBolt", "HealOnKill", "FoxFire", "DashThrough"
        );*/
        _character = new CharacterBase(
            this,
            _characterName.ToString(),
            SkillFactory.Create("MagicBolt"),
            SkillFactory.Create("HealOnKill"),
            SkillFactory.Create("FoxFire"),
            SkillFactory.Create("DashThrough")
        );
    }

    // 이게 최종적으로 쓸 함수임
    /*private async void Start()
    {
        // ECharacterName → string 변환
        string name = _characterName.ToString();

        // Firebase에서 캐릭터 데이터 불러오기
        CharacterData data = await FirebaseManager.GetCharacterData(name);

        _character = CharacterFactory.CreateCharacter(
            data.Name,
            data.Basic,
            data.Passive,
            data.Skill,
            data.Ultimate
        );
    }*/

    // 스킬 사용만 알아서 추가
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            _character.UseSkill(ESkillType.BasicAttack);

        if (Input.GetKeyDown(KeyCode.W))
            _character.UseSkill(ESkillType.Skill);
    }
}
