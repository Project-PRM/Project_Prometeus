using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class GroundItemBox : MonoBehaviour, IDamageable
{
    [SerializeField] private float _health = 100f;

    private List<ItemData> _itemList;
    private List<GameObject> _itemObjects = new List<GameObject>();

    public void Start()
    {
        
    }

    /// <summary>
    /// 최초에 게임매니저가 이거 호출
    /// </summary>
    public void SetItem(List<ItemData> itemDatas)
    {
        _itemList = itemDatas;

        foreach(var item in _itemList)
        {
            GameObject itemObject = PhotonNetwork.Instantiate(item.Name, transform.position, Quaternion.identity);
            _itemObjects.Add(itemObject);
            itemObject.SetActive(false);
        }
    }

    public void TakeDamage(float Damage)
    {
        _health -= Damage;
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
                        Random.Range(-1f, 1f),           // x
                        Random.Range(1f, 2f),            // y 항상 양수
                        Random.Range(-1f, 1f)            // z
                    );
                    rb.AddForce(force * 3f, ForceMode.Impulse);
                }
            }
        }

        /*PhotonNetwork.*/Destroy(gameObject);
    }
}