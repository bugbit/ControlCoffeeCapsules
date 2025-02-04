using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

using SimpleFileBrowser;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using UnityEngine.Events;

public class ControlCoffeeCapsules : MonoBehaviour
{
    const string fileNameControlCoffeeCapsulesData = "ControlCoffeeCapsules.data";
    const string fileBackUpDataPathKey = "fileBackUpDataPath";
    const string autoBackUpKey = "autoBackUp";
    const string restoreBackUpKey = "restoreBackUp";

    [Header("UIItems")]
    [SerializeField] private Text VersionText;
    [SerializeField] private InputField capsulesAddInputField;
    [SerializeField] private InputField capsulesInputField;
    [SerializeField] private InputField capsulesKitInputField;
    [SerializeField] private Text capsulesInfoText;
    [SerializeField] private Button addCapsulesButton;
    [SerializeField] private Button editCapsulesButton;
    [SerializeField] private Button editCapsulesKitButton;
    [SerializeField] private Button decalcificationButton;
    [SerializeField] private Text fileDataPathText;
    [SerializeField] private Button showBackUpPanel;
    [SerializeField] private Button showCapsulesPanel;
    // BackUp Panel
    [Header("UIItems BackUp")]
    [SerializeField] private GameObject backUpPanel;
    [SerializeField] private InputField fileSourceInputField;
    [SerializeField] private InputField fileBackUpInputField;
    [SerializeField] private Button chooseDirBackUpButton;
    [SerializeField] private Button backUpButton;
    [SerializeField] private Button restoreBackUpButton;
    [SerializeField] private Button closeBackUpButton;
    [SerializeField] private Toggle autoBackUpToggle;
    [SerializeField] private Toggle restoreBackUpToggle;
    // Capsules Panel
    [Header("UIItems Capsules")]
    [SerializeField] private GameObject capsulesPanel;
    [SerializeField] private Button closeCapsulesButton;
    [Header("Other")]
    [SerializeField] private Color[] colorsInfo;
    [Header("Debug")]
    [SerializeField] private string fileDataPath;
    [SerializeField] private string fileBackUpDataPath;
    [SerializeField] private ControlData controlData;
    [SerializeField] private int capsulesRemain;
    [SerializeField] private bool autoBackUp;
    [SerializeField] private bool restoreBackUp;

    public UnityEvent<ICollection<JsonDateTime>> ChangeCapsulesTimes;

    public async Task AddCapsuleTimeAsync(JsonDateTime time)
    {
        controlData.CapsulesTime ??= new();
        controlData.CapsulesTime.Add(time);

        await ChangeCapsules();
    }

    public async Task delCapsuleTimeAsync(JsonDateTime time)
    {
        controlData.CapsulesTime ??= new();
        controlData.CapsulesTime.Remove(time);

        await ChangeCapsules();
    }

    private void OnEnable()
    {
        addCapsulesButton.onClick.AddListener(AddCapsulesAsync);
        editCapsulesButton.onClick.AddListener(EditCapsules);
        capsulesInputField.onValueChanged.AddListener(ChangeCapsules);
        editCapsulesKitButton.onClick.AddListener(EditCapsulesKit);
        capsulesKitInputField.onValueChanged.AddListener(ChangeCapsulesKit);
        decalcificationButton.onClick.AddListener(Decalcification);
        showBackUpPanel.onClick.AddListener(ShowBackUpPanel);
        showCapsulesPanel.onClick.AddListener(ShowCapsulesPanel);
        chooseDirBackUpButton.onClick.AddListener(ShowSaveFileBackUp);
        backUpButton.onClick.AddListener(BackUp);
        restoreBackUpButton.onClick.AddListener(RestoreBackUp);
        closeBackUpButton.onClick.AddListener(HideBackUpPanel);
        autoBackUpToggle.onValueChanged.AddListener(ChangeAutoBackUp);
        restoreBackUpToggle.onValueChanged.AddListener(ChangeRestoreBackUp);
        closeCapsulesButton.onClick.AddListener(HideCapsulesPanel);
    }

    private void OnDisable()
    {
        addCapsulesButton.onClick.RemoveListener(AddCapsulesAsync);
        editCapsulesButton.onClick.RemoveListener(EditCapsules);
        capsulesInputField.onValueChanged.RemoveListener(ChangeCapsules);
        editCapsulesKitButton.onClick.RemoveListener(EditCapsulesKit);
        capsulesKitInputField.onValueChanged.RemoveListener(ChangeCapsulesKit);
        decalcificationButton.onClick.RemoveListener(Decalcification);
        showBackUpPanel.onClick.RemoveListener(ShowBackUpPanel);
        showCapsulesPanel.onClick.RemoveListener(ShowCapsulesPanel);
        chooseDirBackUpButton.onClick.RemoveListener(ShowSaveFileBackUp);
        backUpButton.onClick.RemoveListener(BackUp);
        restoreBackUpButton.onClick.RemoveListener(RestoreBackUp);
        closeBackUpButton.onClick.RemoveListener(HideBackUpPanel);
        autoBackUpToggle.onValueChanged.RemoveListener(ChangeAutoBackUp);
        restoreBackUpToggle.onValueChanged.RemoveListener(ChangeRestoreBackUp);
        closeCapsulesButton.onClick.RemoveListener(HideCapsulesPanel);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        CalcCapsulesRemain();
        ShowCapsules();
        ShowCapsulesKit();
        ShowCapsulesInfo();
    }
#endif

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        VersionText.text = Application.version;
        HideBackUpPanel();
        HideCapsulesPanel();
        DisableButtons();

