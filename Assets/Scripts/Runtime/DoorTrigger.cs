using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var npc = other.GetComponent<NpcController>();
        if (npc != null)
        {
            gameManager.OnNpcReachedDoor(npc);
            Destroy(npc.gameObject);
        }
    }
}