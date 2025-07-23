using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AttackerAoEField : MonoBehaviour
{
    private CharacterBase _owner;
    private IDamageable _damageSelf;

    [SerializeField] private float _duration;
    [SerializeField] private float _tickDamage;
    [SerializeField] private float _tickTime = 0.5f;
    [SerializeField] private int _totalTick = 10;

    private HashSet<IDamageable> _targetsInRange = new HashSet<IDamageable>();

    public void StartAoEField(CharacterBase character, float duration, float tickDamage)
    {
        _owner = character;
        _duration = duration;
        _tickDamage = tickDamage;

        _damageSelf = _owner.Behaviour.GetComponent<IDamageable>();
        StartCoroutine(DamageTickRoutine());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IDamageable>(out var damageable) && damageable != _damageSelf)
        {
            _targetsInRange.Add(damageable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var damageable) && damageable != _damageSelf)
        {
            _targetsInRange.Remove(damageable);
        }
    }

    private IEnumerator DamageTickRoutine()
    {
        var wait = new WaitForSeconds(_tickTime);
        for (int i = 0; i < _totalTick; i++)
        {
            foreach (var target in _targetsInRange)
            {
                if (target is MonoBehaviour mb && mb.TryGetComponent<PhotonView>(out var targetView))
                {
                    // 피해자 클라이언트에게 데미지 적용 요청
                    targetView.RPC("TakeDamage", targetView.Owner, _tickDamage);
                    Debug.Log($"{target} : Tick - {_tickDamage}");
                }
            }

            yield return wait;
        }

        PhotonNetwork.Destroy(gameObject);
    }
}
