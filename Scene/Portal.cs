using System.Collections;
using UnityEngine;

public class Portal : Interactable
{
    [Header("Portal Settings")] 
    public string targetSceneName;
    public string portalID; // 当前传送门ID
    public string targetPortalID; // 目标传送门ID
    public Transform spawnPoint;

    public override void Interact()
    {
        StartCoroutine(TransitionRoutine());
    }

    private IEnumerator TransitionRoutine()
    {
        // 设置目标传送门ID
        PortalManager.Instance.lastUsedPortalID = targetPortalID;
        
        // 使用SceneLoader加载场景
        SceneLoader.Instance.LoadScene(targetSceneName);
        yield return null;
    }
}