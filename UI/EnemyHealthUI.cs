using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    public Canvas healthCanvas; // 引用血量条的 Canvas
    public Image healthBarFill; // 引用血量条的填充部分

    private Enemy enemy;        // 引用 Enemy 脚本
    private float maxWidth;     // 血量条的最大宽度

    private void Awake()
    {
        enemy = GetComponent<Enemy>(); // 获取 Enemy 脚本
        healthCanvas.gameObject.SetActive(false); // 初始时隐藏血量条

        // 记录血量条的最大宽度
        if (healthBarFill != null)
        {
            maxWidth = healthBarFill.rectTransform.sizeDelta.x;
        }
        
    }

    private void Update()
    {
        if (enemy == null) return;

        // 更新血量条的显示
        UpdateHealthUI();

        // 检查玩家是否在检测范围内
        float distanceToPlayer = Vector3.Distance(transform.position, CharacterManager.instance.player.transform.position);
        if (distanceToPlayer <= enemy.detectionRange)
        {
            healthCanvas.gameObject.SetActive(true); // 显示血量条
        }
        else
        {
            healthCanvas.gameObject.SetActive(false); // 隐藏血量条
        }
    }

    void UpdateHealthUI()
    {
        if (healthBarFill == null) return;
        // 计算当前血量的比例
        float healthRatio = (float)enemy.currentHealth / enemy.maxHealth;
        // 调整血量条的宽度
        healthBarFill.rectTransform.sizeDelta = new Vector2(maxWidth * healthRatio, healthBarFill.rectTransform.sizeDelta.y);
    }
}