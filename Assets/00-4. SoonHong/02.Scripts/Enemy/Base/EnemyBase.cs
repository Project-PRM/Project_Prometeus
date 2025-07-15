using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    public EnemyStat EnemyStat;

    private float _helath;

    private void Awake()
    {
        _helath = EnemyStat.MaxHelath;
    }

    public void TakeDamage(float Damage)
    {
        _helath -= Damage;
    }

    public void Heal(float Amount)
    {
        _helath += Amount;
        if (_helath > EnemyStat.MaxHelath)
            _helath = EnemyStat.MaxHelath;
    }
}