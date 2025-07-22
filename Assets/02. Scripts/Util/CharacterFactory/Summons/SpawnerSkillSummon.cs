using UnityEngine;

public class SpawnerSkillSummon : MonoBehaviour, ISummonObject, IDamageable
{
    private CharacterBase _owner;

    private float _speed = 0f;
    private float _damage = 0f;
    private float _maxRange = 0f;
    private float _radius = 0f;

    private float _traveledDistance = 0f;
    private bool _isInitialized = false;

    public void Update()
    {
        if (!_isInitialized)
        {
            return;
        }

        float moveDistance = _speed * Time.deltaTime;

        Vector3 pos = transform.position;
        pos.y = 1.5f;
        transform.position = pos;

        _traveledDistance += moveDistance;
        if (_traveledDistance >= _maxRange)
        {
           
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
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
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    public void TakeDamage(float damage)
    {

    }

    public void Heal(float heal)
    {

    }

    public void SetData(SkillData data, CharacterBase character, CharacterBase target = null)
    {
        _owner = character;
        _speed = data.Speed;
        _damage = data.Damage;
        _maxRange = data.MaxRange;
        _radius = data.Radius;
        _traveledDistance = 0f;

        _isInitialized = true;
    }
}
