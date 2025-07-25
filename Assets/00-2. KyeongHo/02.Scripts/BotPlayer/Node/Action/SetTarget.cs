using UnityEngine;

public class SetTarget : IActionNode
{
    private readonly BotController _bot;
    public SetTarget(BotController bot) =>  _bot = bot;
    public ENodeState Evaluate()
    {
        GameObject newTarget = new GameObject();// 새로운 팀원을 set
        _bot.Target = newTarget;
        return newTarget != null ? ENodeState.Success : ENodeState.Failure;
    }
}
