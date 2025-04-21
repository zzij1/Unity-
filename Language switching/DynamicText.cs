using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[DisallowMultipleComponent]
public class DynamicText : MonoBehaviour
{
    [Header("Text Configuration")]
    [Tooltip("Category in database")]
    [SerializeField] private string category = "UI";
    
    [Tooltip("Key in language table")]
    [SerializeField] private string textKey;

    private Text _textComponent;
    private string _defaultText;

    void Awake()
    {
        _textComponent = GetComponent<Text>();
        _defaultText = _textComponent.text;
        UpdateText();
    }

    void OnEnable()
    {
        if (GameTextManager.Instance != null)
        {
            GameTextManager.Instance.OnLanguageChanged += UpdateText;
            UpdateText();
        }
    }

    void OnDisable()
    {
        if (GameTextManager.Instance != null)
        {
            GameTextManager.Instance.OnLanguageChanged -= UpdateText;
        }
    }

    public void UpdateText()
    {
        if (GameTextManager.Instance == null || _textComponent == null) return;

        _textComponent.text = GameTextManager.Instance.GetText(category, textKey, _defaultText);
    }
    
}