using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    public GameObject defaultPosition;
    public string savePath;

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/save.json";
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        defaultPosition=GameObject.FindGameObjectWithTag("DefaultSpawnPoint");
        if (scene.name == "MainMenu")
        {
            return;
        }
        StartCoroutine(EnsureSceneLoaded(scene));
        
    }
    
    private IEnumerator EnsureSceneLoaded(Scene scene)
{
    // 等待一帧确保所有GameObject都已初始化
    yield return null;
    
    // 查找默认出生点
    defaultPosition = GameObject.FindGameObjectWithTag("DefaultSpawnPoint");
    while (defaultPosition == null)
    {
        Debug.Log("等待默认出生点初始化...");
        defaultPosition = GameObject.FindGameObjectWithTag("DefaultSpawnPoint");
        yield return null;
    }
    
    // 确保CharacterManager已初始化
    while (CharacterManager.instance == null || CharacterManager.instance.player == null)
    {
        Debug.Log("等待CharacterManager初始化...");
        yield return null;
    }
    
    // 初始化玩家位置（改为协程）
    yield return SetupPlayerPosition(scene);
    yield return new WaitForEndOfFrame(); // 确保物理引擎更新位置
    yield return SceneFader.Instance.FadeIn();
    
}

// 改为协程
private IEnumerator SetupPlayerPosition(Scene scene)
{
    // 人物设置为存档位置
    if (SaveSystem.instance.isLoading)
    {
        Debug.Log("人物设置为存档位置");
        string json = File.ReadAllText(savePath);
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
        CharacterManager.instance.TeleportPlayer(saveData.player.position);
        SaveSystem.instance.isLoading = false;
        yield return null; // 确保位置更新完成
    }
    // 人物设置为默认点
    else if (scene.name == "Scene1" && !SaveSystem.instance.isLoading && 
             (PortalManager.Instance == null || PortalManager.Instance.lastUsedPortalID == null))
    {
        Debug.Log("人物设置为默认点");
        CharacterManager.instance.TeleportPlayer(defaultPosition.transform.position);
        yield return null;
    }
    // 到传送门位置
    else if (!SaveSystem.instance.isLoading && 
             PortalManager.Instance != null && 
             PortalManager.Instance.lastUsedPortalID != null)
    {
        Debug.Log("人物设置为传送门位置");
        var portal = FindPortal(PortalManager.Instance.lastUsedPortalID);
        if (portal != null)
        {
            CharacterManager.instance.TeleportPlayer(portal.spawnPoint.position);
            yield return null;
        }
    }
    
    CharacterManager.instance.player.gameObject.SetActive(true);
}
    private Portal FindPortal(string portalID)
    {
        var portal = FindObjectsOfType<Portal>(true).FirstOrDefault(p => p.portalID == portalID);
        if (portal == null) Debug.LogError($"未找到ID为 {portalID} 的传送门");
        return portal;
    }


}
