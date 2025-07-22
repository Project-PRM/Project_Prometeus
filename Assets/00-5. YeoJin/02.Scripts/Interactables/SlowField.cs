using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowField : MonoBehaviour
{
    private CharacterBase _owner;

    [SerializeField] private float _duration;
    [SerializeField] private float _slowAmount = -2f;
    private StatModifier _slowEffect;

    private HashSet<CharacterBase> _targetsInRange = new HashSet<CharacterBase>();

    public void StartSlowField(CharacterBase character)
    {
        _owner = character;
    }

    private void Start()
    {
        _slowEffect = new StatModifier();
        _slowEffect.Add(EStatType.MoveSpeed, -2);
    }

    private void OnTriggerEnter(Collider other)
    {
        var target = other.GetComponent<CharacterBase>();
        if (target != null && target != _owner)
        {
            _targetsInRange.Add(target);
            target.AddStatModifier(_slowEffect);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var target = other.GetComponent<CharacterBase>();
        if (target != null && target != _owner)
        {
            _targetsInRange.Remove(target);
            target.RemoveStatModifier(_slowEffect);
        }
    }

    private void OnDestroy()
    {
        foreach(var character in _targetsInRange)
        {
            character.RemoveStatModifier(_slowEffect);
        }
    }
}
