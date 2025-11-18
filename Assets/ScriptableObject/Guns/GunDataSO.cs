using UnityEngine;

[CreateAssetMenu(fileName = "GunDataSO", menuName = "Scriptable Objects/GunDataSO")]
public class GunDataSO : ScriptableObject
{
    public GameObject bulletPrefab;
    public LayerMask botLayer;

    [Header("Настройки стрельбы")]
    public float damagePerBullet = 15f;
    public float maxShootDistance = 15f;
    public int shootPerMinutes = 600;
    public float spread = 0.1f;
    public int bulletsPerShot = 1;
    public float bulletLife = 3f;
    public AudioClip bulletSound;

    public float rotationRadius = 1.4f;
    public float rotationSpeed = 90f;
}

