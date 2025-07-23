using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class GroundItemBox : MonoBehaviour, IDamageable
{
    [SerializeField] private float _health = 100f;

    private List<ItemData> _itemList;
    private List<GameObject> _itemObjects = new List<GameObject>();

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            List<ItemData> datas = new();
            //Test
            if (ItemManager.Instance.TryGetItemData("TestItem", out var itemData))
            {
                datas.Add(itemData);
            }
            if (ItemManager.Instance.TryGetItemData("TestArmor", out var data))
            {
                datas.Add(data);
            }
            SetItem(datas);
        }   
    }

    /// <summary>
    /// 최초에 게임매니저가 이거 호출
    /// </summary>
    public void SetItem(List<ItemData> itemDatas)
    {
        _itemList = itemDatas;

        foreach(var item in _itemList)
        {
            GameObject temp = Resources.Load<GameObject>(item.Name);
            GameObject itemObject = Instantiate(temp, transform.position, Quaternion.identity);
            //GameObject itemObject = PhotonNetwork.Instantiate(item.Name, transform.position, Quaternion.identity);
            _itemObjects.Add(itemObject);
            itemObject.SetActive(false);
        }
    }

    public void RPC_TakeDamage(float Damage)
    {
        _health -= DamageCalculator.CalculateDamage(Damage, 0);
        if (_health <= 0f)
        {
            DestroyAndSpreadItems();
        }
    }

    public void Heal(float Amount)
    {
    }

    public void DestroyAndSpreadItems()
    {
        foreach (var itemObject in _itemObjects)
        {
            if (itemObject.TryGetComponent<ItemBase>(out var item))
            {
                item.SpreadFrom(transform.position);
            }
        }

        /*PhotonNetwork.*/Destroy(gameObject);
    }
}