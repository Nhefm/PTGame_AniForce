using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] int idx;
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.GetComponent<PlayerController>())
        {
            SceneManager.LoadScene("map" + idx);
        }
    }
}
