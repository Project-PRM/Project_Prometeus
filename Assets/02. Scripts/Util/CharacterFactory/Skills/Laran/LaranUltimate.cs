using Photon.Pun;
using UnityEngine;

public class LaranUltimate : ITargetableSkill
{
    private float _timer = 0f;
    public SkillData Data { get; set; }
    public CharacterBase Character { get; set; }
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
