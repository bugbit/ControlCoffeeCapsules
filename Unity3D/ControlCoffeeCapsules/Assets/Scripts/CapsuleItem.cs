using System;
using UnityEngine;
using UnityEngine.UI;

public class CapsuleItem : MonoBehaviour
{
    [SerializeField] private Text dateText;
    [SerializeField] private JsonDateTime date;

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
