using UnityEngine;
using UnityEngine.UI; // Required for accessing UI elements like Slider

public class PlayerUI : MonoBehaviour
{
    [Header("Wall Run UI")]
    public Slider wallRunSlider; // Drag your Slider UI element here in the Inspector
    public GameObject wallRunBarContainer; // The parent object of the slider to easily hide/show it

    private void Start()
    {
        // Ensure the bar is hidden when the game starts
        if (wallRunBarContainer != null)
        {
            wallRunBarContainer.SetActive(false);
        }
    }

    // This function will be called from the PlayerMovement script
    public void UpdateWallRunBar(float currentDuration, float maxDuration)
    {
        if (wallRunSlider == null) return;

        // Calculate the remaining time as a value between 0 and 1
        float remainingValue = (maxDuration - currentDuration) / maxDuration;
        wallRunSlider.value = remainingValue;
    }

    // Call this to show or hide the entire bar
    public void SetWallRunBarVisibility(bool isVisible)
    {
        if (wallRunBarContainer != null)
        {
            wallRunBarContainer.SetActive(isVisible);
        }
    }
}