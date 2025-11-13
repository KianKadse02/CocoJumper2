using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    public float startingScore = 10000f;
    public float lossRate = 50f;
    public float wallRunGainRate = 100f;

    private float totalScore;
    private PlayerMovement playerMovement;
    private PlayerUI playerUI;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerUI = playerMovement.playerUI;
        totalScore = startingScore;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        float rate = -lossRate; // lose points by default

        // check if player is currently wallrunning
        bool isWallRunning = IsWallRunning();

        if (isWallRunning)
        {
            rate = wallRunGainRate; // gain points instead
        }

        totalScore += rate * deltaTime;
        totalScore = Mathf.Max(0, totalScore);

        if (playerUI != null)
        {
            playerUI.UpdateScoreText(Mathf.RoundToInt(totalScore));
        }
    }

    private bool IsWallRunning()
    {
        // Access the wallRunTimer from PlayerMovement to detect active wallrun
        // We use reflection-safe check based on how your PlayerMovement is structured
        return playerMovement != null && playerMovement.wallDetection != null && playerMovement.wallDetection.isNearRunnableWall;
    }

    public int GetScore()
    {
        return Mathf.RoundToInt(totalScore);
    }
}
