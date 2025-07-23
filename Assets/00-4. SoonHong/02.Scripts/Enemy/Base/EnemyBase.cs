using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    public EEnemyType EnemyType;

    public EnemyData EnemyData { get; private set; }
    public float CurrentHealth;

    [Header("# Components")]
    private Animator _animator;
    private EnemyBTBase _enemyBtBase;

    private void Awake()
    {
        _enemyBtBase = GetComponent<EnemyBTBase>();
        _animator = GetComponent<Animator>();
        //CurrentHealth = EnemyData.MaxHealth;
    }

    public void Start()
    {
        Invoke(nameof(Init), 1f); // Delay to ensure all components are initialized
    }

    private void Init()
    {
        if (EnemyDataManager.Instance.TryGetEnemyData(EnemyType.ToString(), out var data))
        {
            EnemyData = data;
            CurrentHealth = EnemyData.MaxHealth;
            _enemyBtBase.Init();
        }
        else
        {
            Debug.LogError($"EnemyData for {EnemyType} not found!");
        }
    }

    public void UpdateMasterFlag()
    {
        EnemyBTBase bt = GetComponent<EnemyBTBase>();
        bt?.UpdateMasterClientFlag();
    }

    public void RPC_TakeDamage(float Damage)
    {
        CurrentHealth -= DamageCalculator.CalculateDamage(Damage, EnemyData.Armor);
        Debug.Log($"[{gameObject.name}] 피해 받음: {Damage}, 현재 체력: {CurrentHealth}");
        if (CurrentHealth <= 0.0f)
        {
            EnemyManager.Instance.Unregister(this);
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"[{gameObject.name}] 사망");
        _animator.SetTrigger("Die");
        // 추가적인 사망 처리 로직 (예: 애니메이션, 효과 등)
        /*PhotonNetwork.*/Destroy(gameObject);
    }

    public void Heal(float Amount)
    {
        CurrentHealth += Amount;
        if (CurrentHealth > EnemyData.MaxHealth)
        {
            CurrentHealth = EnemyData.MaxHealth;
        }
    }
}