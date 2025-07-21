using UnityEngine;

public class UI_BottomMainBar : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private UI_DeadbodyPanel deadbodyPanel;
    [SerializeField] private UI_MarketPanel marketPanel;
    [SerializeField] private UI_CharacterPanel characterPanel;
    [SerializeField] private UI_GameplayPanel gameplayPanel;
    [SerializeField] private UI_EventPanel eventPanel;

    public void OnClickDeadbodyButton()
    {
        UI_PopUpManager.Instance.ShowPopUp(deadbodyPanel);
    }
    public void OnClickMarketButton()
    {
        UI_PopUpManager.Instance.ShowPopUp(marketPanel);
    }
    public void OnClickGameplayButton()
    {
        UI_PopUpManager.Instance.ShowPopUp(gameplayPanel);

    }
    public void OnClickCharacterButton()
    {
        UI_PopUpManager.Instance.ShowPopUp(characterPanel);

    }

    public void OnClickEventButton()
    {
        Debug.Log("Event는 현재 만든 UI가 없음 ");
        return;
        eventPanel?.Show();
    }
}