        try
        {
            LoadVarsIniciales();
            ShowFileDataPath();
            ShowAutoBack();

            if (!string.IsNullOrWhiteSpace(fileDataPath))
            {
                if (!File.Exists(fileDataPath))
                {
                    if (restoreBackUp)
                        if (!string.IsNullOrWhiteSpace(fileBackUpDataPath) && File.Exists(fileBackUpDataPath))
                        {
                            yield return RestoreBackUpCorountine();
                        }
                }
                else
                {
                    Task t = LoadDataAsync();

                    while (!t.IsCompleted)
                    {
                        yield return null;
                    }
                }
            }

            ShowCapsules();
            ShowCapsulesKit();
            ShowCapsulesInfo();
        }
        finally
        {
            EnableButtons();
        }
    }

    void EnableButtons()
    {
        addCapsulesButton.interactable = true;
        editCapsulesButton.interactable = true;
        editCapsulesKitButton.interactable = true;
        decalcificationButton.interactable = true;
        chooseDirBackUpButton.interactable = true;
    }

    void DisableButtons()
    {
        addCapsulesButton.interactable = false;
        editCapsulesButton.interactable = false;
        editCapsulesKitButton.interactable = false;
        decalcificationButton.interactable = false;
        chooseDirBackUpButton.interactable = false;
    }

    void LoadVarsIniciales()
    {
        fileDataPath = Path.Combine(Application.persistentDataPath, fileNameControlCoffeeCapsulesData);
        fileBackUpDataPath = PlayerPrefs.GetString(fileBackUpDataPathKey);
        autoBackUp = PlayerPrefs.GetInt(autoBackUpKey) == 1;
        restoreBackUp = PlayerPrefs.GetInt(restoreBackUpKey) == 1;

        Debug.Log($"FileDataPath: {fileDataPath}");
        Debug.Log($"FileBackUpDataPath: {fileBackUpDataPath}");
        Debug.Log($"AutoBackUp: {autoBackUp}");
        Debug.Log($"RestoreBackUp: {restoreBackUp}");
    }

    void ShowFileDataPath()
    {
        fileSourceInputField.text = fileDataPath;
        fileBackUpInputField.text = fileBackUpDataPath;
    }

    void ShowAutoBack()
    {
        autoBackUpToggle.isOn = autoBackUp;
        restoreBackUpToggle.isOn = restoreBackUp;
    }

    async Task LoadDataAsync()
    {
        string jsonString;

        try
        {
            using (StreamReader sr = new StreamReader(fileDataPath))
            {
                jsonString = await sr.ReadToEndAsync();
                sr.Close();
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);

            return;
        }

        ControlData sdpc = JsonUtility.FromJson<ControlData>(jsonString);

        controlData = sdpc;
        CalcCapsulesRemain();
    }

    async Task<bool> SaveDataAsync()
    {
        if (string.IsNullOrWhiteSpace(fileDataPath))
            return false;

        string jsonString = JsonUtility.ToJson(controlData);
        bool ok;

        try
        {
            using (StreamWriter sw = new StreamWriter(fileDataPath))
            {
                await sw.WriteLineAsync(jsonString);
                sw.Close();
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);

            ok = false;
        }

        ok = true;
        if (autoBackUp)
            BackUp();

        return ok;
    }

    private async Task<bool> SaveDataAsync(string file)
    {
        string jsonString = JsonUtility.ToJson(controlData);

        try
        {
            using (StreamWriter sw = new StreamWriter(file))
            {
                await sw.WriteLineAsync(jsonString);
                sw.Close();
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);

            return false;
        }

        return true;
    }

    void CalcCapsulesRemain()
    {
        capsulesRemain = controlData is null ? 0 : Mathf.Clamp(controlData.CapsulesToKit - controlData.Capsules, 0, controlData.CapsulesToKit);
    }

    void ShowCapsules()
    {
        capsulesInputField.text = controlData is null ? "" : controlData.Capsules.ToString();
    }

    void ShowCapsulesKit()
    {
        capsulesKitInputField.text = controlData is null ? "" : controlData.CapsulesToKit.ToString();
    }

    void ShowCapsulesInfo()
    {
        if (controlData is null)
            capsulesInfoText.text = "";
        else
        {
            float percent = controlData.CapsulesToKit == 0 ? 0 : Mathf.Clamp((100 * controlData.Capsules / controlData.CapsulesToKit * 100) / 100f, 0, 100);
            int index = Mathf.Clamp((int)percent / (100 / colorsInfo.Length), 0, colorsInfo.Length - 1);
            var strBuilder = new StringBuilder();

            if (controlData is not null)
            {
                strBuilder.Append("Left: ");
                strBuilder.Append(capsulesRemain);
                strBuilder.Append(" ");
                strBuilder.Append(percent);
                strBuilder.Append("%");
                strBuilder.AppendLine();
                if (controlData.CapsulesTime is not null && controlData.CapsulesTime.Count > 0)
                {
                    strBuilder.Append("Last capsule: ");
                    strBuilder.Append(((DateTime)controlData.CapsulesTime[^1]).ToString("dd/MM/yyyy"));
                }
            }
            //capsulesInfoText.text = controlData is null ? "" : string.Format("Quedan: {0} {1}%", capsulesRemain, percent);
            capsulesInfoText.text = strBuilder.ToString();
            capsulesInfoText.color = colorsInfo[index];
        }
    }

    async void AddCapsulesAsync()
    {
        int capsulesToAdd = int.Parse(capsulesAddInputField.text);

        controlData ??= new();

        controlData.Capsules += capsulesToAdd;

        await AddCapsuleTimeAsync((JsonDateTime)DateTime.Now);
    }

    private async Task ChangeCapsules()
    {
        ChangeCapsulesTimes.Invoke(controlData.CapsulesTime ?? new List<JsonDateTime>());
        await SaveDataAsync();
        CalcCapsulesRemain();

        ShowCapsules();
        ShowCapsulesInfo();
    }

    void EditCapsules()
    {
        capsulesInputField.interactable = !capsulesInputField.interactable;
    }

    private async void ChangeCapsules(string capsulesTxt)
    {

        int capsules = int.Parse(capsulesTxt);

        controlData ??= new();
        controlData.Capsules = capsules;

        await ChangeCapsules();
    }

    void EditCapsulesKit()
    {
        capsulesKitInputField.interactable = !capsulesKitInputField.interactable;
    }

    async void ChangeCapsulesKit(string capsulesTxt)
    {

        int capsules = int.Parse(capsulesTxt);

        controlData ??= new();
        controlData.CapsulesToKit = capsules;

        CalcCapsulesRemain();
        await SaveDataAsync();

        ShowCapsulesKit();
        ShowCapsulesInfo();
    }

    async void Decalcification()
    {
        if (controlData is null)
            return;

        controlData.Capsules = 0;
        controlData.CapsulesTime?.Clear();

        await ChangeCapsules();
    }

    void ShowBackUpPanel()
    {
        backUpPanel.SetActive(true);
    }

    void HideBackUpPanel()
    {
        backUpPanel.SetActive(false);
    }

    void ShowSaveFileBackUp()
    {
        StartCoroutine(ShowSaveFileBackUpCoroutine());
    }

    IEnumerator ShowSaveFileBackUpCoroutine()
    {
        string fileBackUpDataDir = string.IsNullOrWhiteSpace(fileBackUpDataPath) ? null : Path.GetDirectoryName(fileBackUpDataPath);

        FileBrowser.SetFilters(true, new FileBrowser.Filter("data", ".data"));
        FileBrowser.SetDefaultFilter(".data");

        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files, false, fileBackUpDataDir, fileBackUpDataPath, "Aceptar");

        if (FileBrowser.Success)
        {
            fileBackUpInputField.text = fileBackUpDataPath = FileBrowser.Result[0];
            PlayerPrefs.SetString(fileBackUpDataPathKey, fileBackUpDataPath);
        }
    }

    void BackUp()
    {
        //StartCoroutine(BackUpCorountine());
        if (string.IsNullOrWhiteSpace(fileBackUpDataPath))
            return;

        string jsonString = JsonUtility.ToJson(controlData);

        FileBrowserHelpers.WriteTextToFile(fileBackUpDataPath, jsonString);
    }

    void RestoreBackUp()
    {
        StartCoroutine(RestoreBackUpCorountine());
    }

    IEnumerator RestoreBackUpCorountine()
    {
        if (string.IsNullOrWhiteSpace(fileBackUpDataPath) || !FileBrowserHelpers.FileExists(fileBackUpDataPath))
            yield break;

        string jsonString = FileBrowserHelpers.ReadTextFromFile(fileBackUpDataPath);

        ControlData sdpc = JsonUtility.FromJson<ControlData>(jsonString);

        controlData = sdpc;

        CalcCapsulesRemain();
        ShowCapsules();
        ShowCapsulesInfo();
        ShowCapsulesKit();

        var t2 = SaveDataAsync();

        while (!t2.IsCompleted)
            yield return null;
    }

    private void ChangeAutoBackUp(bool value)
    {
        autoBackUp = value;
        PlayerPrefs.SetInt(autoBackUpKey, autoBackUp ? 1 : 0);
    }

    private void ChangeRestoreBackUp(bool value)
    {
        restoreBackUp = value;
        PlayerPrefs.SetInt(restoreBackUpKey, restoreBackUp ? 1 : 0);
    }

    void ShowCapsulesPanel()
    {
        capsulesPanel.SetActive(true);
    }

    void HideCapsulesPanel()
    {
        capsulesPanel.SetActive(false);
    }
}
