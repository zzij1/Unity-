using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;

public class GameEvents : MonoBehaviour
{
    // 单例实例
    public static GameEvents Instance { get; private set; }
    // 敌人死亡事件
    public UnityAction<string> OnEnemyKilled;

    // 物品捡起事件
    public UnityAction<string> OnItemPickedUp;
    //背包更新事件
    public UnityAction OnInventoryChanged;
    //人物状态更新事件
    public UnityAction OnPlayerStateChanged;
    private bool isPaused = false;
    private void Awake()
    {
        // 单例初始化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 防止场景切换时销毁
        }
        else
        {
            Destroy(gameObject); // 销毁多余的实例
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SaveSystem.instance.SaveGame();
            Debug.Log("游戏已保存！");
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (SaveSystem.instance.SaveFileExists())
            {
                StartCoroutine(LoadGameCoroutine());
                Debug.Log("游戏加载中...");
            }
            else
            {
                Debug.Log("没有找到存档！");
            }
        }
    }
    private void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;  // 暂停/恢复时间
        AudioListener.pause = isPaused;     // 暂停/恢复全局音频
    }
    // 新增协程：处理加载游戏的流程
    private IEnumerator LoadGameCoroutine()
    {
        // 确保玩家在加载过程中被禁用
        if (CharacterManager.instance != null)
        {
            CharacterManager.instance.player.gameObject.SetActive(false);
        }

        // 加载存档
        yield return SaveSystem.instance.LoadGame();

        // 确保玩家在加载后被启用
        if (CharacterManager.instance != null)
        {
            CharacterManager.instance.gameObject.SetActive(true);
        }
        
    }

    // 触发敌人死亡事件
    public void TriggerEnemyKilled(string enemyType)
    {
        OnEnemyKilled?.Invoke(enemyType);
    }

    // 触发物品捡起事件
    public void TriggerItemPickedUp(string itemType)
    {
        OnItemPickedUp?.Invoke(itemType);
    }
}