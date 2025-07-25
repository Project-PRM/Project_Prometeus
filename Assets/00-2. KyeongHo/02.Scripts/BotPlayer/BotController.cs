using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BotController : MonoBehaviour, IDamageable
{
    private IBtNode _btRoot;

    private BotData _botData;
    public BotData BotData => _botData;
    public GameObject Target;
    private void Start()
    {
        _btRoot = new Selector(new List<IBtNode>
        {
            new Sequence(new List<IBtNode>
            {
                new IsTargetNull(Target),
                new SetTarget(this)
            }),
            // new Sequence(new List<IBtNode>
            // {
            //     // new IsPlayerInSight(this),
            //     // new PatrolAction(this) // TODO : 플레이어 Trace
            // }),
        });
    }
    public void RPC_TakeDamage(float Damage)
    {
    }
    public void Heal(float Amount)
    {
    }
}
