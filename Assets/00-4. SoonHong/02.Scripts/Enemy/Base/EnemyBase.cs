using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageAble
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
}
