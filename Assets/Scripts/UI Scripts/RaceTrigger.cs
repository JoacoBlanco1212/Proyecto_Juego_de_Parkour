/*           
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RaceTrigger : MonoBehaviour
{
    public GameObject firstCheckpoint;
    public GameObject racePanel;
    public GameObject player;
    public Rigidbody playerRigidbody;
    public Transform startPoint;
    public GameObject countdownText;
    public GameObject raceTimerText;  // Reference al txt del cronometro
    public GameObject lastCheckpoint;  // Reference al ultimo checkpoint

    private bool raceStarted = false;
    private bool raceFinished = false;
    private float raceTime = 0f;  // Tiempo transcurrido en la carrera

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PlayerObj" && !raceStarted)
        {
            ShowRacePrompt();
        }
    }

    void ShowRacePrompt()
    {
        racePanel.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void No()
    {
        racePanel.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Yes()
    {
        racePanel.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Mueve al jugador a la posición de inicio
        player.transform.position = startPoint.position;
        player.transform.rotation = startPoint.rotation;

        // Paro la velocidad del jugador
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
        firstCheckpoint.SetActive(true);  // Activa el primer checkpoint

        StartCoroutine(UpdateRaceTimer());  // Inicia el cronometro
    }

    private IEnumerator UpdateRaceTimer()
    {
        raceTimerText.SetActive(true);  // Muestra el cronometro en pantalla

        while (!raceFinished)
        {
            raceTime += Time.deltaTime;  // Incrementa el tiempo transcurrido
            int minutes = Mathf.FloorToInt(raceTime / 60F);
            int seconds = Mathf.FloorToInt(raceTime % 60F);
            raceTimerText.GetComponent<Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);  // Actualiza el texto
            yield return null;
        }
    }

    public void OnLastCheckpointReached()
    {
        raceFinished = true;
        raceTimerText.GetComponent<Text>().text = "Final Time: " + raceTimerText.GetComponent<Text>().text;  // Muestra el tiempo final
        lastCheckpoint.SetActive(false);  // Desactiva el ultimo checkpoint
    }
}
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RaceTrigger : MonoBehaviour
{
    public GameObject firstCheckpoint;
    public GameObject racePanel;
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
        racePanel.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void No()
    {
        racePanel.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Yes()
    {
        racePanel.SetActive(false);
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
            int minutes = Mathf.FloorToInt(raceTime / 60F);
            int seconds = Mathf.FloorToInt(raceTime % 60F);
            raceTimerText.GetComponent<Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
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

        finalTimeText.text = "Your Time: " + raceTimerText.GetComponent<Text>().text;
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
        lastCheckpoint.SetActive(true);

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

        firstCheckpoint.SetActive(false);
    }
}

