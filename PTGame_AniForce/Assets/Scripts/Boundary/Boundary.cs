using UnityEngine;

public class Boundary : MonoBehaviour
{
    [SerializeField] private SwappingController swappingController;
    
    private void OnTriggerExit2D(Collider2D other) {
        var player = other.GetComponent<PlayerController>();
        Debug.Log("Exit");
        if(player)
        {
            swappingController.Swap();
        }
    }
}
