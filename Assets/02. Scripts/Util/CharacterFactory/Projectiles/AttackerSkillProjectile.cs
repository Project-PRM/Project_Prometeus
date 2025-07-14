using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AttackerSkillProjectile : MonoBehaviour, IProjectile
{
    private CharacterBase _owner;

    private float _speed = 0f;
    private float _damage = 0f;
    private float _maxRange = 0f;
    private float _radius = 0f;
    private Vector3 _direction;

    private float _traveledDistance = 0f;
    private bool _isInitialized = false;

    public void Update()
    {
        if (!_isInitialized)
        {
            return;
        }

        float moveDistance = _speed * Time.deltaTime;
        transform.position += _direction.normalized * moveDistance;
        _traveledDistance += moveDistance;

        if (_traveledDistance >= _maxRange)
        {
            Explode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_owner != null && other.gameObject == _owner.Behaviour.gameObject)
        {
            return;
        }

        //Explode();
    }

    private void Explode()
    {
        // 여기 같은팀 처리 추가
        Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageAble>(out var target))
            {
                target.TakeDamage(_damage);
            }
        }

        /*PhotonNetwork.*/Destroy(gameObject);
    }

    public void SetData(SkillData data, CharacterBase character, Vector3? direction)
    {
        _owner = character;
        _speed = data.Speed;
        _damage = data.Damage;
        _maxRange = data.MaxRange;
        _radius = data.Radius;
        _direction = direction ?? Vector3.forward;
        _traveledDistance = 0f;

        _isInitialized = true;
    }
}
