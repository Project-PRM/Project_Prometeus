using System.Collections;
using TMPro;
using UnityEngine;

public class SampleLoading : MonoBehaviour
{
    public TextMeshProUGUI UI_LoadingText;

    public float LoadingSucessTime = 3f;

    private float _loadingTimer = 0f;


    private void Update()
    {
        if (_loadingTimer < LoadingSucessTime)
        {
            _loadingTimer += Time.deltaTime;
            UI_LoadingText.text = $"Loading... {_loadingTimer:F2} seconds";
        }
        else
        {
            UI_LoadingText.text = "Loading Complete!";
            // Optionally, you can disable the loading text or perform other actions here.
        }
    }
}
