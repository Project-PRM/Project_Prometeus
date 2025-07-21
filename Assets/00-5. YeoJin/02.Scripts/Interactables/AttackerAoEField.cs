using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerAoEField : MonoBehaviour
{
    private CharacterBase _owner;
    private IDamageable _damageSelf;

    [SerializeField] private float _duration;
    [SerializeField] private float _tickDamage;
    [SerializeField] private float _tickTime = 0.5f;
    [SerializeField] private int _totalTick = 10;

    private HashSet<IDamageable> _targetsInRange = new HashSet<IDamageable>();

    public void StartAoEField(CharacterBase character, float duration, float tickDamage)
    {
        _owner = character;
        _duration = duration;
        _tickDamage = tickDamage;

        _damageSelf = _owner.Behaviour.GetComponent<IDamageable>();
    }

    private void Start()
    {
        StartCoroutine(DamageTickRoutine());
    }

    private void OnTriggerEnter(Collider other)
    {
        var target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            _targetsInRange.Add(target);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var target = other.GetComponent<CharacterBehaviour>();
        if (target != null)
        {
            _targetsInRange.Remove(target);
        }
    }

    private IEnumerator DamageTickRoutine()
    {
        for (int i = 0; i < _totalTick; i++)
        {
            foreach (var target in _targetsInRange)
            {
                if (target != null && target != _damageSelf)
                {
                    target.TakeDamage(_tickDamage);
                }
            }

            yield return new WaitForSeconds(_tickTime);
            Debug.Log($"tickdamage");
        }

        Destroy(gameObject);
    }   
}
