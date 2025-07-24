using Photon.Pun;
using UnityEngine;

public class FulfunsUltimate : ITargetableSkill
{
    private float _timer = 0f;
    public SkillData Data { get; set; }

    public void Update() => _timer += Time.deltaTime;

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

        Vector3 origin = character.Behaviour.transform.position + character.Behaviour.transform.forward * 1.5f + Vector3.up;
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
        projectile.GetComponent<IProjectile>().SetData(Data, character, dir);

        _timer = 0f;
    }
}
