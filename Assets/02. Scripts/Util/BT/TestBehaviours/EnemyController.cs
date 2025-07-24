using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private const float TICK = 0.5f;

    public Transform Target { get; private set; }

    private IBtNode _btRoot;

    private bool _isInitialized = false;
    private float _timer;

    [Header ("# Stats")]
    [SerializeField] private EEnemyType _enemyType;
    public EEnemyType EnemyType => _enemyType;
    public EnemyData EnemyData { get; private set; }

    [Header("# Components")]
    public NavMeshAgent NavMeshAgent { get; private set; }
    public Animator Animator { get; private set; }

    private void Awake()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();

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
}