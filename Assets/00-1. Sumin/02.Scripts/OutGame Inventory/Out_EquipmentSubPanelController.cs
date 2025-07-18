using System.Collections.Generic;
using UnityEngine;

public class Out_EquipmentSubPanelController : MonoBehaviour
{
    public GameObject Panel;
    public List<Out_SubCarrySlot> CarrySlots;
    public Out_UserInventorySlot OriginSlot;
    private Out_UserInventorySlot _realOriginInventorySlot; // 실제 인벤토리 슬롯 참조

    private void OnEnable()
    {
        /*for(int i=0; i<CarrySlots.Count; ++i)
        {
            CarrySlots[i].SetItem(CarryPanel.Instance.CarrySlots[i].GetItem(), this, i);
        }*/
    }

    public void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            Out_CarryPanel.Instance.HideSubPanel();
        }
    }

    public void SetLocation(Vector2 location, ItemData originItem) 
    {
        OriginSlot.SetItem(originItem);
        for (int i=0; i<CarrySlots.Count; ++i)
        {
            if(originItem != null)
            {
                bool flag = CarrySlots[i].AllowedType == originItem.ItemType;
                CarrySlots[i].gameObject.SetActive(flag);
                if (flag)
                {
                    CarrySlots[i].SetItem(Out_CarryPanel.Instance.CarrySlots[i].GetItem(), this, i);
                }
            }
        }
        Panel.GetComponent<RectTransform>().anchoredPosition = location;
    }

    // InventorySlot을 Origin으로 지정
    public void SetOriginSlot(Out_UserInventorySlot slot)
    {
        _realOriginInventorySlot = slot;
        OriginSlot.SetItem(slot.GetItem());
    }

    // SubCarrySlot 클릭 시 OriginSlot과 스왑 및 CarrySlot 동기화
    public void SwapWithOrigin(Out_SubCarrySlot subSlot)
    {
        int idx = subSlot.SlotIndex;
        var originItem = OriginSlot.GetItem();
        var subItem = subSlot.GetItem();

        // 스왑
        OriginSlot.SetItem(subItem);
        subSlot.SetItem(originItem, this, idx);

        // CarrySlot 동기화
        Out_CarryPanel.Instance.CarrySlots[idx].SetItem(originItem);

        // InventorySlot 동기화
        if (_realOriginInventorySlot != null)
            _realOriginInventorySlot.SetItem(subItem);
    }
}
