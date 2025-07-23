using UnityEngine;

public interface IDamageable 
{
    public void RPC_TakeDamage(float Damage);
    public void Heal(float Amount); // 나중에 인터페이스 IHealable 분리하자
}
