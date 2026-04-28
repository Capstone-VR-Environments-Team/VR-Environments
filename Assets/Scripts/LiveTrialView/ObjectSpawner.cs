using UnityEngine;

public class ObjectSpawner : MonoBehaviour {
    [Header("Spawn References")]
    public GameObject objectToSpawn;
    public Transform target;

    [Header("Positioning Settings")]
    public float spawnDistance = 10f;
    public float minMissRadius = .5f;
    public float maxMissRadius = 3f;

    private float spawnTimer = 0f;

    private bool spawning;

    void Start() {
        spawning = SessionManager.Instance.GetMovingBackground();
        EventBus.LastSphere += () => spawning = false;
    }

    void Update()
    {
        if (!spawning) return;

        int objPerMin = SessionManager.Instance.getMovingBackgroundQuantity();
        // Prevent division by zero if someone sets the rate to 0
        if (objPerMin <= 0f) return;

        // Calculate how many seconds should pass between each spawn
        float secondsBetweenSpawns = 60f / objPerMin;

        // Advance the timer
        spawnTimer += Time.deltaTime;

        // If enough time has passed, spawn an object
        if (spawnTimer >= secondsBetweenSpawns)
        { 
            // Subtract the interval. This keeps any "leftover" time, 
            // ensuring the spawn rate is perfectly spread out over time.
            spawnTimer -= secondsBetweenSpawns;

            SpawnObject();
        }
    }

    private void SpawnObject() {
        Vector3 direction = SessionManager.Instance.getMovingBackgroundDirection();

        if (direction == Vector3.zero) return;

        Vector3 normalizedDir = direction.normalized;
        Vector3 baseSpawnPos = target.position - (normalizedDir * spawnDistance);

        Vector2 randomCircle = Random.insideUnitCircle.normalized;
        float randomMagnitude = Random.Range(minMissRadius, maxMissRadius);
        Vector2 randomOffset2D = randomCircle * randomMagnitude;

        Quaternion spawnOrientation = Quaternion.LookRotation(normalizedDir);
        Vector3 perpendicularOffset = spawnOrientation * new Vector3(randomOffset2D.x, randomOffset2D.y, 0f);

        Vector3 finalSpawnPos = baseSpawnPos + perpendicularOffset;

        // Instantiate with the orientation matching the movement direction
        GameObject newObject = Instantiate(objectToSpawn, finalSpawnPos, spawnOrientation);
        newObject.transform.localScale = SessionManager.Instance.getMovingBackgroundSize();

        string color = SessionManager.Instance.getMovingBackgroundColor();
        if (!color.StartsWith("#")) {
            color = "#" + color;
        }

        MeshRenderer meshRenderer = newObject.GetComponent<MeshRenderer>();
        meshRenderer.material.color = ColorUtility.TryParseHtmlString(color, out Color parsedColor) ? parsedColor : Color.black;
    }
}