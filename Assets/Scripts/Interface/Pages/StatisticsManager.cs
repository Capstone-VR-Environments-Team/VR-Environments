using TMPro;
using UnityEngine;

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
        averageText.SetText(statistics.Average.ToString());
        maximumText.SetText(statistics.Max.ToString());
        minimumText.SetText(statistics.Min.ToString());
        stDevText.SetText(statistics.StDev.ToString());
    }
}