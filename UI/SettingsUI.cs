using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public GameObject settingsPanel;
    public Button closeButton;

    private void OnEnable()
    {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(CloseSettingsUI);
    }
    private void OnDisable()
    {
        closeButton.onClick.RemoveListener(CloseSettingsUI);
    }

    public void CloseSettingsUI()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
}