using Photon.Pun;
using UnityEngine;

public class LaranUltimate : ITargetableSkill
{
    private float _timer = 0f;
    public SkillData Data { get; set; }
    public CharacterBase Character { get; set; }

    /* 궁극기 : 불 대충 던지기
    - 코스트 - 70 / 쿨타임 40초
    - 속성 1 :  포물선 투사체 공격
    - 속성 2 : 1차 폭파
        - 작은 넉백 / 데미지 - 5
    - 속성 3 : 2차 화염
        - 원형 범위 내의 적에게 화상 상태 부여
        - 화상 : 도트 데미지 1초당 - 5
        - 지속 시간 : 6초 */
    public void SetOwner(CharacterBase character)
    {
        Character = character;
    }

    public void Update() => _timer += Time.deltaTime;

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

        Vector3 origin = Character.Behaviour.transform.position + Character.Behaviour.transform.forward * 1.5f + Vector3.up;
        target.y = 1.5f;
        Vector3 dir = (target - origin).normalized;
        Quaternion rotation = Quaternion.LookRotation(dir);

        GameObject prefab = Resources.Load<GameObject>("Projectiles/" + Data.ProjectilePrefabName);
        if (prefab == null)
        {
            Debug.LogError($"Projectile prefab {Data.ProjectilePrefabName} not found.");
            return;
        }

        GameObject projectile = PhotonNetwork.Instantiate($"Projectiles/{Data.ProjectilePrefabName}", origin, rotation);
        projectile.GetComponent<IProjectile>().SetData(Data, Character, dir);

        _timer = 0f;
    }
}
