using UnityEngine;
using System.Collections.Generic;

public class FirstBossController : EnemyController
{
    protected override void Start()
    {
        _btRoot = new Selector(new List<IBtNode>
        {
            // 혼자 있는 상태
            new Sequence(new List<IBtNode>
            {
                new IsIdle(this),
                new Selector(new List<IBtNode>
                {

                })
            }),
            //페이즈 1
            new Sequence(new List<IBtNode>
            {
                new IsPhaseOne(this),
                new Selector(new List<IBtNode>
                {
                    new RushAttack(this),
                })
            }),
            //페이즈 2
            new Sequence(new List<IBtNode>
            {
                new IsPhaseTwo(this),
                new Selector(new List<IBtNode>
                {

                })
            }),
            //페이즈 3
            new Sequence(new List<IBtNode>
            {
                new Selector(new List<IBtNode>
                {

                })
            }),
        });
    }
}
