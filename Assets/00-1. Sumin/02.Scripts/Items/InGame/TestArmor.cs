using UnityEngine;

public class TestArmor : ItemBase
{
    public override void SetItemData()
    {
        if (ItemManager.Instance.TryGetItemData(nameof(TestArmor), out var data))
        {
            _itemData = data;
        }
        else
        {
            Debug.LogError($"아이템 데이터 '{nameof(TestArmor)}'을(를) 찾을 수 없습니다.");
        }
    }
}