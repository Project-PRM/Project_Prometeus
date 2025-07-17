using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    public CharacterBase Owner;
    public Collider AttackRange;
    private HashSet<IDamageable> _damagedThisActivation = new();

    public void TurnOnOrOff(bool check)
    {
        AttackRange.enabled = check;
        _damagedThisActivation.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"collided with {other.gameObject.name}");
        IDamageable target = other.gameObject.GetComponent<IDamageable>();
        if (target != null && target != Owner && !_damagedThisActivation.Contains(target))
        {
            Debug.Log($"Hit {target}, damage {Owner.BaseStats.BaseDamage}");
            target.TakeDamage(Owner.BaseStats.BaseDamage);
            _damagedThisActivation.Add(target);
        }
    }
}