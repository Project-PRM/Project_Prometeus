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
                    Vector3 direction = (mb.transform.position - transform.position).normalized;
                    float distance = Vector3.Distance(transform.position, mb.transform.position);

                    // Ray를 쏴서 직접 맞는 오브젝트가 target인지 확인
                    if (Physics.Raycast(transform.position, direction, out RaycastHit hit, distance))
                    {
                        // target과 직접 부딪혔는지 확인 (transform 기준 비교)
                        if (hit.transform == mb.transform)
                        {
                            targetView.RPC("RPC_TakeDamage", RpcTarget.AllBuffered, _tickDamage);
                        }
                    }
                }
            }

            yield return wait;
        }

        PhotonNetwork.Destroy(gameObject);
    }
}
