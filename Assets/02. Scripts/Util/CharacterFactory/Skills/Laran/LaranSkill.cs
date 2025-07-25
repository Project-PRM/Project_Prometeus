using Photon.Pun;
using UnityEngine;

public class LaranSkill : ITargetableSkill
{
    private float _timer = 0f;

    public SkillData Data { get; set; }
    public CharacterBase Character { get; set; }

    /* 액티브 : 작은 불 대충 던지기
    - 코스트 -10 / 쿨타임 6초
    - 속성 1 : 포물선 투사체 공격 ( 느림 )
    - 속성 2 : 2초간 화염 장판 생성
    - 속성 3 : 화염 장판 피격 시 -8 데미지 ( 0.5초당 1틱 )
    - 속성 4 : 화상
        - 화상 : 도트 데미지 1초당 - 5
        - 지속 시간 3초 */
    public void SetOwner(CharacterBase character)
    {
        Character = character;
    }

    public void Update()
    {
        _timer += Time.deltaTime;
    }

    public GameObject GetIndicatorPrefab()
    {
        return Resources.Load<GameObject>($"Indicators/{Data.IndicatorPrefabName}");
    }

    public void Activate(Vector3 target)
    {
        if (_timer < Data.Cooltime)
        {
            Debug.Log($"{Character.Name} Ultimate is on cooldown.");
            return;
        }

        Debug.Log("created Fulfuns ULTIMATE cube");
        Vector3 origin = Character.Behaviour.transform.position + Character.Behaviour.transform.forward * 1.5f + Vector3.up;
        target.y = 1.5f; // Y축 고정하여 2D 발사 느낌을 주기 위함
        Vector3 dir = (target - origin).normalized;

        Quaternion rotation = Quaternion.LookRotation(dir);

        GameObject prefab = Resources.Load<GameObject>("Projectiles/" + Data.ProjectilePrefabName);

        if (prefab == null)
        {
            Debug.LogError($"프리팹 {Data.ProjectilePrefabName} 을(를) Resources/Projectiles 에서 찾을 수 없습니다.");
            return;
        }

        Debug.Log($"{origin} is where ultimate cube was born");
        GameObject projectile = /*GameObject.*/PhotonNetwork.Instantiate($"Projectiles/{Data.ProjectilePrefabName}", origin, rotation);
        projectile.GetComponent<IProjectile>().SetData(Data, Character, dir);

        _timer = 0f;
    }
}
