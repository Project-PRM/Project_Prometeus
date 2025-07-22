using UnityEngine;

public class SpawnerSkillSummon : MonoBehaviour, ISummonObject, IDamageable
{
    private CharacterBase _owner;

    // TODO : 스탯 modifier 영향 받게? 
    private float _maxHealth = 20f;
    private float _curHealth;

    private float _timer = 0f;
    private float _speed = 0f;
    private float _damage = 0f;
    private float _maxRange = 0f;
    private float _radius = 0f;
    private float _duration = 0f;

    private bool _isInitialized = false;

    public void Start()
    {
        SpawnSlowField();
    }

    public void Update()
    {
        if (!_isInitialized)
        {
            return;
        }

        // 타이머 기반이 아님
        /*if(_timer >= _duration)
        {
            *//*PhotonNetwork.*//*Destroy(gameObject); Destroy(this);
        }*/

        _timer += Time.deltaTime;

        Vector3 pos = transform.position;
        transform.position = pos;
    }

    private void SpawnSlowField()
    {
        GameObject prefab = Resources.Load<GameObject>("AttackerSlowField");
        /*PhotonNetwork.*/
        GameObject field = Instantiate(
        prefab,
        transform.position,
        Quaternion.identity,
        transform
        );

        SlowField slowField = field.GetComponent<SlowField>();
        slowField.StartSlowField(_owner);
    }

    public void TakeDamage(float damage)
    {
        // 현재 방어 수치 애매
        _curHealth -= DamageCalculator.CalculateDamage(damage, 0);
        if(_curHealth <= 0)
        {
            Destroy(this);
        }
    }

    public void Heal(float heal)
    {
        _curHealth += heal;
        _curHealth = Mathf.Min(_curHealth, _maxHealth);
    }

    public void SetData(SkillData data, CharacterBase character, CharacterBase target = null)
    {
        _owner = character;
        _speed = data.Speed;
        _damage = data.Damage;
        _maxRange = data.MaxRange;
        _radius = data.Radius;
        _duration = data.Duration;

        _curHealth = _maxHealth;

        _isInitialized = true;
    }
}
