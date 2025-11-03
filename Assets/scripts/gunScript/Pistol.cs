using UnityEngine;

public class Pistol : MonoBehaviour, IGun
{
    [SerializeField] private GameObject bulletPrefab;
    
    private Transform spawnPoint;
    
    public float shootInterval = 1f; // Интервал между выстрелами

    private float lastShotTime;
    
    public float rotationRadius = 1f;
    public float rotationSpeed = 90f;
    
    private float currentAngle = 0f;
    

    void Update()
    {
        rotate();
        
        spawnPoint = transform;
        if (Time.time - lastShotTime >= shootInterval)
        {
            spawnBoolet();
            lastShotTime = Time.time;
        }
    }
    
    public void spawnBoolet()
    {
        Instantiate(bulletPrefab, spawnPoint.position, this.transform.rotation);
    }

    public void rotate()
    {
        currentAngle += rotationSpeed * Time.deltaTime;
        
        float x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * rotationRadius;
        float y = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * rotationRadius;
        
       transform.localPosition = new Vector3(x, y, 0);
       
        Vector3 direction = transform.localPosition.normalized;
        float lookAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, lookAngle + 180f);
    }
}