using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject nextCheckpoint;
    public void ActivateNextCheckpoint() // Activo el prox Checkpoint
    {
        if (nextCheckpoint != null)
        {
            nextCheckpoint.SetActive(true);
        }
    }
}
