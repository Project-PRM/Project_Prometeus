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

    public void TakeDamage(float Damage)
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
            if (itemObject != null)
            {
                itemObject.SetActive(true);

                // 위치 약간 랜덤하게
                Vector3 spawnPos = transform.position + Random.insideUnitSphere * 1f;
                spawnPos.y = Mathf.Abs(spawnPos.y); // y는 항상 양수
                itemObject.transform.position = spawnPos;

                // Rigidbody가 있으면 약하게 튕기듯 퍼지기
                if (itemObject.TryGetComponent<Rigidbody>(out var rb))
                {
                    Vector3 force = new Vector3(
                        Random.Range(-0.5f, 0.5f),  // x
                        Random.Range(0.3f, 0.6f),   // y (낮게 튀도록)
                        Random.Range(-0.5f, 0.5f)   // z
                    );
                    rb.AddForce(force * 1.5f, ForceMode.Impulse); // 힘도 약하게
                }
            }
        }

        /*PhotonNetwork.*/Destroy(gameObject);
    }
}