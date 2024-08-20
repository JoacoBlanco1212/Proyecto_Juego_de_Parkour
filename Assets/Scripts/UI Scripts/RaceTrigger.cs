/*
using UnityEngine;
using System.Collections;

public class RaceTrigger : MonoBehaviour
{
    public GameObject firstCheckpoint;
    public GameObject racePanel;
    public GameObject player;  // Referencia al jugador
    public Rigidbody playerRigidbody;  // Referencia al Rigidbody del jugador
    public Transform startPoint;  // Transform donde se ubicará el jugador al iniciar la carrera
    public GameObject countdownText;  // Referencia al texto que mostrará el contador

    private bool raceStarted = false;

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

        // Mueve al jugador a la posición de inicio, (lo asigno en "U" a un lugar orientado hacia el primer checkpoint)
        player.transform.position = startPoint.position;
        player.transform.rotation = startPoint.rotation;

        // Paro la velocidad del jugador, para que no empieze con velocidad
        playerRigidbody.velocity = Vector3.zero;

        // Cuenta regresiva
        StartCoroutine(StartRaceCountdown());
    }

    private IEnumerator StartRaceCountdown()
    {
        raceStarted = true;

        countdownText.SetActive(true);  // Activo el txt del contador

        playerRigidbody.constraints = RigidbodyConstraints.FreezePosition;

        for (int i = 3; i > 0; i--)
        {
            countdownText.GetComponent<UnityEngine.UI.Text>().text = i.ToString();  // Actualiza el texto
            yield return new WaitForSeconds(1);  // 1 segundo de espera
        }


        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        countdownText.GetComponent<UnityEngine.UI.Text>().text = "GO!";  // Muestra el "GO!"
        yield return new WaitForSeconds(1);

        countdownText.SetActive(false);  // Oculta el txt del contador
        firstCheckpoint.SetActive(true);  // Empieza la carrera y activa el primer checkpoint
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
    public GameObject raceTimerText;  // Referencia al texto del cronómetro
    public GameObject lastCheckpoint;  // Referencia al último checkpoint

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

        StartCoroutine(UpdateRaceTimer());  // Inicia el cronómetro
    }

    private IEnumerator UpdateRaceTimer()
    {
        raceTimerText.SetActive(true);  // Muestra el cronómetro en pantalla

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
        lastCheckpoint.SetActive(false);  // Desactiva el último checkpoint
        // Aquí puedes agregar más lógica para finalizar la carrera, como mostrar un panel con el resultado.
    }
}
