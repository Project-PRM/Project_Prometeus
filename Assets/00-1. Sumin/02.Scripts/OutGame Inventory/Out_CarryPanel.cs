using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 들고가는 장비 판넬
/// </summary>
public class Out_CarryPanel : Singleton<Out_CarryPanel>
{
    [SerializeField] private List<Out_CarrySlot> _carrySlots;
    public List<Out_CarrySlot> CarrySlots => _carrySlots;
    [SerializeField] private Out_EquipmentSubPanelController _panelRoot;
    [SerializeField] private RectTransform _canvasRectTransform;

    public RectTransform CanvasRectTransform => _canvasRectTransform;

    /// <summary>
    /// 서브 슬롯 띄우기
    /// </summary>
    public void ShowNear(Vector2 canvasLocalClickPos, ItemData item)
    {
        _panelRoot.gameObject.SetActive(true);
        _panelRoot.SetLocation(canvasLocalClickPos, item);
    }

    public void HideSubPanel()
    {
        _panelRoot.gameObject.SetActive(false);
    }

    /// <summary>
    /// 들고있는 슬롯, 서브 슬롯 동기화
    /// </summary>
    public void SyncCarryAndSubCarrySlots()
    {
        if (_panelRoot == null || _carrySlots == null || _panelRoot.CarrySlots == null)
            return;
        int count = Mathf.Min(_carrySlots.Count, _panelRoot.CarrySlots.Count);
        for (int i = 0; i < count; i++)
        {
            var item = _carrySlots[i].GetItem();
            _panelRoot.CarrySlots[i].SetItem(item, _panelRoot, i);
        }

        List<ItemData> carryingItems = new List<ItemData>();
        foreach(var slot in _carrySlots)
        {
            if (slot.GetItem() == null)
                continue;
            carryingItems.Add(slot.GetItem());
        }
        CarryManager.Instance.SetCarryingList(carryingItems);
    }
}
