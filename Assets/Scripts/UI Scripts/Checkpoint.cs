using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject nextCheckpoint;
    
    void Start()
    {
        
    }

   
    void Update()
    {
       
    }
    public void ActivateNextCheckpoint() // Activo el prox Checkpoint
    {
        if (nextCheckpoint != null)
        {
            nextCheckpoint.SetActive(true);
        }
    }
}
