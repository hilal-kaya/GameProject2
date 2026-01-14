using UnityEngine;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 4f; 
    public float rotationSpeed = 100f;
    public Transform playerTransform; 

    [Header("Saldırı Referansları")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    private QLearningBrain brain;
    private TankStatus myStatus;

    void Start()
    {
        myStatus = GetComponent<TankStatus>();
        brain = GetComponent<QLearningBrain>();

        brain.RegisterAction("Forward", (p) => Move(Vector3.forward), 0);
        brain.RegisterAction("Backward", (p) => Move(Vector3.back), 0);
        brain.RegisterAction("Left", (p) => Rotate(-1f), 0);
        brain.RegisterAction("Right", (p) => Rotate(1f), 0);
        brain.RegisterAction("Shoot", (p) => HandleShooting(), 0);

        if (GameManagerStatic.IsAiLoaded)
        {
            brain.LoadModelFromJson(GameManagerStatic.LoadedJsonData);
            Debug.Log("Ajan rasyonel modda başlatıldı.");
        }
        else
        {
            brain.exploration = 1.0f; 
            Debug.Log("Ajan rastgele modda başlatıldı.");
        }
        if (GameManagerStatic.IsAiLoaded)
        {
            GetComponent<Renderer>().material.color = Color.green; 
            brain.LoadModelFromJson(GameManagerStatic.LoadedJsonData);
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.red; 
            brain.exploration = 1.0f;
        }
    }

    void Update()
    {
        if (myStatus != null && myStatus.isStunned) return;
        if (playerTransform == null) return;

        Vector3 relPos = playerTransform.position - transform.position;
        List<float> inputs = new List<float> { Mathf.Round(relPos.x), Mathf.Round(relPos.z) };
        brain.SetInputs(inputs);

        int actionIndex = brain.DecideAction();
        
        brain.ExecuteAction(actionIndex);

        float dotProduct = Vector3.Dot(transform.forward, relPos.normalized);
        if (dotProduct > 0.9f) 
        {
            brain.Reward(0.1f); 
        }

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance < 5f) brain.Reward(0.05f);
    }

    public void HandleShooting()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }

    void Move(Vector3 dir) => transform.Translate(dir * moveSpeed * Time.deltaTime);
    void Rotate(float side) => transform.Rotate(Vector3.up * side * rotationSpeed * Time.deltaTime);
}