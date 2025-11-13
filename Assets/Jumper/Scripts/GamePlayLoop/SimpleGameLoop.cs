using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleGameLoop : MonoBehaviour
{
    private const float UI_PADDING = 300f;
    private const int TITLE_FONT_SIZE = 48;

    [Header("Player Settings")]
    public Transform player;
    public float deathHeight = -10f;
    private Vector3 startPosition;

    [Header("Game State")]
    private bool gameStarted = false;
    private bool gameFinished = false;
    private float gameTime = 0f;

    [Header("UI Settings")]
    public bool showUI = true;
    private GUIStyle titleStyle;
    private GUIStyle textStyle;

    void Start()
    {
        // Save starting position
        if (player != null)
        {
            startPosition = player.position;
        }

        // Setup UI styles
        titleStyle = new GUIStyle();
        titleStyle.fontSize = TITLE_FONT_SIZE;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.white;
        titleStyle.alignment = TextAnchor.MiddleCenter;

        textStyle = new GUIStyle();
        textStyle.fontSize = 24;
        textStyle.normal.textColor = Color.white;
        textStyle.alignment = TextAnchor.MiddleCenter;
    }

    void Update()
    {
        // Timer update
        if (gameStarted && !gameFinished)
        {
            gameTime += Time.deltaTime;

            // Check if player fell
            if (player != null && player.position.y < deathHeight)
            {
                RespawnPlayer();
            }
        }

        // Restart with R key
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
    }

    void OnGUI()
    {
        if (!showUI) return;

        // Timer display (top center)
        if (gameStarted && !gameFinished)
        {
            GUI.Label(new Rect(Screen.width / 2 - 150, 20, 300, 50),
                FormatTime(gameTime), textStyle);
        }

        // Start message
        if (!gameStarted)
        {
            GUI.Label(new Rect(Screen.width / 2 - UI_PADDING, Screen.height / 2 - 100, UI_PADDING * 2, 100),
                "Enter GREEN zone to START", titleStyle);

            GUI.Label(new Rect(Screen.width / 2 - UI_PADDING, Screen.height / 2 + 50, UI_PADDING * 2, 50),
                "WASD-Move | Space-Jump | Shift-Sprint | R-Restart", textStyle);
        }

        // Win message
        if (gameFinished)
        {
            GUI.Label(new Rect(Screen.width / 2 - UI_PADDING, Screen.height / 2 - 100, UI_PADDING * 2, 100),
                "YOU WIN!", titleStyle);

            GUI.Label(new Rect(Screen.width / 2 - UI_PADDING, Screen.height / 2 + 50, UI_PADDING * 2, 50),
                "Time: " + FormatTime(gameTime), textStyle);

            GUI.Label(new Rect(Screen.width / 2 - UI_PADDING, Screen.height / 2 + 100, UI_PADDING * 2, 50),
                "Press R to Restart", textStyle);
        }
    }

    // Called by trigger zones
    public void StartGame()
    {
        if (gameStarted) return;

        gameStarted = true;
        gameTime = 0f;
        Debug.Log("GAME STARTED!");
    }

    public void FinishGame()
    {
        if (gameFinished || !gameStarted) return;

        gameFinished = true;
        Debug.Log("GAME FINISHED! Time: " + FormatTime(gameTime));
    }

    public void RespawnPlayer()
    {
        if (player == null) return;

        player.position = startPosition;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        player.rotation = Quaternion.identity;

        Debug.Log("Player Respawned!");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }
}
