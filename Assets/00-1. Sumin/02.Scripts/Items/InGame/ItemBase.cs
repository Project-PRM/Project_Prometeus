using Photon.Pun;
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
        // 아이템 데이터 설정, 추후 주석
        SetItemData();
    }

    public abstract void SetItemData();

    /*public bool Pickup(CharacterInventory playerInventory)
    {
        playerInventory.AddItem(_itemData);
        return true;
    }*/
    public void OnPickup()
    {
        Debug.Log($"아이템 {_itemData.Name}을(를) 획득했습니다.");
        // TODO : 사운드, 이펙트 등 추가
        /*PhotonNetwork.*/
        Destroy(gameObject);
    }

    public ItemData GetItemData()
    {
        return _itemData;
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ground"))
        {
            _rigidBody.isKinematic = true;
            _rigidBody.useGravity = false;
        }
    }*/
}