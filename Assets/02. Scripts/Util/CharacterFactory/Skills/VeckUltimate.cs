using UnityEngine;

public class VeckUltimate : ISkillNoTarget
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

    public void Activate()
    {
        if (_timer < Data.Cooltime)
        {
            Debug.Log($"{Character.Name} Skill is on cooldown.");
            return;
        }

        Debug.Log($"{Character.Name} activated VeckUltimate.");

        CharacterController controller = Character.Behaviour.GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogWarning("CharacterController not found on character.");
            return;
        }


        _timer = 0f;
    }

    private void ActivateUltimate()
    {
        GameObject shield = FindShieldInFront();
        if (shield == null) return;

        var veckShield = shield.GetComponent<VeckSkillShield>();
        if (veckShield == null) return;

        veckShield.OnUltimateActivate();
    }

    private GameObject FindShieldInFront()
    {
        Vector3 origin = Character.Behaviour.transform.position + Vector3.up * 1f;
        Vector3 direction = Character.Behaviour.transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, 2f))
        {
            if (hit.collider.CompareTag("VeckShield"))
            {
                return hit.collider.gameObject;
            }
        }

        return null;
    }
}
