using UnityEngine;

public static class DamageCalculator
{
    public static float CalculateDamage(float baseDamage, float armor/*, float armorPenetration*/)
    {
        // 방어력 관통 적용
        //float effectiveArmor = Mathf.Max(armor - armorPenetration, 0f);
        float effectiveArmor = armor;

        // 피해 계산: 피해량 * (1 - 방어력 / (방어력 + 100))
        float damage;
        if (armor >= 0)
        {
            damage = baseDamage * (1f - 1 / (1 + effectiveArmor * 0.01f));
        }
        else
        {
            damage = baseDamage * (2f - 1 / (1 - effectiveArmor * 0.01f));
        }

        return Mathf.Max(damage, 0f); // 피해는 음수가 될 수 없음
    }
}