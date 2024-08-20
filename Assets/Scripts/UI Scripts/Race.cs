using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Race : MonoBehaviour
{

    [SerializeField] GameObject player;
    [SerializeField] List<GameObject> checkPoints;
    [SerializeField] Vector3 vectorPoint;
    [SerializeField] float dead;

    void Update()
    {
        if (player.transform.position.y < -dead)
        {
            player.transform.position = vectorPoint;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            vectorPoint = player.transform.position;
            Checkpoint checkpoint = other.GetComponent<Checkpoint>();
            checkpoint.ActivateNextCheckpoint();
            Destroy(other.gameObject);
        }
       
    }
    
}
