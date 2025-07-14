using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AttackerSkillProjectile : MonoBehaviour, IProjectile
{
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
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageAble>(out var damageable))
        {
            damageable.TakeDamage(_damage);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void SetData(SkillData data, CharacterBase character, Vector3? direction)
    {
        _speed = data.Speed;
        _damage = data.Damage;
        _maxRange = data.MaxRange;
        _radius = data.Radius;
        _direction = direction ?? Vector3.forward;
        _traveledDistance = 0f;

        _isInitialized = true;
    }
}
