using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 5.0f;
    public float rotationSpeed = 100.0f;

    public GameObject bulletPrefab;
    public Transform firePoint;    
    public float empRadius = 10f; 
    public float empDuration = 3f; 
    public GameObject minePrefab;
    public AudioClip shootSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        float moveInput = Input.GetAxis("Vertical");
        float rotateInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.forward * moveInput * movementSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * rotateInput * rotationSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            DropMine();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ActivateEMP();
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
    void DropMine()
    {
        Vector3 dropPosition = transform.position - (transform.forward * 2);
        
        dropPosition.y = 0.05f;

        Instantiate(minePrefab, dropPosition, Quaternion.identity);
    }


    void ActivateEMP ()
    {
        Debug.Log("EMP Dalgası Yayıldı!");

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, empRadius);

        foreach (var hitCollider in hitColliders)
        {
            TankStatus status = hitCollider.GetComponent<TankStatus>();

            if (status != null && hitCollider.gameObject != gameObject)
            {
                status.Stun(empDuration);
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, empRadius);
    }
}