using Unity.VisualScripting;
using UnityEngine;

public class Boundary : MonoBehaviour
{
    [SerializeField] private SwappingController swappingController;
    
    private void OnTriggerExit2D(Collider2D other) {
        if(!other)
        {
            return;
        }

        var player = other.GetComponent<PlayerController>();
        
        if(player && player.gameObject.activeInHierarchy)
        {
            swappingController.Swap();
        }
    }
}
