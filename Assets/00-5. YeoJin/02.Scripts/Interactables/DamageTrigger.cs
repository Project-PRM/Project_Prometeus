using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    public CharacterBase owner;

    private void OnTriggerEnter(Collider other)
    {
        if (!gameObject.activeSelf) return;

        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null && target != owner)
        {
            Debug.Log($"Hit {target}, damage {owner.BaseStats.BaseDamage}");
            target.TakeDamage(owner.BaseStats.BaseDamage);
        }
    }
}