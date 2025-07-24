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

    public void Activate()
    {
        if (_timer < Data.Cooltime) return;
        _timer = 0f;

        Character.Behaviour.PhotonView.RPC("RPC_SetAnimation", RpcTarget.All, "BasicAttack");

        bool current = Character.Behaviour.Animator.GetBool("IsFirstAttack");
        bool next = !current;

        DamageTrigger trigger = Character.Behaviour.DamageTrigger;
        Character.Behaviour.StartCoroutine(EnableTriggerTemporarily(trigger, 0.5f));

        Character.Behaviour.Animator.SetBool("IsFirstAttack", next);
    }

    private IEnumerator EnableTriggerTemporarily(DamageTrigger trigger, float duration)
    {
        trigger.TurnOnOrOff(true);
        yield return new WaitForSeconds(duration);
        trigger.TurnOnOrOff(false);
    }
}