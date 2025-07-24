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
    [SerializeField] private LayerMask _raycastLayerMask;

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
        else if(damageable == _damageSelf)
        {
            _owner.RaiseEvent(ECharacterEvent.OnFulfunsFieldTouched);
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
                    float distance = Vector3.Distance(transform.position, mb.transform.position) + 0.1f;

                    RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, distance, _raycastLayerMask);

                    System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                    bool hitObstacleFirst = false;
                    bool hitTargetFirst = false;

                    foreach (var hit in hits)
                    {
                        var hitTransform = hit.transform;
                        int hitLayer = hitTransform.gameObject.layer;

                        if (hitLayer == LayerMask.NameToLayer("Obstacle"))
                        {
                            // 장애물이 먼저 맞았다면 루프 종료 — 데미지 안 줌
                            hitObstacleFirst = true;
                            break;
                        }

                        if (hitLayer == LayerMask.NameToLayer("Character") ||
                            hitLayer == LayerMask.NameToLayer("Enemy"))
                        {
                            if (hitTransform == mb.transform)
                            {
                                // 타겟이 먼저 맞았다면 데미지 주고 루프 종료
                                hitTargetFirst = true;
                                break;
                            }
                        }
                    }

                    if (hitTargetFirst && !hitObstacleFirst)
                    {
                        targetView.RPC("RPC_TakeDamage", RpcTarget.AllBuffered, _tickDamage);
                    }
                }
            }

            yield return wait;
        }

        PhotonNetwork.Destroy(gameObject);
    }
}
