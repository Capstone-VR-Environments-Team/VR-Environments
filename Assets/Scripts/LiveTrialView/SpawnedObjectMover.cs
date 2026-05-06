using UnityEngine;

public class SpawnedObjectBehavior : MonoBehaviour {
    [Tooltip("How far the object can travel from its spawn point before being destroyed.")]
    public float maxTravelDistance = 50f;

    private Vector3 spawnPosition;
    private float maxDistanceSquared;
    private bool moving;

    void Start() {
        // Record the exact point in space where this object was instantiated
        spawnPosition = transform.position;

        // Calculate the squared distance once at the start. 
        // This saves the CPU from doing heavy math in the Update loop.
        maxDistanceSquared = maxTravelDistance * maxTravelDistance;


        moving = true;
    }

    private void OnEnable()
    {
        EventBus.LastSphere += EndExperiment;
    }

    private void OnDisable()
    {
        EventBus.LastSphere -= EndExperiment;
    }

    private void EndExperiment() {
        moving = false;
    }

    void Update() {
        if (!moving) return;

        // 1. Handle Movement
        Vector3 normalizedDir = SessionManager.Instance.getMovingBackgroundDirection();
        float currentSpeed = SessionManager.Instance.getMovingBackgroundSpeed();

        transform.position += normalizedDir * currentSpeed * Time.deltaTime;

        // 2. Handle Distance-Based Destruction
        // We calculate a vector pointing from the spawn position to the current position.
        Vector3 displacement = transform.position - spawnPosition;

        // sqrMagnitude gives us the squared length of that vector.
        // Comparing squared distances is significantly faster than using Vector3.Distance().
        if (displacement.sqrMagnitude >= maxDistanceSquared) {
            Destroy(gameObject);
        }
    }
}