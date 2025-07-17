using System.Collections.Generic;
using UnityEngine;

public class CarryPanel : Singleton<CarryPanel>
{
    [SerializeField] private List<CarrySlot> _carrySlots;
    public List<CarrySlot> CarrySlots => _carrySlots;
    [SerializeField] private EquipmentPanelRootController _panelRoot;
    [SerializeField] private RectTransform _canvasRectTransform;

    public RectTransform CanvasRectTransform => _canvasRectTransform;

    public void ShowNear(Vector2 canvasLocalClickPos, ItemData item)
    {
        _panelRoot.gameObject.SetActive(true);
        _panelRoot.SetLocation(canvasLocalClickPos, item);
    }

    public void HideSubPanel()
    {
        _panelRoot.gameObject.SetActive(false);
    }

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
    }
}
