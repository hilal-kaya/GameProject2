using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f; 

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Health targetHealth = other.GetComponent<Health>();

        if (targetHealth != null)
        {

            if (other.CompareTag("Enemy")) 
            {
                targetHealth.TakeDamage(20); 
                Destroy(gameObject);
            }
        }
        
        else if (! other.CompareTag("Bullet") && !other.CompareTag("Player")) 
        {
             Destroy(gameObject);
        }
    }
}