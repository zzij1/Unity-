using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景渐变控制器
/// 处理场景切换时的淡入淡出效果
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance { get; private set; }

    [Header("渐变设置")]
    [Tooltip("淡入淡出持续时间(秒)")]
    [SerializeField] private float fadeDuration = 1f;

    private CanvasGroup canvasGroup;  // 控制渐变的CanvasGroup组件
    
    private void Awake()
    {
        // 单例模式实现
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // 注册场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // 初始化组件
        canvasGroup = GetComponent<CanvasGroup>();
        // 确保始终显示在最上层
        GetComponent<Canvas>().sortingOrder = 999; 
        ResetState();
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 确保只有当前实例处理场景加载事件
        if (this == Instance) 
        {
            ResetState();
        }
    }

    /// <summary>
    /// 重置渐变状态为透明
    /// </summary>
    private void ResetState()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    /// <summary>
    /// 淡出效果（屏幕变黑）
    /// </summary>
    public IEnumerator FadeOut()
    {
        float timer = 0;
        while (timer < fadeDuration)
        {
            // 线性插值计算透明度
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
        // 阻止点击穿透
        canvasGroup.blocksRaycasts = true; 
    }

    /// <summary>
    /// 淡入效果（屏幕变亮）
    /// </summary>
    public IEnumerator FadeIn()
    {
        float timer = 0;
        while (timer < fadeDuration)
        {
            // 线性插值计算透明度
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        ResetState();
    }
    
}