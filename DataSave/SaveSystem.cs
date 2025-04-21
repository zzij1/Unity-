using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem instance;
    public bool isLoading = false;
    private string savePath;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 防止场景切换时销毁
        }
        else
        {
            Destroy(gameObject); // 销毁多余的实例
        }

        savePath = Application.persistentDataPath + "/save.json";
    }

    public void SaveGame()
    {
        try
        {
            // 1. 验证系统是否准备好保存
            if (!IsReadyToSave())
            {
                throw new Exception("无法保存：关键游戏组件未初始化");
            }

            // 2. 收集所有需要保存的数据
            var saveData = new GameSaveData
            {
                sceneName = SceneManager.GetActiveScene().name,
                player = SavePlayerData(),
                inventory = SaveInventoryData(),
                quests = SaveQuestData(),
                enemies = SaveEnemyData(),
                lastUsedPortalID = PortalManager.Instance?.lastUsedPortalID
            };

            // 3. 序列化并保存到文件
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(savePath, json);

            Debug.Log($"游戏已保存到: {savePath}\n" +
                      $"场景: {saveData.sceneName}\n" +
                      $"玩家位置: {CharacterManager.instance.player.position}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"保存游戏失败: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public IEnumerator LoadGame()
    {
        if (isLoading) yield break;
        isLoading = true;

        // 1. 验证存档文件
        if (!File.Exists(savePath))
        {
            Debug.LogError($"存档文件不存在: {savePath}");
            yield break;
        }

        // 2. 读取并解析存档数据
        string json = File.ReadAllText(savePath);
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

        if (saveData == null)
        {
            Debug.LogError("存档数据解析失败");
            yield break;
        }

        // 3. 显示加载界面
        if (SceneFader.Instance != null)
        {
            yield return SceneFader.Instance.FadeOut();
        }

        // 4. 处理场景切换
        bool needSceneChange = SceneManager.GetActiveScene().name != saveData.sceneName;
        if (needSceneChange)
        {
            SceneLoader.Instance.LoadScene(saveData.sceneName);
            // 确保场景完全加载
            yield return new WaitForEndOfFrame();
        }
        else
        {
            Debug.Log("同场景加载修改人物位置");
            CharacterManager.instance.TeleportPlayer(saveData.player.position);
            CharacterManager.instance.player.gameObject.SetActive(true);
            
        }

        // 5. 等待所有必需系统初始化
        // yield return WaitForEssentialSystems();

        // 6. 恢复传送门状态
        if (PortalManager.Instance != null)
        {
            PortalManager.Instance.lastUsedPortalID = saveData.lastUsedPortalID;
        }

        // 7. 恢复玩家状态
        RestorePlayer(saveData.player);

        // 8. 恢复其他游戏系统
        yield return RestoreInventory(saveData.inventory);
        RestoreQuests(saveData.quests);
        yield return RestoreEnemies(saveData.enemies);
        yield return SceneFader.Instance.FadeIn();
        Debug.Log($"游戏加载完成！场景: {saveData.sceneName}");
    }

    #region 数据保存方法

    private static bool IsReadyToSave()
    {
        return CharacterManager.instance != null &&
               CharacterManager.instance.player != null &&
               Inventory.instance != null &&
               QuestManager.instance != null;
    }

    private static PlayerSaveData SavePlayerData()
    {
        Transform playerTransform = CharacterManager.instance.player.transform;
        PlayerState playerState = CharacterManager.instance.playerState;

        return new PlayerSaveData
        {
            currentHealth = playerState.currentHealth,
            maxHealth = playerState.maxHealth,
            damage = playerState.damage,
            position = playerTransform.position
        };
    }

    private static InventorySaveData SaveInventoryData()
    {
        return new InventorySaveData
        {
            items = Inventory.instance.items
                .Where(item => item != null)
                .Select(item => item.name)
                .ToArray(),
            equippedWeapon = CharacterManager.instance.weaponSlot?.item?.name
        };
    }

    private static QuestSaveData[] SaveQuestData()
    {
        return QuestManager.instance.tasks
            .Where(task => task.questData != null)
            .Select(task => new QuestSaveData
            {
                questId = task.questData.questName,
                isCompleted = task.isCompleted,
                requirementProgress = task.questData.questRequires
                    .Select(r => r.currentAmount)
                    .ToArray()
            }).ToArray();
    }

    private static EnemySaveData[] SaveEnemyData()
    {
        return GameObject.FindObjectsOfType<Enemy>()
            .Where(enemy => enemy != null)
            .Select(enemy => new EnemySaveData
            {
                enemyId = enemy.enemyID,
                currentHealth = enemy.currentHealth,
                position = new float[]
                {
                    enemy.transform.position.x,
                    enemy.transform.position.y,
                    enemy.transform.position.z
                }
            }).ToArray();
    }

    #endregion

    #region 数据恢复方法
    private static void RestorePlayer(PlayerSaveData playerData)
    {
        if (CharacterManager.instance == null || CharacterManager.instance.player == null)
        {
            Debug.LogError("无法恢复玩家：玩家系统未初始化");
            return;
        }

        // 恢复基础属性
        PlayerState playerState = CharacterManager.instance.playerState;
        playerState.currentHealth = playerData.currentHealth;
        playerState.maxHealth = playerData.maxHealth;
        playerState.damage = playerData.damage;

        // 恢复位置和旋转
        if (playerData.position != null)
        {
            Vector3 position = playerData.position;
            CharacterManager.instance.TeleportPlayer(position);
            Debug.Log($"玩家状态恢复 - 位置: {position}, 血量: {playerData.currentHealth}/{playerData.maxHealth}");
        }
    }

    private static IEnumerator RestoreInventory(InventorySaveData inventoryData)
    {
        if (Inventory.instance == null)
        {
            Debug.LogError("无法恢复物品栏：物品栏系统未初始化");
            yield break;
        }

        // 清空当前物品栏
        Inventory.instance.items.Clear();

        // 恢复物品
        foreach (string itemName in inventoryData?.items ?? Array.Empty<string>())
        {
            Item item = Resources.Load<Item>($"Items/{itemName}");
            if (item != null)
            {
                Inventory.instance.Add(item);
                Debug.Log($"物品恢复: {itemName}");
            }
            else
            {
                Debug.LogWarning($"无法加载物品: {itemName}");
            }

            yield return null;
        }

        // 恢复装备
        if (!string.IsNullOrEmpty(inventoryData?.equippedWeapon))
        {
            Equipment weapon = Resources.Load<Equipment>($"Items/{inventoryData.equippedWeapon}");
            if (weapon != null && CharacterManager.instance.weaponSlot != null)
            {
                CharacterManager.instance.weaponSlot.AddItem(weapon);
                Debug.Log($"装备武器: {inventoryData.equippedWeapon}");
            }
            else
            {
                Debug.LogWarning($"无法加载武器: {inventoryData.equippedWeapon}");
            }
        }
    }

    private static void RestoreQuests(QuestSaveData[] questData)
    {
        if (QuestManager.instance == null)
        {
            Debug.LogError("无法恢复任务：任务系统未初始化");
            return;
        }

        foreach (var questSave in questData ?? Array.Empty<QuestSaveData>())
        {
            var task = QuestManager.instance.tasks
                .FirstOrDefault(t => t.questData?.questName == questSave.questId);

            if (task != null)
            {
                task.isCompleted = questSave.isCompleted;

                for (int i = 0;
                     i < Mathf.Min(
                         task.questData.questRequires.Count,
                         questSave.requirementProgress.Length);
                     i++)
                {
                    task.questData.questRequires[i].currentAmount = questSave.requirementProgress[i];
                }

                Debug.Log($"任务恢复: {questSave.questId} (完成: {questSave.isCompleted})");
            }
        }
    }

    private static IEnumerator RestoreEnemies(EnemySaveData[] enemyData)
    {
        Enemy[] sceneEnemies = GameObject.FindObjectsOfType<Enemy>();
        if (sceneEnemies.Length == 0)
        {
            Debug.LogWarning("场景中没有找到敌人");
            yield break;
        }

        foreach (var enemySave in enemyData ?? Array.Empty<EnemySaveData>())
        {
            Enemy enemy = sceneEnemies.FirstOrDefault(e => e.enemyID == enemySave.enemyId);
            if (enemy != null)
            {
                enemy.currentHealth = enemySave.currentHealth;
                enemy.transform.position = new Vector3(
                    enemySave.position[0],
                    enemySave.position[1],
                    enemySave.position[2]
                );
                Debug.Log($"敌人恢复: {enemySave.enemyId} HP={enemySave.currentHealth}");
            }

            yield return null;
        }
    }

    #endregion

    #region 实用方法

    public bool SaveFileExists()
    {
        return File.Exists(savePath);
    }

    public void DeleteSaveFile()
    {
        try
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Debug.Log("存档已删除");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"删除存档失败: {ex.Message}");
        }
    }
    #endregion
    public void SaveLanguageSetting(int langIndex)
    {
        PlayerPrefs.SetInt("CurrentLanguage", langIndex);
        PlayerPrefs.Save(); // 确保立即写入磁盘
    }

    public int LoadLanguageSetting()
    {
        // 默认返回0（中文）如果不存在设置
        return PlayerPrefs.GetInt("CurrentLanguage", 0); 
    }
}

