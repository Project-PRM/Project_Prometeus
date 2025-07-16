using System.Collections.Generic;
using UnityEngine;

public class EquipmentPanelRootController : MonoBehaviour
{
    public GameObject Panel;
    public List<CarrySlot> CarrySlots;

    private void OnEnable()
    {
        for(int i=0; i<CarrySlots.Count; ++i)
        {
            CarrySlots[i].SetItem(CarryPanel.Instance.CarrySlots[i].GetItem());
        }
    }

    public void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            CarryPanel.Instance.Hide();
        }
    }

    public void SetLocation(Vector2 location, ItemData item) 
    {
        for(int i=0; i<CarrySlots.Count; ++i)
        {
            if(item != null)
                CarrySlots[i].gameObject.SetActive(CarrySlots[i].AllowedType == item.ItemType);
        }
        Panel.GetComponent<RectTransform>().anchoredPosition = location;
    }
}
