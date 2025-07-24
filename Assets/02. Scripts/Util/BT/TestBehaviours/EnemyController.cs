using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class EnemyController : MonoBehaviour, IDamageable
{
    private const float TICK = 0.5f;
    private IBtNode _btRoot;

    public Transform Target { get; private set; }

    private bool _isInitialized = false;
    private float _timer;

    [SerializeField] private EEnemyType _enemyType;
    public EEnemyType EnemyType => _enemyType;
    public Transform[] PatrolPoints;

    [Header ("# Stats")]
    public EnemyData EnemyData { get; private set; }
    public float CurrentHealth { get; private set; }

    [Header("# Components")]
    public NavMeshAgent NavMeshAgent { get; private set; }
    public Animator Animator { get; private set; }
    public PhotonView PhotonView { get; private set; }

    private void Awake()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        PhotonView = GetComponent<PhotonView>();

        // 임시
        Invoke(nameof(Initialize), 2f);
    }

    private void Start()
    {
        _btRoot = new Selector(new List<IBtNode>
        {
            new Sequence(new List<IBtNode>
            {
                new IsPlayerInAttackRange(this),
                new AttackAction(this)
            }),
            new Sequence(new List<IBtNode>
            {
                new IsPlayerInSight(this),
                new ChaseAction(this)
            }),
            new PatrolAction(this)
        });
    }

    private void Initialize()
    {
        if(EnemyDataManager.Instance.TryGetEnemyData(_enemyType.ToString(), out var data))
        {
            EnemyData = data;
            _isInitialized = true;
            NavMeshAgent.speed = EnemyData.Speed;
            CurrentHealth = EnemyData.MaxHealth;
        }
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < TICK || !_isInitialized)
        {
            return;
        }

        _btRoot.Evaluate();
    }

    public void SetTarget(Transform target)
    {
        Target = target;
    }

    [PunRPC]
    public void RPC_TakeDamage(float Damage)
    {
        CurrentHealth -= DamageCalculator.CalculateDamage(Damage, EnemyData.Armor);
        if(CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float Amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + Amount, EnemyData.MaxHealth);
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        /*PhotonNetwork.*/Destroy(gameObject);
    }
}