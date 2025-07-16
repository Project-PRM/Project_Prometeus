using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    private ItemData _item;

    public void SetItem(ItemData newItem)
    {
        _item = newItem;
        // 아이콘 등 설정
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 캔버스 기준 클릭 위치 전달
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            CarryPanel.Instance.CanvasRectTransform, // 캔버스의 RectTransform
            eventData.position, // 마우스 클릭 위치
            eventData.pressEventCamera, // 클릭 이벤트 카메라 (Screen Space - Camera일 경우 필요)
            out Vector2 localClickPosition
        );

        CarryPanel.Instance.ShowNear(localClickPosition, _item);
    }
}
