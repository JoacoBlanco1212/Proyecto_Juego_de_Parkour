﻿

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RaceTrigger : MonoBehaviour
{
    public GameObject firstCheckpoint;
    public GameObject raceMenu;
    public GameObject player;
    public Rigidbody playerRigidbody;
    public Transform startPoint;
    public GameObject countdownText;
    public GameObject raceTimerText;
    public GameObject lastCheckpoint;
    public GameObject finishPanel;  // Panel que se muestra al finalizar la carrera
    public Text finalTimeText;  // Texto que muestra el tiempo final en el panel de finalización

    private bool raceStarted = false;
    private bool raceFinished = false;
    private float raceTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PlayerObj" && !raceStarted)
        {
            ShowRacePrompt();
        }
    }

    void ShowRacePrompt()
    {
        raceMenu.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void No()
    {
        raceMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Yes()
    {
        raceMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        player.transform.position = startPoint.position;
        player.transform.rotation = startPoint.rotation;
        playerRigidbody.velocity = Vector3.zero;

        StartCoroutine(StartRaceCountdown());
    }

    private IEnumerator StartRaceCountdown()
    {
        raceStarted = true;
        countdownText.SetActive(true);
        playerRigidbody.constraints = RigidbodyConstraints.FreezePosition;

        for (int i = 3; i > 0; i--)
        {
            countdownText.GetComponent<Text>().text = i.ToString();
            yield return new WaitForSeconds(1);
        }

        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        countdownText.GetComponent<Text>().text = "GO!";
        yield return new WaitForSeconds(1);

        countdownText.SetActive(false);
        firstCheckpoint.SetActive(true);

        StartCoroutine(UpdateRaceTimer());
    }

    private IEnumerator UpdateRaceTimer()
    {
        raceTimerText.SetActive(true);

        while (!raceFinished)
        {
            raceTime += Time.deltaTime;
            int seconds = Mathf.FloorToInt(raceTime);
            int milliseconds = Mathf.FloorToInt((raceTime - seconds) * 100);
            raceTimerText.GetComponent<Text>().text = string.Format("{0:00}.{1:00}", seconds, milliseconds);
            yield return null;
        }
    }

    public void OnLastCheckpointReached()
    {
        raceFinished = true;
        lastCheckpoint.SetActive(false);
        raceTimerText.SetActive(false);

        Time.timeScale = 0;  // Pausa el juego
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Actualiza el texto del tiempo final en el panel de finalización
        int finalSeconds = Mathf.FloorToInt(raceTime);
        int finalMilliseconds = Mathf.FloorToInt((raceTime - finalSeconds) * 100);
        finalTimeText.text = string.Format(" {0:00}.{1:00}", finalSeconds, finalMilliseconds);

        finishPanel.SetActive(true);  // Muestra el panel de finalización
    }

    public void RetryRace()
    {
        finishPanel.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        player.transform.position = startPoint.position;
        player.transform.rotation = startPoint.rotation;
        playerRigidbody.velocity = Vector3.zero;

        raceTime = 0f;
        raceFinished = false;

        firstCheckpoint.SetActive(false);
        lastCheckpoint.SetActive(false);
        raceStarted = false;

        StartCoroutine(StartRaceCountdown());
    }

    public void ReturnToStart()
    {
        finishPanel.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        player.transform.position = startPoint.position;
        player.transform.rotation = startPoint.rotation;
        playerRigidbody.velocity = Vector3.zero;

        raceTime = 0f;
        raceFinished = false;
        raceStarted = false;

        firstCheckpoint.SetActive(false);
    }
}
