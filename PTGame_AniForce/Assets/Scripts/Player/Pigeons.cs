using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pigeons : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float attackDuration;
    [SerializeField] private AudioClip attackSound;
    private bool canAttack;
    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        canAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector2.right);

        if(transform.position.x > Camera.main.ScreenToWorldPoint(Vector3.right * Camera.main.pixelWidth).x)
        {
            gameObject.SetActive(false);
        }

        if(!gameObject.activeInHierarchy)
        {
            canAttack = true;
            return;
        }

        if(!canAttack)
        {
            return;
        }

        StartCoroutine(Attack());
    }

    public IEnumerator Attack()
    {
        canAttack = false;
        GameObject bullet = ObjectPooling.sharedInstance.GetObject();

        if(bullet)
        {
            bullet.SetActive(true);
            bullet.transform.position = transform.position;
            audioSource.PlayOneShot(attackSound);
        }


        yield return new WaitForSeconds(attackDuration);
        canAttack = true;
    }
}
