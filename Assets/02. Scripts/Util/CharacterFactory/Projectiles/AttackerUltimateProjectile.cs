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
        {
            return;
        }

        float moveDistance = _speed * Time.deltaTime;
        Vector3 move = _direction.normalized * moveDistance;

        // 이동: Y는 무시, 현재 위치의 y는 고정 -> 직선으로 가게 하고싶을때 사용
        transform.position += new Vector3(move.x, 0f, move.z);

        Vector3 pos = transform.position;
        pos.y = 1.5f;
        transform.position = pos;

        _traveledDistance += moveDistance;
        if (_traveledDistance >= _maxRange)
        {
            SpawnAoEField();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log($"collided with {collision.gameObject}");
        // 땅 처리
        //if (collision.gameObject.CompareTag("Ground"))
        //{
        //    return;
        //}
        // 자기 자신 처리
        if (_owner != null && collision.gameObject == _owner.Behaviour.gameObject)
        {
            return;
        }
        // 팀원 처리

        SpawnAoEField();
    }

    private void SpawnAoEField()
    {
        GameObject field = Resources.Load<GameObject>("AttackerAoEField");
        /*PhotonNetwork.*/GameObject.Instantiate(field, transform.position, Quaternion.identity);
        /*PhotonNetwork.*/Destroy(gameObject);
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
