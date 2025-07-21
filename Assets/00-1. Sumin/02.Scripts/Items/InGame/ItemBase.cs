using UnityEngine;

public abstract class ItemBase : MonoBehaviour, IPickupable
{
    protected ItemData _itemData;
    private Rigidbody _rigidBody;
    private Collider _collider;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        // 아이템 데이터 설정
        SetItemData();
    }

    public abstract void SetItemData();

    public bool Pickup(CharacterInventory playerInventory)
    {
        playerInventory.AddItem(_itemData);
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ground"))
        {
            _rigidBody.isKinematic = true;
            _rigidBody.useGravity = false;
        }
    }
}