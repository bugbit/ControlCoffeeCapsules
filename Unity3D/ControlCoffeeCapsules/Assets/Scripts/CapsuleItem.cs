using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CapsuleItem : MonoBehaviour
{
    [SerializeField] private Text dateText;
    [SerializeField] private JsonDateTime date;
    [SerializeField] private Button delButton;

    public UnityEvent<JsonDateTime> OnDeletedCapsuleItem;

    private void OnEnable()
    {
        delButton.onClick.AddListener(OnDelButtonClick);
    }

    private void OnDelButtonClick()
    {
        OnDeletedCapsuleItem.Invoke(date);
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
