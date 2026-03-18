using UnityEngine;

public static class GeometryAnalyzer {
    public static GeometryResults AnalyzeGeometry(GeometryInputData input) {
        if (input == null || input.Points == null || input.Points.Count == 0)
            return null;

        var results = new GeometryResults();

        // Calculate the Line Vector
        Vector3 lineDir = (input.LinePointB - input.LinePointA).normalized;

        if (lineDir == Vector3.zero) {
            Debug.LogError("GeometryAnalyzer: Line points are identical. Cannot form a line.");
            Debug.LogError("Point A: " + input.LinePointA.ToString() + " Point B: " + input.LinePointB.ToString());
            return null;
        }

        // Process all points
        Vector3 origin = input.LinePointA;

        for(int i = 0; i < input.Points.Count; i++) {
            var point = input.Points[i];
            // Vector from Line Origin to the data point
            Vector3 v = point - origin;

            // Project v onto the line direction
            float t = Vector3.Dot(v, lineDir);
            Vector3 closestPointOnLine = origin + (lineDir * t);

            Vector3 diff = point - closestPointOnLine;

            if (input.Mode == AnalysisMode.LineToTarget) {
                // Perpendicular distance to the path
                results.total.DistancesFromLine.Add(diff.magnitude);
                results.approach.DistancesFromLine.Add(diff.magnitude);
                results.approach.DeviationsX.Add(diff.x);
                results.approach.DeviationsY.Add(diff.y);
                results.approach.DeviationsZ.Add(diff.z);
                results.approach.Timestamps.Add(input.Timestamps[i]);
            } else {
                results.total.DistancesFromLine.Add(Vector3.Distance(point, input.LinePointB));
                results.search.DistancesFromLine.Add(Vector3.Distance(point, input.LinePointB));
                results.search.DeviationsX.Add(diff.x);
                results.search.DeviationsY.Add(diff.y);
                results.search.DeviationsZ.Add(diff.z);
                results.search.Timestamps.Add(input.Timestamps[i]);
            }
            results.total.DeviationsX.Add(diff.x);
            results.total.DeviationsY.Add(diff.y);
            results.total.DeviationsZ.Add(diff.z);
            results.total.Timestamps.Add(input.Timestamps[i]);
        }

        return results;
    }
}