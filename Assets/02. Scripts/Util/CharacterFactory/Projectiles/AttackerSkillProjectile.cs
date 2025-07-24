using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AttackerSkillProjectile : MonoBehaviour, IProjectile
{
    private CharacterBase _owner;
    private SkillData _data;

    private Vector3 _direction;

    private float _traveledDistance = 0f;
    private bool _isInitialized = false;

    public void Update()
    {
        if (!_isInitialized)
        {
            return;
        }

        float moveDistance = _data.Speed * Time.deltaTime;
        Vector3 move = _direction.normalized * moveDistance;

        // 이동: Y는 무시, 현재 위치의 y는 고정 -> 직선으로 가게 하고싶을때 사용
        transform.position += new Vector3(move.x, 0f, move.z);

        Vector3 pos = transform.position;
        pos.y = 1.5f;
        transform.position = pos;

        _traveledDistance += moveDistance;
        if (_traveledDistance >= _data.MaxRange)
        {
            Explode();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!_isInitialized)
        {
            return;
        }
        // 땅 처리
        if (collision.gameObject.CompareTag("Ground"))
        {
            return;
        }
        // 자기 자신 처리
        if (_owner != null && collision.gameObject == _owner.Behaviour.gameObject)
        {
            return;
        }
        // 팀원 처리

        Explode();
    }

    private void Explode()
    {
        Debug.Log("cube exploded");

        Collider[] hits = Physics.OverlapSphere(transform.position, _data.Radius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var target))
            {
                if (target is MonoBehaviour mb && mb.TryGetComponent<PhotonView>(out var targetView))
                {
                    // 자신 제외
                    if (_owner != null && targetView.ViewID == _owner.Behaviour.PhotonView.ViewID)
                        continue;

                    // 팀원 제외 (선택적으로)
                    // if (_owner.Team == target.Team) continue;

                    targetView.RPC("RPC_TakeDamage", RpcTarget.AllBuffered, _data.Damage);
                }
            }
        }

        PhotonNetwork.Destroy(gameObject);
    }

    public void SetData(SkillData data, CharacterBase character, Vector3? direction, CharacterBase target = null)
    {
        _owner = character;
        _data = data;
        _direction = direction ?? Vector3.forward;
        _traveledDistance = 0f;

        _isInitialized = true;
    }
}
