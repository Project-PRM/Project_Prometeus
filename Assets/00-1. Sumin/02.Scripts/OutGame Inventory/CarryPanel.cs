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
        // 위치 설정: 캔버스 기준 좌표로 패널 위치 지정
        //RectTransform panelRect = _panelRoot.GetComponent<RectTransform>();
        //panelRect.anchoredPosition = canvasLocalClickPos + new Vector2(150f, 0f);

        // 아이템 배치
        //foreach (var slot in _carrySlots)
        //{
        //    if (slot.CanAccept(item) && slot.IsEmpty)
        //    {
        //        slot.SetItem(item);
        //        break;
        //    }
        //}
    }

    public void Hide()
    {
        _panelRoot.gameObject.SetActive(false);
    }
}
