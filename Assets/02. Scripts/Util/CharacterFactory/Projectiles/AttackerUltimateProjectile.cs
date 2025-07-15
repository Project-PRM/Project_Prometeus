using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UltimateProjectile : MonoBehaviour, IProjectile
{
    private CharacterBase _owner;

    private float _speed = 0f;
    private float _damage = 0f;
    private float _maxRange = 0f;
    private float _radius = 0f;
    private float _duration = 3f;

    private Vector3 _direction;
    private float _traveledDistance = 0f;
    private bool _isInitialized = false;

    public void Update()
    {
        if (!_isInitialized)
            return;

        float moveDistance = _speed * Time.deltaTime;
        transform.position += _direction.normalized * moveDistance;
        _traveledDistance += moveDistance;

        if (_traveledDistance >= _maxRange)
        {
            SpawnAoEField();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_owner != null && other.gameObject == _owner.Behaviour.gameObject)
            return;

        SpawnAoEField();
    }

    private void SpawnAoEField()
    {
        PhotonNetwork.Instantiate("UltimateAoEField", transform.position, Quaternion.identity);
        PhotonNetwork.Destroy(gameObject);
    }

    public void SetData(SkillData data, CharacterBase character, Vector3? direction, CharacterBase target = null)
    {
        _owner = character;
        _speed = data.Speed;
        _damage = data.Damage;
        _maxRange = data.MaxRange;
        _radius = data.Radius;
        _duration = data.Duration; // 장판 유지 시간
        _direction = direction ?? Vector3.forward;
        _traveledDistance = 0f;

        _isInitialized = true;
    }
}
