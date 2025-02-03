using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CapsulesPanel : MonoBehaviour
{
    [Header("UIItems")]
    [SerializeField] private Transform contentCapsulesList;

    [Header("Debug")]
    [SerializeField] private JsonDateTime[] capsulesTime;
    [SerializeField] private bool onValidateRandomTime = true;

    public void SetCapsulesTimes(ICollection<JsonDateTime> capsulesTimes)
    {
        //capsulesTime.sort
    }

    private void OnValidate()
    {
        if (onValidateRandomTime)
        {
            var capsulesTimeList = new List<JsonDateTime>();

            for (int i = 0; i <= 30; i++)
            {
                var date = new DateTime(Random.Range(2024, 2025), Random.Range(1, 12), Random.Range(1, 28), Random.Range(0, 23), Random.Range(0, 59), Random.Range(0, 59));

                capsulesTimeList.Add(date);
            }

            capsulesTime = capsulesTimeList.ToArray();
        }
    }
}
