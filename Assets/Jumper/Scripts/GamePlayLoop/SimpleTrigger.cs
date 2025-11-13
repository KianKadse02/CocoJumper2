using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SimpleTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        StartZone,
        FinishZone,
        DeathZone
    }

    [Header("Trigger Type")]
    public TriggerType triggerType = TriggerType.StartZone;

    [Header("Visual")]
    public Color gizmoColor = Color.green;

    [Header("References")]
    [Tooltip("Optional — will FindObjectOfType if not set.")]
    [SerializeField] private SimpleGameLoop gameLoop;

    void Start()
    {
        // Make sure collider is trigger
        var col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;

        // Find game loop if not assigned
        if (gameLoop == null)
        {
#if UNITY_2023_1_OR_NEWER
            // Newer Unity: use the non-obsolete API
            gameLoop = Object.FindFirstObjectByType<SimpleGameLoop>();
#else
            // Fallback for older Unity versions
            gameLoop = FindObjectOfType<SimpleGameLoop>();
#endif
        }

        if (gameLoop == null)
            Debug.LogWarning("SimpleTrigger: No SimpleGameLoop found in scene.");
    }

    void OnTriggerEnter(Collider other)
    {
        // Accept either tag "Player" OR the player transform assigned in SimpleGameLoop
        bool isPlayer = other.CompareTag("Player") ||
                        (gameLoop != null && gameLoop.player != null && other.transform == gameLoop.player);

        if (!isPlayer) return;
        if (gameLoop == null) return;

        switch (triggerType)
        {
            case TriggerType.StartZone:
                gameLoop.StartGame();
                break;
            case TriggerType.FinishZone:
                gameLoop.FinishGame();
                break;
            case TriggerType.DeathZone:
                gameLoop.RespawnPlayer();
                break;
        }
    }

    void OnValidate()
    {
        // Keep editor gizmo color consistent when changing triggerType
        switch (triggerType)
        {
            case TriggerType.StartZone:
                gizmoColor = Color.green;
                break;
            case TriggerType.FinishZone:
                gizmoColor = Color.yellow;
                break;
            case TriggerType.DeathZone:
                gizmoColor = Color.red;
                break;
        }
    }

    void OnDrawGizmos()
    {
        var col = GetComponent<Collider>();
        if (col == null) return;

        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.5f);

        // Draw a simple representation using the collider bounds (works for Box/Sphere/Capsule)
        Bounds b = col.bounds;
        Gizmos.DrawWireCube(b.center, b.size);
    }
}
