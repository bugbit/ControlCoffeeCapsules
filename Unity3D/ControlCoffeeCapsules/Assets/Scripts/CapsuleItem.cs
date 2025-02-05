using TMPro;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CapsuleItem : MonoBehaviour
{
    [SerializeField] private Text dateText;
    [SerializeField] private TMP_InputField dateInputField;
    [SerializeField] private JsonDateTime date;
    [SerializeField] private Button delButton;
    [SerializeField] private Button editButton;

    public UnityEvent<JsonDateTime> OnDeletedCapsuleItem;

    private void OnEnable()
    {
        delButton.onClick.AddListener(OnDelButtonClick);
        editButton.onClick.AddListener(OnEditButtonClick);
        dateInputField.onSubmit.AddListener(s => Debug.Log($"onSubmit {s}"));
        dateInputField.onEndEdit.AddListener(s => Debug.Log($"onEndEdit {s}"));
        dateInputField.onValueChanged.AddListener(s => Debug.Log($"onValueChanged {s}"));
    }

    private void OnDisable()
    {
        delButton.onClick.RemoveListener(OnDelButtonClick);
        editButton.onClick.RemoveListener(OnEditButtonClick);
    }

    private void OnDelButtonClick()
    {
        OnDeletedCapsuleItem.Invoke(date);
    }

    private void OnEditButtonClick()
    {
        dateInputField.text = dateText.text;
        EditDate();
    }

    private void EditDate()
    {
        dateText.gameObject.SetActive(false);
        dateInputField.gameObject.SetActive(true);
        dateInputField.Select();
        editButton.gameObject.SetActive(false);
        delButton.gameObject.SetActive(false);
    }

    public void SetDate(JsonDateTime date)
    {
        this.date = date;
        RefreshDate();
    }

    public void RefreshDate()
    {
        dateText.text = ((DateTime)date).ToString("G");
    }

    private void OnValidate()
    {
        RefreshDate();
    }
}
