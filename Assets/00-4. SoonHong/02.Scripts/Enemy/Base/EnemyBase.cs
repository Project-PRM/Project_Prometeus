using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    public EnemyStat EnemyStat;

    private float _health;

    private void Awake()
    {
        _health = EnemyStat.MaxHelath;
    }
    public void UpdateMasterFlag()
    {
        EnemyBTBase bt = GetComponent<EnemyBTBase>();
        bt?.UpdateMasterClientFlag();
    }
    public void TakeDamage(float Damage)
    {
        _health -= Damage;
        if (_health <= 0.0f)
        {
            EnemyManager.Instance.Unregister(this);
        }
    }

    public void Heal(float Amount)
    {
        _health += Amount;
        if (_health > EnemyStat.MaxHelath)
            _health = EnemyStat.MaxHelath;
    }
}