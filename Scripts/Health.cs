using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100; 
    private int currentHealth;
    public AudioClip explosionSound;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        
        QLearningBrain brain = GetComponent<QLearningBrain>();
        if (brain != null)
        {
            brain.Punish(0.5f);
        }

        if (currentHealth <= 0)
        {
            if (brain != null) brain.Punish(2.0f); 
            Die();
        }
    }

    void Die()
    {
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, 1.0f);
        }
        Debug.Log(gameObject.name + " yok oldu!");
        Destroy(gameObject);
    }
}