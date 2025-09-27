using UnityEngine;
using TMPro; // Important: Need this for TextMeshPro

public class FPSCounter : MonoBehaviour
{
    // Assign your TextMeshProUGUI component in the Inspector
    [SerializeField]
    private TextMeshProUGUI fpsText;

    // Used to average FPS over a short period for a smoother reading
    private float pollingTime = 0.5f;
    private float time;
    private int frameCount;

    void Update()
    {
        if (fpsText == null) return;

        // 1. Accumulate time and frame count
        time += Time.unscaledDeltaTime;
        frameCount++;

        // 2. Check if the polling interval has passed
        if (time >= pollingTime)
        {
            // 3. Calculate the average FPS for this interval
            int frameRate = Mathf.RoundToInt(frameCount / time);

            // 4. Update the Text element
            fpsText.text = frameRate.ToString() + " FPS";

            // 5. Reset the counters
            time -= pollingTime;
            frameCount = 0;
        }
    }
}