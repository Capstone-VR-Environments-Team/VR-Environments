using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TargetHitSequenceBuilder {
    private const float LocationMatchTolerance = 0.02f;

    public static List<HitEvent> BuildWithInitialTarget(
        List<HitEvent> recordedHits,
        IReadOnlyList<Vector3> configuredTargets,
        double initialTime) {

        List<HitEvent> normalizedHits = recordedHits != null
            ? recordedHits.OrderBy(hit => hit.time).Select(hit => new HitEvent(hit.time, hit.targetId, hit.location)).ToList()
            : new List<HitEvent>();

        if (configuredTargets == null || configuredTargets.Count == 0) {
            return normalizedHits;
        }

        Vector3 firstConfiguredTarget = configuredTargets[0];
        if (normalizedHits.Count > 0 && IsMatchingLocation(normalizedHits[0].location, firstConfiguredTarget)) {
            return normalizedHits;
        }

        normalizedHits.Insert(0, new HitEvent(initialTime, 1, firstConfiguredTarget));
        return normalizedHits;
    }

    private static bool IsMatchingLocation(Vector3 lhs, Vector3 rhs) {
        float toleranceSquared = LocationMatchTolerance * LocationMatchTolerance;
        return (lhs - rhs).sqrMagnitude <= toleranceSquared;
    }
}