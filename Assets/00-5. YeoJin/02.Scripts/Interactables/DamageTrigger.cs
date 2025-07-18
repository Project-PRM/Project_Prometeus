using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    public CharacterBehaviour Owner;
    private CharacterBase _base;
    public Collider AttackRange;
    private HashSet<IDamageable> _damagedThisActivation = new();
    private bool _isInitialized = false;

    public void TurnOnOrOff(bool check)
    {
        if (!_isInitialized)
        {
            _base = Owner.GetCharacterBase();
        }
        AttackRange.enabled = check;
        _damagedThisActivation.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"collided with {other.gameObject.name}");
        IDamageable target = other.gameObject.GetComponent<IDamageable>();
        if (target != null && target != (IDamageable)Owner && !_damagedThisActivation.Contains(target))
        {
            Debug.Log($"Hit {target}, damage {_base.BaseStats.BaseDamage}");
            target.TakeDamage(_base.BaseStats.BaseDamage);
            _damagedThisActivation.Add(target);
        }
    }
}