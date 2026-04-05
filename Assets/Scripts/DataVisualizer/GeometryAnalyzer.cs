using UnityEngine;

public static class GeometryAnalyzer {
    public static GeometryResults AnalyzeGeometry(GeometryInputData input) {
        if (input == null || input.Points == null || input.Points.Count == 0)
            return null;

        var results = new GeometryResults();

        bool requiresLine = input.Mode != AnalysisMode.PREVIOUSSPHERE;

        // Calculate the line vector only for approach/search modes.
        Vector3 lineDir = Vector3.zero;
        if (requiresLine) {
            lineDir = (input.LinePointB - input.LinePointA).normalized;

            if (lineDir == Vector3.zero) {
                Debug.LogError("GeometryAnalyzer: Line points are identical. Cannot form a line.");
                Debug.LogError("Point A: " + input.LinePointA.ToString() + " Point B: " + input.LinePointB.ToString());
                return null;
            }
        }

        // Process all points
        Vector3 origin = input.LinePointA;

        for(int i = 0; i < input.Points.Count; i++) {
            var point = input.Points[i];
            Vector3 totalDiff;
            float totalDistance;

            if (input.Mode == AnalysisMode.PREVIOUSSPHERE) {
                Vector3 sphereDiff = point - input.LinePointA;
                totalDiff = sphereDiff;
                totalDistance = sphereDiff.magnitude;

                results.previousSphere.DistancesFromLine.Add(totalDistance);
                results.previousSphere.DeviationsX.Add(sphereDiff.x);
                results.previousSphere.DeviationsY.Add(sphereDiff.y);
                results.previousSphere.DeviationsZ.Add(sphereDiff.z);
                results.previousSphere.Timestamps.Add(input.Timestamps[i]);
            } else {
                // Vector from Line Origin to the data point
                Vector3 v = point - origin;

                // Project v onto the line direction
                float t = Vector3.Dot(v, lineDir);
                Vector3 closestPointOnLine = origin + (lineDir * t);

                Vector3 diff = point - closestPointOnLine;
                totalDiff = diff;
                totalDistance = diff.magnitude;

                if (input.Mode == AnalysisMode.LINETOTARGET) {
                    // Perpendicular distance to the path
                    results.approach.DistancesFromLine.Add(diff.magnitude);
                    results.approach.DeviationsX.Add(diff.x);
                    results.approach.DeviationsY.Add(diff.y);
                    results.approach.DeviationsZ.Add(diff.z);
                    results.approach.Timestamps.Add(input.Timestamps[i]);
                } else {
                    totalDistance = Vector3.Distance(point, input.LinePointB);
                    results.search.DistancesFromLine.Add(totalDistance);
                    results.search.DeviationsX.Add(diff.x);
                    results.search.DeviationsY.Add(diff.y);
                    results.search.DeviationsZ.Add(diff.z);
                    results.search.Timestamps.Add(input.Timestamps[i]);
                }
            }

            results.total.DistancesFromLine.Add(totalDistance);
            results.total.DeviationsX.Add(totalDiff.x);
            results.total.DeviationsY.Add(totalDiff.y);
            results.total.DeviationsZ.Add(totalDiff.z);
            results.total.Timestamps.Add(input.Timestamps[i]);
        }

        return results;
    }
}