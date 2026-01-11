using TMPro;
using UnityEngine;
using System;

[System.Serializable]
public class StatisticsManager {

    [SerializeField] private TMP_Text averageText;
    public TMP_Text AverageText => averageText;

    [SerializeField] private TMP_Text maximumText;
    public TMP_Text MaximumText => maximumText;

    [SerializeField] private TMP_Text minimumText;
    public TMP_Text MinimumText => minimumText;

    [SerializeField] private TMP_Text stDevText;
    public TMP_Text StDevText => stDevText;

    public void SetStatistics(Statistics statistics)
    {
        averageText.SetText(Math.Round(statistics.Average, 4).ToString());
        maximumText.SetText(Math.Round(statistics.Max, 4).ToString());
        minimumText.SetText(Math.Round(statistics.Min, 4).ToString());
        stDevText.SetText(Math.Round(statistics.StDev, 4).ToString());
    }
}