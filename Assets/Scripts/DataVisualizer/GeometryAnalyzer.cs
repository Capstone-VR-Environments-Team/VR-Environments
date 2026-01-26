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

        // Calculate Horizontal Axis
        Vector3 referenceRight = Vector3.right;

        // Project referenceRight onto the plane defined by normal 'lineDir'
        Vector3 axisH = referenceRight - (Vector3.Dot(referenceRight, lineDir) * lineDir);

        // If parallel to right then switch to forward
        if (axisH.sqrMagnitude < 0.0001f) {
            Vector3 fallback = Vector3.forward;
            axisH = fallback - (Vector3.Dot(fallback, lineDir) * lineDir);
        }

        axisH.Normalize();

        // Calculate Vertical Axis
        Vector3 axisV = Vector3.Cross(lineDir, axisH).normalized;

        // Flip of pointing down
        if (Vector3.Dot(axisV, Vector3.up) < 0) {
            axisV = -axisV;
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
                results.approach.PlaneAxisH.Add(Vector3.Dot(diff, axisH));
                results.approach.PlaneAxisV.Add(Vector3.Dot(diff, axisV));
                results.approach.Timestamps.Add(input.Timestamps[i]);
            } else {
                results.total.DistancesFromLine.Add(Vector3.Distance(point, input.LinePointB));
                results.search.DistancesFromLine.Add(Vector3.Distance(point, input.LinePointB));
                results.search.PlaneAxisH.Add(Vector3.Dot(diff, axisH));
                results.search.PlaneAxisV.Add(Vector3.Dot(diff, axisV));
                results.search.Timestamps.Add(input.Timestamps[i]);
            }
            results.total.PlaneAxisH.Add(Vector3.Dot(diff, axisH));
            results.total.PlaneAxisV.Add(Vector3.Dot(diff, axisV));
            results.total.Timestamps.Add(input.Timestamps[i]);
        }

        return results;
    }
}