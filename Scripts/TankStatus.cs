using UnityEngine;
using System.Collections; 

public class TankStatus : MonoBehaviour
{
    private Renderer tankRenderer;
    private Color originalColor;
    public bool isStunned = false; 

    void Start()
    {
        tankRenderer = GetComponent<Renderer>();
        if (tankRenderer != null)
        {
            originalColor = tankRenderer.material.color;
        }
    }

    public void Stun(float duration)
    {
        if (!isStunned) 
        {
            StartCoroutine(StunRoutine(duration));
        }
    }

    IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        Debug.Log(gameObject.name + " SERSEMLEDİ! Hareket edemez.");
        
        if (tankRenderer != null) tankRenderer.material.color = Color.blue;

        yield return new WaitForSeconds(duration);

        isStunned = false;
        if (tankRenderer != null) tankRenderer.material.color = originalColor;
        Debug.Log(gameObject.name + " normale döndü.");
    }
}