using UnityEngine;

public class UI_LobbyManager : MonoBehaviour
{
    public GameObject[] _lobbyPopUpUI;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            foreach(GameObject ui in _lobbyPopUpUI)
            {
                ui.SetActive(false);
            }
        }
    }
}
