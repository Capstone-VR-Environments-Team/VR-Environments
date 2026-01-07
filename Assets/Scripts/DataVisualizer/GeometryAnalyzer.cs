using UnityEngine;

public static class GeometryAnalyzer {
    public static GeometryResults AnalyzeGeometry(GeometryInputData input) {
        if (input == null || input.Points == null || input.Points.Count == 0)
            return null;

        var results = new GeometryResults();

        // 1. Calculate the Line Vector (Forward Axis)
        // We normalize it to get a direction (D)
        Vector3 lineDir = (input.LinePointB - input.LinePointA).normalized;

        if (lineDir == Vector3.zero) {
            Debug.LogError("GeometryAnalyzer: Line points are identical. Cannot form a line.");
            return null;
        }

        // 2. Calculate the "Grounded" Axis (Horizontal Plane Axis)
        // We want a vector perpendicular to the Line, but also flat against the ground.
        // The Cross Product of the Line and World.Up yields a horizontal vector.
        Vector3 axisH = Vector3.Cross(lineDir, Vector3.up).normalized;

        // Edge Case: If the line is perfectly vertical (pointing straight up/down),
        // the cross product with Vector3.up is zero. In that case, we default to World.Right.
        if (axisH == Vector3.zero) {
            axisH = Vector3.right;
        }

        // 3. Calculate the "Perpendicular" Axis (Vertical Plane Axis)
        // This is perpendicular to both the Line and our new Horizontal axis.
        Vector3 axisV = Vector3.Cross(axisH, lineDir).normalized;

        // 4. Process all points
        // We use Point A as the "origin" of our calculation to find vectors
        Vector3 origin = input.LinePointA;

        foreach (var point in input.Points) {
            // Vector from Line Origin to the data point
            Vector3 v = point - origin;

            // Project v onto the line direction to find where we are "along" the line
            float t = Vector3.Dot(v, lineDir);

            // Find the closest point on the infinite line
            Vector3 closestPointOnLine = origin + (lineDir * t);

            // The "Difference Vector" is the vector from the Line to the Point.
            // This vector lies entirely on the 2D plane perpendicular to the line.
            Vector3 diff = point - closestPointOnLine;

            // --- DataSet 1: Absolute Distance ---
            results.DistancesFromLine.Add(diff.magnitude);

            // --- DataSet 2: Horizontal (Grounded) Signed Axis ---
            // Project the difference vector onto our H axis
            results.PlaneAxisH.Add(Vector3.Dot(diff, axisH));

            // --- DataSet 3: Vertical Signed Axis ---
            // Project the difference vector onto our V axis
            results.PlaneAxisV.Add(Vector3.Dot(diff, axisV));
        }

        return results;
    }
}