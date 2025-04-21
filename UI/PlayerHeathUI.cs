using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealthUI : MonoBehaviour
{
    public Image healthSlider;    // 血条滑动条
    public Text healthText;      // 血量文本

    private void Awake()
    {
        // 自动获取子对象组件（如果未手动赋值）
        if (healthSlider == null)
            healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        if (healthText == null)
            healthText = healthSlider.GetComponentInChildren<Text>();

        DontDestroyOnLoad(gameObject); // 跨场景不销毁
        SceneManager.sceneLoaded += OnSceneLoaded; // 监听场景加载事件
    }

    // 场景加载时的回调
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 如果是主菜单场景，隐藏UI；否则显示
        gameObject.SetActive(scene.name != "MainMenu");
    }

    private void OnDestroy()
    {
        // 销毁时取消事件监听
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        UpdateHealth();
    }

    // 更新血量显示
    void UpdateHealth()
    {
        // 安全校验：确保CharacterManager和playerState存在
        if (CharacterManager.instance == null || CharacterManager.instance.playerState == null)
            return;

        int currentHealth = CharacterManager.instance.playerState.currentHealth;
        int maxHealth = CharacterManager.instance.playerState.maxHealth;

        // 更新血条填充值
        healthSlider.fillAmount = (float)currentHealth / maxHealth;
        // 更新文本（例如 "100/200"）
        healthText.text = $"{currentHealth}/{maxHealth}";
    }
}