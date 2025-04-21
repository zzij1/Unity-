using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExperienceUI : MonoBehaviour {
    [SerializeField] private Image experienceBar;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // 跨场景不销毁
        SceneManager.sceneLoaded += OnSceneLoaded;
        experienceBar.fillAmount = 0;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 主菜单隐藏，其他场景显示
        gameObject.SetActive(scene.name != "MainMenu");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        UpdateExperience();
    }

    private void UpdateExperience()
    {
        if (CharacterManager.instance?.playerState == null) return;
        var state = CharacterManager.instance.playerState;
        float ratio = (float)state.currentExp / state.expToNextLevel;
        experienceBar.fillAmount = ratio;
    }
}