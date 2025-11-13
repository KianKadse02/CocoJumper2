using UnityEngine;
using UnityEngine.UI;
using TMPro; // 1. Add this line for TextMeshPro

public class PlayerUI : MonoBehaviour
{
    [Header("Wall Run UI")]
    public Slider wallRunSlider;
    public GameObject wallRunBarContainer;
    public Text scoreText;
    [Header("Air Jump UI")]
    // 2. Add this variable for the text
    public TextMeshProUGUI airJumpText;

    private void Start()
    {
        if (wallRunBarContainer != null)
        {
            wallRunBarContainer.SetActive(false);
        }
    }

    public void UpdateScoreText(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    public void UpdateWallRunBar(float currentDuration, float maxDuration)
    {
        if (wallRunSlider == null) return;

        float remainingValue = (maxDuration - currentDuration) / maxDuration;
        wallRunSlider.value = remainingValue;
    }

    public void SetWallRunBarVisibility(bool isVisible)
    {
        if (wallRunBarContainer != null)
        {
            wallRunBarContainer.SetActive(isVisible);
        }
    }

    // 3. Add this new function to update the jump text
    public void UpdateAirJumpText(int remainingJumps)
    {
        if (airJumpText == null) return;

        // We will format the string to be displayed
        airJumpText.text = $"Air Jumps: {remainingJumps}";
    }

    // (Optional but Recommended) A function to hide the text when not needed
    public void SetAirJumpTextVisibility(bool isVisible)
    {
        if (airJumpText != null)
        {
            airJumpText.gameObject.SetActive(isVisible);
        }
    }
}