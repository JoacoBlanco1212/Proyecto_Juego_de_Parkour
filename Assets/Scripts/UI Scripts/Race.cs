using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Race : MonoBehaviour
{

    private GameObject player;
    private Vector3 vectorPoint;
    [SerializeField] float dead;

    private void Start()
    {
        player = gameObject;
    }

    void Update()
    {
        if (player.transform.position.y < -dead)
        {
            player.transform.position = vectorPoint;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered by: " + other.gameObject.name);
        if (other.CompareTag("Checkpoint"))
        {
            vectorPoint = player.transform.position;
            Checkpoint checkpoint = other.GetComponent<Checkpoint>();
            checkpoint.ActivateNextCheckpoint();
            other.gameObject.SetActive(false);
        }
       
    }
    
}
