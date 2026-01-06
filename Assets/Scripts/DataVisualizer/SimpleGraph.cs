using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleGraph : MonoBehaviour {
    [SerializeField] private Sprite circleSprite;
    [SerializeField] private RectTransform graphContainer;
    [SerializeField] private Color lineColor = Color.green;

    public void ShowGraph(List<double> valueList) {
        // Clear old graph
        foreach (Transform child in graphContainer) Destroy(child.gameObject);

        if (valueList == null || valueList.Count == 0) return;

        float graphHeight = graphContainer.sizeDelta.y;
        float graphWidth = graphContainer.sizeDelta.x;

        // Find min/max to normalize vertical axis
        double yMax = double.MinValue;
        double yMin = double.MaxValue;
        foreach (var v in valueList) {
            if (v > yMax) yMax = v;
            if (v < yMin) yMin = v;
        }

        // Add padding
        double yRange = yMax - yMin;
        if (yRange <= 0) yRange = 1;

        GameObject lastCircle = null;

        // Skip points if data is huge to prevent lag (e.g., render every 10th point)
        int step = Mathf.Max(1, valueList.Count / 200);

        for (int i = 0; i < valueList.Count; i += step) {
            float xPosition = (i / (float)valueList.Count) * graphWidth;

            // Normalize Y (0 to 1)
            float normalizedY = (float)((valueList[i] - yMin) / yRange);
            float yPosition = normalizedY * graphHeight;

            GameObject circle = CreateCircle(new Vector2(xPosition, yPosition));

            if (lastCircle != null) {
                CreateDotConnection(lastCircle.GetComponent<RectTransform>().anchoredPosition,
                                    circle.GetComponent<RectTransform>().anchoredPosition);
            }
            lastCircle = circle;
        }
    }

    private GameObject CreateCircle(Vector2 anchoredPosition) {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        gameObject.GetComponent<Image>().color = lineColor;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(5, 5);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return gameObject;
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB) {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(lineColor.r, lineColor.g, lineColor.b, 0.5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 2f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }
}