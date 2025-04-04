
using UnityEngine;
using TMPro;  // TextMeshPro namespace
using System.Collections;  // Add this for IEnumerator support

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject selectSongCanvas;
    public GameObject metricMenuCanvas;  // Metric menu canvas
    public GameObject drumObject;  // Drum object to reveal
    public TextMeshProUGUI countdownText;  // UI Text for countdown (TextMeshPro)

    public void ShowSelectSongMenu()
    {
        metricMenuCanvas.SetActive(false);
        drumObject.SetActive(false);
        countdownText.gameObject.SetActive(false);
        mainMenuCanvas.SetActive(false);
        selectSongCanvas.SetActive(true);
    }

    public void GoBackToMainMenu()
    {
        selectSongCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
        metricMenuCanvas.SetActive(false);
        drumObject.SetActive(false);
        countdownText.gameObject.SetActive(false);
    }

    public void StartTutorial()
    {
        // Hide all UI menus before the tutorial starts
        mainMenuCanvas.SetActive(false);
        selectSongCanvas.SetActive(false);
        metricMenuCanvas.SetActive(false);

        // Ensure countdown text and drum object are initially inactive
        countdownText.gameObject.SetActive(false);
        drumObject.SetActive(false);

        // Reveal the drum and countdown text
        drumObject.SetActive(true);
        countdownText.gameObject.SetActive(true);
        

        // Start the countdown from 3 to 1
        StartCoroutine(CountdownSequence());
    }

    private IEnumerator CountdownSequence()
    {
        countdownText.gameObject.SetActive(true);

        // Countdown from 3 to 1
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);  // Wait for 1 second before updating the countdown
        }

        countdownText.text = "Go!";
        yield return new WaitForSeconds(1);  // Wait for 1 second before starting the game

        for (int i = 10; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);  // Wait for 1 second before updating the countdown
        }
        // Run the gameplay loop for 10 seconds
        yield return new WaitForSeconds(7);

        // Hide the countdown text and the drum after gameplay
        countdownText.gameObject.SetActive(false);
        drumObject.SetActive(false);
        mainMenuCanvas.SetActive(false);
        selectSongCanvas.SetActive(false);

        // Transition to MetricMenu
        metricMenuCanvas.SetActive(true);
    }
}
