using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStreamingTrigger : MonoBehaviour
{
    [SerializeField] private string _loadTargetSceneName;
    [SerializeField] private string _unLoadTargetSceneName;

    private IEnumerator StreamingTargetScene()
    {
        var targetScene = SceneManager.GetSceneByName(_loadTargetSceneName);
        if (!targetScene.isLoaded)
        {
            var op = SceneManager.LoadSceneAsync(_loadTargetSceneName, LoadSceneMode.Additive);

            while (!op.isDone)
            {
                Debug.Log($"Loading scene: {_loadTargetSceneName}, Progress: {op.progress * 100}%");
                yield return null;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(StreamingTargetScene());
            Debug.Log($"Triggering scene streaming for: {_loadTargetSceneName}");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.UnloadSceneAsync(_unLoadTargetSceneName);
        }
    }
}
