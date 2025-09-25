using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class AsinhroneLoad : MonoBehaviour
{
    private AsyncOperation asyncOperation; // Указываем пространство имён UnityEngine
    public GameObject loadpanel;
    public TextMeshProUGUI progressText;
    // Метод для запуска загрузки сцены
    public void LoadNewScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOperation.isDone)
        {            // Прогресс загрузки от 0 до 0.9
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            // Обновление текста прогресса
            progressText.text = Mathf.FloorToInt(progress * 100) + "%";
            loadpanel.SetActive(true);
            yield return null; // Ждем следующего кадра
        }
    }
}
