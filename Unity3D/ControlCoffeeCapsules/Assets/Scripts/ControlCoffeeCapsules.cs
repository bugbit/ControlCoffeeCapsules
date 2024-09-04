using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ControlCoffeeCapsules : MonoBehaviour
{
    [Header("UIItems")]
    [SerializeField] private InputField capsulesAddInputField;
    [SerializeField] private InputField capsulesInputField;
    [SerializeField] private InputField capsulesKitInputField;
    [SerializeField] private Text capsulesInfoText;
    [SerializeField] private Button addCapsulesButton;
    [SerializeField] private Button editCapsulesButton;
    [SerializeField] private Button editCapsulesKitButton;
    [SerializeField] private Button decalcificationButtonButton;
    [SerializeField] private Text fileDataPathText;
    [SerializeField] private GameObject backUpPanel;
    [Header("Other")]
    [SerializeField] private Color[] colorsInfo;
    [Header("Debug")]
    [SerializeField] private string fileDataPath;
    [SerializeField] private ControlData controlData;
    [SerializeField] private int capsulesRemain;

    private void OnEnable()
    {
        addCapsulesButton.onClick.AddListener(AddCapsules);
        editCapsulesButton.onClick.AddListener(EditCapsules);
        capsulesInputField.onValueChanged.AddListener(ChangeCapsules);
        editCapsulesKitButton.onClick.AddListener(EditCapsulesKit);
        capsulesKitInputField.onValueChanged.AddListener(ChangeCapsulesKit);
        decalcificationButtonButton.onClick.AddListener(Decalcification);
    }

    private void OnDisable()
    {
        addCapsulesButton.onClick.RemoveListener(AddCapsules);
        editCapsulesButton.onClick.RemoveListener(EditCapsules);
        capsulesInputField.onValueChanged.RemoveListener(ChangeCapsules);
        editCapsulesKitButton.onClick.RemoveListener(EditCapsulesKit);
        capsulesKitInputField.onValueChanged.RemoveListener(ChangeCapsulesKit);
        decalcificationButtonButton.onClick.RemoveListener(Decalcification);
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
        HiddenPanelsStart();
        DisableButtons();

        try
        {
            SetFileDataPath();

            ShowFileDataPath();

            var task = LoadDataAsync();

            while (!task.IsCompleted)
            {
                yield return null;
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

    private void HiddenPanelsStart()
    {
        backUpPanel.SetActive(false);
    }

    void EnableButtons()
    {
        addCapsulesButton.interactable = true;
        editCapsulesButton.interactable = true;
        editCapsulesKitButton.interactable = true;
        decalcificationButtonButton.interactable = true;
    }

    void DisableButtons()
    {
        addCapsulesButton.interactable = false;
        editCapsulesButton.interactable = false;
        editCapsulesKitButton.interactable = false;
        decalcificationButtonButton.interactable = false;
    }

    void SetFileDataPath()
    {
        fileDataPath = Application.persistentDataPath + "/ControlCoffeeCapsules.data";

        Debug.Log($"FileDataPath: {fileDataPath}");
    }

    void ShowFileDataPath()
    {
        fileDataPathText.text = fileDataPath;
    }

    async Task LoadDataAsync()
    {
        string jsonString;
        using (StreamReader sr = new StreamReader(fileDataPath))
        {
            jsonString = await sr.ReadToEndAsync();
            sr.Close();
        }

        ControlData sdpc = JsonUtility.FromJson<ControlData>(jsonString);

        controlData = sdpc;
        CalcCapsulesRemain();
    }

    async Task SaveDataAsync()
    {
        string jsonString = JsonUtility.ToJson(controlData);

        using (StreamWriter sw = new StreamWriter(fileDataPath))
        {
            await sw.WriteLineAsync(jsonString);
            sw.Close();
        }
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

            capsulesInfoText.text = controlData is null ? "" : string.Format("Quedan: {0} {1}%", capsulesRemain, percent);
            capsulesInfoText.color = colorsInfo[index];
        }
    }

    async void AddCapsules()
    {
        int capsulesToAdd = int.Parse(capsulesAddInputField.text);

        controlData ??= new();

        controlData.Capsules += capsulesToAdd;

        await ChangeCapsules();
    }

    private async Task ChangeCapsules()
    {
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

        await ChangeCapsules();
    }
}
