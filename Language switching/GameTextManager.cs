using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;
using UnityEngine;
using UnityEngine.UI;

public class GameTextManager : MonoBehaviour
{
    public static GameTextManager Instance { get; private set; }
    public event Action OnLanguageChanged;

    [Header("UI Bindings")]
    [SerializeField] private Dropdown languageDropdown;

    public string CurrentLanguage { get; private set; } = "zh";
    private Dictionary<string, Dictionary<string, string>> _textCache = new Dictionary<string, Dictionary<string, string>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    async void InitializeSystem()
    {
        CurrentLanguage = PlayerPrefs.GetString("SelectedLanguage", "zh");
        InitLanguageSelector();
        await LoadLanguageData(CurrentLanguage);
    }

    void InitLanguageSelector()
    {
        if (languageDropdown == null) return;

        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(new List<string> { "中文", "English" });
        languageDropdown.value = CurrentLanguage == "zh" ? 0 : 1;
        languageDropdown.onValueChanged.AddListener(async index =>
        {
            await SwitchLanguage(index == 0 ? "zh" : "en");
        });
    }

    public async Task SwitchLanguage(string langCode)
    {
        if (CurrentLanguage != langCode)
        {
            CurrentLanguage = langCode;
            PlayerPrefs.SetString("SelectedLanguage", langCode);

            await LoadLanguageData(langCode);
            OnLanguageChanged?.Invoke();
            RefreshAllText();
        }
    }

    async Task LoadLanguageData(string langCode)
    {
        try
        {
            using (var conn = await DatabaseConnector.GetConnection())
            {
                var langId = await GetLanguageId(langCode, conn);
                await LoadCategory("UI", langId, conn);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Load language data failed: {ex}");
        }
    }

    async Task<int> GetLanguageId(string langCode, MySqlConnection conn)
    {
        var cmd = new MySqlCommand(
            "SELECT lang_id FROM AppLanguages WHERE lang_code=@code", 
            conn
        );
        cmd.Parameters.AddWithValue("@code", langCode);
        var result = await cmd.ExecuteScalarAsync();
        return result != null ? Convert.ToInt32(result) : throw new Exception($"Language code {langCode} not found");
    }

    async Task LoadCategory(string category, int langId, MySqlConnection conn)
    {
        var cmd = new MySqlCommand(@"
            SELECT text_key, text_value 
            FROM GameText 
            WHERE lang_id=@langId
            AND category_id=(
                SELECT category_id 
                FROM TextCategories 
                WHERE category_name=@category
            )", conn);

        cmd.Parameters.AddWithValue("@langId", langId);
        cmd.Parameters.AddWithValue("@category", category);

        var categoryDict = new Dictionary<string, string>();
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                categoryDict[reader.GetString(0)] = reader.GetString(1);
            }
        }

        _textCache[category] = categoryDict;
    }

    public string GetText(string category, string key, string fallback)
    {
        if (category != "UI") return fallback;

        if (_textCache.TryGetValue(category, out var dict) && dict.TryGetValue(key, out var value))
            return value;

        return fallback;
    }

    void RefreshAllText()
    {
        foreach (var text in FindObjectsOfType<DynamicText>())
        {
            text.UpdateText();
        }
    }
}