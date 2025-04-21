using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // 单例实例
    public static SceneLoader Instance { get; private set; }

    private void Awake()
    {
        // 单例模式验证
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <param name="withFade">是否使用淡入淡出效果</param>
    public void LoadScene(string sceneName, bool withFade = true)
    {   
        StartCoroutine(LoadSceneCoroutine(sceneName, withFade));
    }

    public IEnumerator LoadSceneCoroutine(string sceneName, bool withFade)
    {
        // 验证场景是否存在
        if (!SceneExists(sceneName))
        {
            Debug.LogError($"场景不存在: {sceneName}");
            yield break;
        }

        yield return SceneFader.Instance.FadeOut();

        // 异步加载场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        // 确保场景完全激活
        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// 检查场景是否存在
    /// </summary>
    private bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            if (System.IO.Path.GetFileNameWithoutExtension(scenePath) == sceneName)
                return true;
        }
        return false;
    }
}