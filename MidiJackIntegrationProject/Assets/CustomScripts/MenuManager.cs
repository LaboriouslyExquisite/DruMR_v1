using UnityEngine;
using TMPro;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("UI Canvases")]
    public GameObject mainMenuCanvas;
    public GameObject selectSongCanvas;
    public GameObject metricMenuCanvas;
    public GameObject countdownCanvas;
    public GameObject tempoPracticeCanvas;


    [Header("Gameplay Objects")]
    public GameObject drumObject;
    public GameObject[] movableObjects; // Assign [BuildingBlock] HandGrab GameObjects
    public GameObject acceptButton;

    [Header("Countdown")]
    public TextMeshProUGUI countdownText;

    private void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        mainMenuCanvas.SetActive(true);
        selectSongCanvas.SetActive(false);
        metricMenuCanvas.SetActive(false);
        countdownText.gameObject.SetActive(false);
        acceptButton.SetActive(false);
        drumObject.SetActive(false);
        countdownCanvas.SetActive(false);


        SetMovableObjectsActive(false);
    }

    public void ShowSelectSongMenu()
    {
        mainMenuCanvas.SetActive(false);
        selectSongCanvas.SetActive(true);
        metricMenuCanvas.SetActive(false);
        drumObject.SetActive(false);
        countdownText.gameObject.SetActive(false);
        acceptButton.SetActive(false);

        SetMovableObjectsActive(false);
    }

    public void StartPlacementMode()
    {
        // Hide all menus
        mainMenuCanvas.SetActive(false);
        selectSongCanvas.SetActive(false);
        metricMenuCanvas.SetActive(false);

        // Show drums and accept button
        drumObject.SetActive(true);
        acceptButton.SetActive(true);

        // Enable placement mode
        SetMovableObjectsActive(true);
        countdownText.gameObject.SetActive(true);
    }

    public void AcceptPlacement()
    {
        acceptButton.SetActive(false);

        // Lock objects in place (disable movement, reset color)
        SetMovableObjectsActive(false);

        StartCoroutine(CountdownSequence());
    }

    private IEnumerator CountdownSequence()
    {
        countdownCanvas.SetActive(true);
        countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "Go!";
        yield return new WaitForSeconds(1f);

        for (int i = 15; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);

        // DO NOT hide drums here anymore
        ShowMetricMenu();
    }

    public void ShowMetricMenu()
    {
        drumObject.SetActive(false);
        mainMenuCanvas.SetActive(false);
        selectSongCanvas.SetActive(false);
        metricMenuCanvas.SetActive(true);
    }

    private void SetMovableObjectsActive(bool isActive)
    {
        foreach (GameObject obj in movableObjects)
        {
            obj.SetActive(true); // Make sure it's visible

            var handGrab = obj.GetComponent<Oculus.Interaction.HandGrab.HandGrabInteractable>();
            if (handGrab != null) handGrab.enabled = isActive;

            // Optional: Change color to pink while movable
            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = isActive ? Color.magenta : Color.white;
            }
        }
    }

    public void StartTempoPracticeMode()
    {
        mainMenuCanvas.SetActive(false);
        selectSongCanvas.SetActive(false);
        metricMenuCanvas.SetActive(false);
        countdownCanvas.SetActive(false);
        tempoPracticeCanvas.SetActive(true);

        drumObject.SetActive(true);
        acceptButton.SetActive(false);
        SetMovableObjectsActive(false);

    }



    public void GoBackToMainMenu()
    {
        mainMenuCanvas.SetActive(true);
        selectSongCanvas.SetActive(false);
        metricMenuCanvas.SetActive(false);
        countdownCanvas.SetActive(false);
        tempoPracticeCanvas.SetActive(false);

        countdownText.gameObject.SetActive(false);
        acceptButton.SetActive(false);
        drumObject.SetActive(false);

        SetMovableObjectsActive(false);
    }
}