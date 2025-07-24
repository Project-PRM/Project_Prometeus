using Photon.Pun;
using UnityEngine;

public class SpawnerUltimate : ITargetableSkill
{
    private float _timer = 0f;

    public SkillData Data { get; set; }
    public CharacterBase Character { get; set; }
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

    public void Activate(CharacterBase character, Vector3 target)
    {
        if (_timer < Data.Cooltime)
        {
            Debug.Log($"{character.Name} Ultimate is on cooldown.");
            return;
        }

        Debug.Log("SpawnerUltimate activated.");

        // 발사 위치 계산
        Vector3 origin = character.Behaviour.transform.position + character.Behaviour.transform.forward * 1.5f + Vector3.up;
        target.y = 1.5f;
        Vector3 dir = (target - origin).normalized;
        Quaternion rotation = Quaternion.LookRotation(dir);

        // 프리팹 로드
        GameObject prefab = Resources.Load<GameObject>("Projectiles/" + Data.ProjectilePrefabName);
        if (prefab == null)
        {
            Debug.LogError($"Projectile prefab '{Data.ProjectilePrefabName}' not found in Resources/Projectiles.");
            return;
        }

        // 큐브 생성 (Photon)
        GameObject projectile = PhotonNetwork.Instantiate($"Projectiles/{Data.ProjectilePrefabName}", origin, rotation);

        // 데이터 세팅
        projectile.GetComponent<IProjectile>().SetData(Data, character, dir);

        _timer = 0f;
    }
}
