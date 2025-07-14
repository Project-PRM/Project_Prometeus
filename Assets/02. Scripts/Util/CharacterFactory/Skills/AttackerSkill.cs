using Photon.Pun;
using UnityEngine;
using UnityEngine.TextCore.Text;

/// <summary>
/// 투사체 발사
/// </summary>
public class AttackerSkill : ITargetableSkill
{
    private float _timer = 0f;

    public SkillData Data { get; set; }

    public void Update()
    {
        _timer += Time.deltaTime;
    }

    public void Activate(CharacterBase character, Vector3 target)
    {
        if (_timer < Data.Cooltime)
        {
            Debug.Log($"{character.Name} Skill is on cooldown.");
            return;
        }

        // 마우스 방향 계산
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 dir = (mousePos - character.Behaviour.transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        // 발사
        GameObject projectile = PhotonNetwork.Instantiate(
            Data.ProjectilePrefabName,
            character.Behaviour.transform.position,
            rotation,
            0
        );

        projectile.GetComponent<IProjectile>().SetData(Data, character);
        _timer = 0f;
    }
}