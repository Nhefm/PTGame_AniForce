using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isCheck;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isCheck = false;
    }

    public bool SetCheckpoint()
    {
        if(isCheck == true)
        {
            return false;
        }

        isCheck = true;
        GetComponent<Animator>().SetTrigger("Check");
        return true;
    }

}
