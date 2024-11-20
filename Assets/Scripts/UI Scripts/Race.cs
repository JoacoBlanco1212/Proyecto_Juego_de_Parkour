using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Race : MonoBehaviour
{

    public RaceTrigger raceTrigger;
    private GameObject player;
    private Vector3 vectorPoint;
    [SerializeField] float dead;

    private void Start()
    {
        player = gameObject;
        vectorPoint = player.transform.position;
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
         if (other.CompareTag("Checkpoint"))
        {
            vectorPoint = player.transform.position;
            Checkpoint checkpoint = other.GetComponent<Checkpoint>();
	    if(checkpoint.nextCheckpoint != null) {	
               checkpoint.ActivateNextCheckpoint();
               vectorPoint = checkpoint.transform.position;
               other.gameObject.SetActive(false);
	    } else {
               raceTrigger.OnLastCheckpointReached();
	    }
        }
       
    }
    
}
