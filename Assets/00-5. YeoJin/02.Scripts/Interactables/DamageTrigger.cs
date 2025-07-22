using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    public CharacterBehaviour Owner;
    private CharacterBase _base;
    private string _curTeam;
    public Collider AttackRange;

    private HashSet<IDamageable> _damagedThisActivation = new();
    private Dictionary<GameObject, CharacterBase> _cachedTargets = new();

    private bool _isInitialized = false;

    public void TurnOnOrOff(bool check)
    {
        if (!_isInitialized)
        {
            _base = Owner.GetCharacterBase();
            _curTeam = _base.Team;
            Debug.Log($"current team of {Owner.gameObject} is {_curTeam}");
            _isInitialized = true;
        }
        AttackRange.enabled = check;
        _damagedThisActivation.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"collided with {other.gameObject.name}");
        IDamageable target = other.gameObject.GetComponent<IDamageable>();

        /*if(other.TryGetComponent<CharacterBehaviour>(out var behaviour))
        {
            CharacterBase otherchar = behaviour.GetCharacterBase();
            if (otherchar.Team == _curTeam)
            {
                Debug.Log(" is the same team");
                return; // 같은 팀이면 무시
            }
        }*/

        if (target != null && target != (IDamageable)Owner && !_damagedThisActivation.Contains(target))
        {
            Debug.Log($"Hit {target}, damage {_base.BaseStats.BaseDamage}");
            target.TakeDamage(_base.BaseStats.BaseDamage);
            _damagedThisActivation.Add(target);
        }
    }
}