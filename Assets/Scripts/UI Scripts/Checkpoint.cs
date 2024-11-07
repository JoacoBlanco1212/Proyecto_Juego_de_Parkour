using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject nextCheckpoint;
    private Vector3 position;

    private float time;

    void Start()
    {
        position = transform.position;
    }

    void Update()
    {
        time += Time.deltaTime;

        transform.position = new Vector3(position.x, position.y + Mathf.Cos(time) / 5 , position.z);
    }

    public void ActivateNextCheckpoint() // Activo el prox Checkpoint
    {
        if (nextCheckpoint != null)
        {
            nextCheckpoint.SetActive(true);
        }
    }
}
