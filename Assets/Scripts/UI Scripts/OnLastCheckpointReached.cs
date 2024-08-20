using UnityEngine;

public class OnLastCheckpointReached : MonoBehaviour
{
    public RaceTrigger raceTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PlayerObj")
        {
            raceTrigger.OnLastCheckpointReached();
        }
    }
}
