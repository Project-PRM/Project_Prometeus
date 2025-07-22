public interface IPickupable
{
    /// <summary>
    /// 플레이어가 아이템을 줍는 동작을 실행하는 메서드
    /// </summary>
    /// <param name="playerInventory">아이템을 받을 플레이어 인벤토리</param>
    /// <returns>줍기 성공 여부 (예: 인벤토리 꽉 찼으면 false)</returns>
    public void OnPickup();
    public ItemData GetItemData();
}
