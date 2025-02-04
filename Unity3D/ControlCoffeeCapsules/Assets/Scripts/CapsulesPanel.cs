using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CapsulesPanel : MonoBehaviour
{
    [Header("UIItems")]
    [SerializeField] private Transform contentCapsulesList;
    [SerializeField] private GameObject capsuleItemPrefab;

    [Header("Debug")]
    [SerializeField] private JsonDateTime[] capsulesTime;
    [SerializeField] private bool onValidateRandomTime = true;

    public void SetCapsulesTimes(ICollection<JsonDateTime> capsulesTimes)
    {
        SortCapsules(capsulesTimes);
        UpdateCapsules();
    }

    private void OnValidate()
    {
        if (onValidateRandomTime)
        {
            if (capsuleItemPrefab.TryGetComponent<RectTransform>(out var rect))
                Debug.Log($"Item size: {rect.rect.width}x{rect.rect.height}");
            var capsulesTimeList = new List<JsonDateTime>();

            for (int i = 0; i <= 30; i++)
            {
                var date = new DateTime(Random.Range(2024, 2025), Random.Range(1, 12), Random.Range(1, 28), Random.Range(0, 23), Random.Range(0, 59), Random.Range(0, 59));

                capsulesTimeList.Add(date);
            }

            SetCapsulesTimes(capsulesTimeList);
        }
    }

    private void SortCapsules(ICollection<JsonDateTime> capsulesTimes)
    {
        var _capsulesTime = capsulesTimes.ToArray();

        Array.Sort(_capsulesTime);
        capsulesTime = _capsulesTime;
    }

    private void UpdateCapsules()
    {
        var transform = contentCapsulesList.transform;
        var i = 0;

        for (; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var capsuleTime = capsulesTime[i];

            if (child.TryGetComponent<CapsuleItem>(out var item))
                item.SetDate(capsuleTime);
        }
        for (; i < capsulesTime.Length; i++)
        {
            var capsuleTime = capsulesTime[i];
            var child = Instantiate(capsuleItemPrefab, transform);

            if (child.TryGetComponent<CapsuleItem>(out var item))
                item.SetDate(capsuleTime);
        }
        if (capsuleItemPrefab.TryGetComponent<RectTransform>(out var rectItem))
            if (contentCapsulesList.TryGetComponent<RectTransform>(out var rectContent))
                rectContent.sizeDelta = new Vector2(rectContent.sizeDelta.x, rectItem.rect.height * capsulesTime.Length);

    }
}
