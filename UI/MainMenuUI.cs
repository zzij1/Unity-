using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private float continueDelay = 0.5f;

    private void Start()
    {
        continueButton.interactable = SaveSystem.instance.SaveFileExists();

        newGameButton.onClick.AddListener(StartNewGame);
        continueButton.onClick.AddListener(ContinueGame);
        tutorialButton.onClick.AddListener(ToggleTutorial);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void StartNewGame()
    {
        PortalManager.ClearPortalData();
        SaveSystem.instance.DeleteSaveFile();
        SceneLoader.Instance.LoadScene("Scene1");
    }



    private void ContinueGame()
    {
        if (!SaveSystem.instance.SaveFileExists()) return;
        
        StartCoroutine(ContinueGameRoutine());
    }

    private IEnumerator ContinueGameRoutine()
    {
        
        
        // 加载游戏数据
        yield return SaveSystem.instance.LoadGame();
        // 等待场景完全加载
        yield return new WaitForSeconds(continueDelay);
    }

    private void ToggleTutorial()
    {
        tutorialPanel.SetActive(!tutorialPanel.activeSelf);
    }

    private void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
}