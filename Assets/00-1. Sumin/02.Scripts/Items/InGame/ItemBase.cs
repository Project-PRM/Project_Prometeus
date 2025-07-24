using Photon.Pun;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour, IPickupable
{
    protected ItemData _itemData;
    private Rigidbody _rigidBody;
    private Collider _collider;

    private bool _hasLanded = false;
    private bool _allowLandingCheck = false;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        ResetPhysics();
    }

    private void Start()
    {
        SetItemData(); // 아이템 데이터 설정
    }

    public abstract void SetItemData();

    public void OnPickup()
    {
        Debug.Log($"아이템 {_itemData.Name}을(를) 획득했습니다.");
        Destroy(gameObject); // 이펙트, 사운드 등 후처리는 여기서
    }

    public ItemData GetItemData() => _itemData;

    public virtual void SpreadFrom(Vector3 origin)
    {
        origin += Vector3.up;
        // 위치 약간 랜덤화
        Vector3 spawnPos = origin + Random.insideUnitSphere * 1f;
        spawnPos.y = Mathf.Abs(spawnPos.y);
        transform.position = spawnPos;

        gameObject.SetActive(true);

        _allowLandingCheck = true;
        _hasLanded = false;

        if (_rigidBody != null)
        {
            _rigidBody.isKinematic = false;
            _rigidBody.useGravity = true;

            Vector3 force = new Vector3(
                Random.Range(-0.5f, 0.5f),
                Random.Range(0.3f, 0.6f),
                Random.Range(-0.5f, 0.5f)
            );
            _rigidBody.AddForce(force * 1.5f, ForceMode.Impulse);
        }

        if (_collider != null)
            _collider.isTrigger = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_allowLandingCheck || _hasLanded) return;

        if (collision.collider.CompareTag("Ground"))
        {
            _hasLanded = true;

            if (_rigidBody != null)
            {
                _rigidBody.isKinematic = true;
                _rigidBody.useGravity = false;
            }

            if (_collider != null)
            {
                _collider.isTrigger = true;
            }

            Debug.Log("아이템이 착지하였고 Trigger로 전환됨");
        }
    }

    private void ResetPhysics()
    {
        _hasLanded = false;
        _allowLandingCheck = false;

        if (_rigidBody != null)
        {
            _rigidBody.isKinematic = true;
            _rigidBody.useGravity = false;
        }

        if (_collider != null)
        {
            _collider.isTrigger = true;
        }
    }
}
