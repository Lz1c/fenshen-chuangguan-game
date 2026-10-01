using UnityEngine;
using UnityEngine.Events;

namespace IronSamiStudio.AdvancedTurretAI {
public class TurretProjectile : MonoBehaviour
{
    public enum ProjectileMode
    {
        Directional, // Moves in a straight line
        Homing       // Seeks a target like a homing missile
    }

    [Header("Projectile Settings")]
    [Tooltip("Select the mode for this projectile: Directional or Homing.")]
    public ProjectileMode mode = ProjectileMode.Directional;
    [Tooltip("Speed of the projectile (units per second).")]
    public float speed = 20f;
    [Tooltip("Effect to spawn on hit (optional).")]
    public GameObject hitEffectPrefab;
    [Tooltip("Lifetime of the projectile before it self-destructs (in seconds).")]
    public float lifeTime = 3f;
    [Tooltip("Rotation speed for homing mode (degrees per second).")]
    public float rotationSpeed = 180f;
    [Tooltip("Lifetime of the hit effect (in seconds).")]
    public float hitEffectLifetime = 2f;

    [Header("Collision Settings")]
    [Tooltip("Select which layers the projectile can collide with.")]
    public LayerMask collisionLayers; // User-defined layers

    private Rigidbody rb;
    private TurretController ownerTurret;
    private Transform target;
    private bool isRaycastMode;

    public UnityEvent<GameObject, float> enemyHit;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Projectile requires a Rigidbody component!", gameObject);
            enabled = false;
            return;
        }
    }

    void Start()
    {
        if (mode == ProjectileMode.Directional && !isRaycastMode)
        {
            rb.velocity = transform.forward * speed;
        }

        if (isRaycastMode)
        {
            CheckRaycastHit();
        }

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (mode == ProjectileMode.Homing && target != null)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            rb.velocity = transform.forward * speed;
        }
        else if (mode == ProjectileMode.Homing && target == null)
        {
            rb.velocity = transform.forward * speed;
        }
    }

    public void Initialize(TurretController turret, Transform target = null, Vector3? raycastStart = null, Vector3? raycastDirection = null)
    {
        ownerTurret = turret;
        this.target = target;
        isRaycastMode = ownerTurret.shootMode == TurretController.ShootMode.Raycast;

        if (isRaycastMode && raycastStart.HasValue && raycastDirection.HasValue)
        {
            transform.position = raycastStart.Value;
            transform.rotation = Quaternion.LookRotation(raycastDirection.Value);
        }
    }

    private void CheckRaycastHit()
    {
        RaycastHit hit;

        // Create a layer mask to ignore the turret's layer
        int turretLayer = ownerTurret.gameObject.layer;
        LayerMask ignoreTurretMask = ~LayerMask.GetMask(LayerMask.LayerToName(turretLayer));

        // Combine the ignore mask with the user-defined collision layers
        LayerMask finalMask = collisionLayers & ignoreTurretMask;

        if (Physics.Raycast(transform.position, transform.forward, out hit, ownerTurret.turretRangeRadius, finalMask))
        {
            transform.position = hit.point; // Move to hit point
            HandleCollision(hit.collider);
        }
        else
        {
            transform.position += transform.forward * ownerTurret.turretRangeRadius;
            Destroy(gameObject);
        }
    }

    private void HandleCollision(Collider other)
    {
        if (((1 << other.gameObject.layer) & collisionLayers) != 0) // Check if the object is in the selected layers
        {
            if (other.CompareTag("Enemy"))
            {
                GameObject enemy = other.gameObject;
                if (enemy != null && ownerTurret != null)
                {
                    enemyHit?.Invoke( enemy, ownerTurret.turretDamage ); // invoke and pass the hit object + the owner turret damage 
                }
            }

            if (hitEffectPrefab != null)
            {
                GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                Destroy(hitEffect, hitEffectLifetime);
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isRaycastMode)
        {
            HandleCollision(other);
        }
    }
}
}