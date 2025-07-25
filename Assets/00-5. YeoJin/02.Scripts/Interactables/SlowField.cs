using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowField : MonoBehaviour
{
    private CharacterBase _owner;
    private SkillData _data;

    [SerializeField] private float _duration;
    [SerializeField] private float _slowAmount = -2f;
    private SlowEffect _slowEffect;

    private HashSet<CharacterBase> _targetsInRange = new HashSet<CharacterBase>();

    public void StartSlowField(CharacterBase character, SkillData data)
    {
        _owner = character;
        _data = data;
        _slowAmount = _data.DebuffAmount[EStatType.MoveSpeed];
    }

    private void Start()
    {
        _slowEffect = new SlowEffect(_data.Duration, _data.DebuffAmount[EStatType.MoveSpeed]);
        //_slowEffect.Add(EStatType.MoveSpeed, _slowAmount);
    }

    private void OnTriggerEnter(Collider other)
    {
        var target = other.GetComponent<CharacterBase>();
        if (target != null && target != _owner)
        {
            _targetsInRange.Add(target);
            //target.AddStatModifier(_slowEffect);
            _slowEffect.Apply(target);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var target = other.GetComponent<CharacterBase>();
        if (target != null && target != _owner)
        {
            _targetsInRange.Remove(target);
            _slowEffect.Remove(target);
        }
    }

    private void OnDestroy()
    {
        foreach(var character in _targetsInRange)
        {
            //character.RemoveStatModifier(_slowEffect);
            _slowEffect.Remove(character);
        }
    }
}
