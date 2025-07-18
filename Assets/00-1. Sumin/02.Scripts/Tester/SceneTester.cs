using UnityEngine;

public class SceneTester : MonoBehaviour
{
    public string NextSceneName = "Sumin_Main_copied";

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(NextSceneName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(NextSceneName);
        }
        else
        {
            Debug.LogWarning("Next scene name is not set.");
        }
    }
}
