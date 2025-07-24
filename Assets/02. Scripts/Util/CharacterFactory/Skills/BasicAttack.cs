using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[Skill("BasicAttack")]
public class BasicAttack : ISkillNoTarget
{
    private float _timer = 0f;

    public SkillData Data { get; set; }
    public CharacterBase Character { get; set; }
    public void SetOwner(CharacterBase character)
    {
        Character = character;
    }

    public void Update()
    {
        _timer += Time.deltaTime;
    }

    public GameObject GetIndicatorPrefab()
    {
        return Resources.Load<GameObject>($"Indicators/{Data.IndicatorPrefabName}");
    }

    public void Activate(CharacterBase user)
    {
        if (_timer < Data.Cooltime) return;
        _timer = 0f;

        user.Behaviour.PhotonView.RPC("RPC_SetAnimation", RpcTarget.All, "BasicAttack");

        bool current = user.Behaviour.Animator.GetBool("IsFirstAttack");
        bool next = !current;

        DamageTrigger trigger = user.Behaviour.DamageTrigger;
        user.Behaviour.StartCoroutine(EnableTriggerTemporarily(trigger, 0.5f));

        user.Behaviour.Animator.SetBool("IsFirstAttack", next);
    }

    private IEnumerator EnableTriggerTemporarily(DamageTrigger trigger, float duration)
    {
        trigger.TurnOnOrOff(true);
        yield return new WaitForSeconds(duration);
        trigger.TurnOnOrOff(false);
    }
}