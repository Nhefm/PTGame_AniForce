using UnityEngine;

public class Boundary : MonoBehaviour
{
    private Vector3 storedPos;
    private void OnTriggerStay2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();

        if(player)
        {
            storedPos = player.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        var player = other.GetComponent<PlayerController>();

        if(player)
        {
            player.transform.SetPositionAndRotation(storedPos, player.transform.rotation);
        }
    }
}
