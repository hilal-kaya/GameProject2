using UnityEngine;

public class Mine : MonoBehaviour
{
    public int damage = 50; 

    void OnTriggerEnter(Collider other)
    {
        Health targetHealth = other.GetComponent<Health>();

        if (targetHealth != null)
        {
            if (other.CompareTag("Enemy"))
            {
                Debug.Log("BOOM! Mayın Patladı!");
                targetHealth.TakeDamage(damage);
                Destroy(gameObject); 
            }
        }
    }
}