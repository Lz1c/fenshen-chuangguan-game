using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace IronSamiStudio.AdvancedTurretAI {
public class TurretController : MonoBehaviour
{
    [Header("Turret Settings")]
    public GameObject turretHead;
    public float turretRotationSpeed = 5f;
    [Tooltip("Adjust this to correct the head's rotation if the model is misaligned")]
    public Vector3 headRotationOffset = Vector3.zero;
    [Tooltip("When true, disables up-and-down rotation (pitch) of the turret head")]
    public bool lockPitchRotation = false; // New toggle
    public float turretDamage = 10f;
    public float turretFireRate = 1f;

    [Header("Range & Enemy Settings")]
    public float turretRangeRadius = 5f;
    public string enemyTag = "Enemy";
    
    [Header("Cone Detection Settings")]
    public bool useConeDetection = false;
    [Range(0f, 360f)] public float coneAngle = 90f;
    public Vector3 coneOffset = Vector3.zero;
    public ConeMode coneMode = ConeMode.Fixed;
    [SerializeField] private Color coneGizmoColor = new Color(1, 0, 0, 0.2f);

    public enum ConeMode
    {
        Fixed,
        HeadAttached
    }

    [Header("Turret Shoot Settings")]
    public GameObject projectilePrefab;
    public List<Transform> projectileSpawnPositions = new List<Transform>();
    public Vector3 spawnPointOffset = Vector3.zero;
    public Vector3 spawnAngleOffset = Vector3.zero;

    public enum FiringMode
    {
        Sequential,
        Simultaneous
    }
    public FiringMode firingMode = FiringMode.Sequential;

    public enum ShootMode
    {
        Projectile,
        Raycast
    }
    public ShootMode shootMode = ShootMode.Projectile;

    [Header("Raycast Trail Settings")]
    [SerializeField] private GameObject raycastTrailPrefab;
    public float trailLifetime = 0.2f;

    private int currentSpawnIndex = 0;

    [Header("Muzzle Decal Settings")]
    [SerializeField] private GameObject muzzleDecalPrefab;
    public Vector3 muzzleDecalOffset = Vector3.zero;
    public Vector3 muzzleDecalScale = Vector3.one;
    public float muzzleDecalLifetime = 1f;

    [Header("Muzzle Light Settings")]
    public Light muzzleLight;
    public bool enableLightFlash = true;
    public float lightIntensity = 5f;

    [Header("Firing Alignment")]
    public float maxAngleDeviation = 5f;

    [Header("Obstacle Detection Settings")]
    [SerializeField] private LayerMask obstacleLayerMask;
    public bool enableObstacleCheck = true;
    public bool lockRotationOnObstacle = false;
    private float nextObstacleCheckTime = 0f;
    private const float OBSTACLE_CHECK_INTERVAL = 0.5f;
    private bool isRotationLocked = false;

    private SphereCollider _sphereCollider;
    private List<GameObject> enemiesInRange = new List<GameObject>();
    [HideInInspector]public GameObject _targetEnemy;
    private float nextFireTime = 0f;

    [Header("Events")]
    public UnityEvent OnStart;
    public UnityEvent OnShoot;
    public UnityEvent EnemyIsInRange;
    public UnityEvent IsRotating;
    public UnityEvent<RaycastHit> OnRaycastHit;

    [Header("Debug Tools")]
    public Color gizmoColor = new Color(0, 0, 1, 0.3f);
    public bool showRange = true;

    void Start()
    {
        _sphereCollider = gameObject.GetComponent<SphereCollider>();
        if (_sphereCollider == null)
        {
            _sphereCollider = gameObject.AddComponent<SphereCollider>();
            Debug.Log("Added missing SphereCollider");
        }
        _sphereCollider.isTrigger = true;
        _sphereCollider.radius = turretRangeRadius;

        if (turretHead == null) turretHead = gameObject;

        if (muzzleLight != null)
        {
            muzzleLight.intensity = 0f;
        }
        else if (enableLightFlash)
        {
            Debug.LogWarning("Muzzle Light is not assigned and Enable Light Flash is true.", gameObject);
        }

        if (projectileSpawnPositions.Count == 0)
        {
            Debug.LogWarning("No projectile spawn positions assigned to turret!", gameObject);
        }
        OnStart?.Invoke();
    }

    void Update()
    {
        FindClosestEnemy();
        RotateTurretPredictive();
    }

    public void FindClosestEnemy()
    {
        if (enemiesInRange.Count == 0)
        {
            _targetEnemy = null;
            return;
        }

        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        enemiesInRange.RemoveAll(enemy => enemy == null);

        foreach (GameObject enemy in enemiesInRange)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= turretRangeRadius)
            {
                if (useConeDetection)
                {
                    Vector3 coneOrigin = coneMode == ConeMode.HeadAttached ? turretHead.transform.position : transform.position;
                    Vector3 directionToEnemy = (enemy.transform.position - coneOrigin).normalized;
                    Quaternion offsetRotation = Quaternion.Euler(coneOffset);
                    Vector3 coneForward = offsetRotation * (coneMode == ConeMode.HeadAttached ? turretHead.transform.forward : transform.forward);
                    float angle = Vector3.Angle(coneForward, directionToEnemy);
                    if (angle <= coneAngle / 2f)
                    {
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestEnemy = enemy;
                        }
                    }
                }
                else if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        _targetEnemy = closestEnemy;
    }

    private bool CanShoot(Transform spawnPoint)
    {
        if (!enableObstacleCheck) return true;

        Vector3 spawnPosition = spawnPoint.position + spawnPoint.TransformDirection(spawnPointOffset);
        Quaternion spawnRotation = turretHead.transform.rotation * Quaternion.Euler(spawnAngleOffset);
        Vector3 direction = spawnRotation * Vector3.forward;

        RaycastHit hit;
        if (Physics.Raycast(spawnPosition, direction, out hit, turretRangeRadius, obstacleLayerMask))
        {
            if (_targetEnemy != null)
            {
                if (hit.transform.gameObject == _targetEnemy)
                {
                    Debug.DrawRay(spawnPosition, direction * hit.distance, Color.green, 0.1f);
                    return true;
                }
                else
                {
                    float distanceToEnemy = Vector3.Distance(spawnPosition, _targetEnemy.transform.position);
                    float distanceToHit = hit.distance;
                    
                    if (distanceToEnemy < distanceToHit)
                    {
                        Debug.DrawRay(spawnPosition, direction * distanceToEnemy, Color.green, 0.1f);
                        return true;
                    }
                    
                    Debug.DrawRay(spawnPosition, direction * hit.distance, Color.red, 0.1f);
                    return false;
                }
            }
            else
            {
                Debug.DrawRay(spawnPosition, direction * hit.distance, Color.red, 0.1f);
                return false;
            }
        }
        
        Debug.DrawRay(spawnPosition, direction * turretRangeRadius, Color.green, 0.1f);
        return true;
    }

    private bool IsPathBlocked()
    {
        if (!enableObstacleCheck || projectileSpawnPositions.Count == 0 || _targetEnemy == null) return false;

        Transform spawnPoint = projectileSpawnPositions[0];
        Vector3 spawnPosition = spawnPoint.position + spawnPoint.TransformDirection(spawnPointOffset);
        Vector3 direction = (_targetEnemy.transform.position - spawnPosition).normalized;

        RaycastHit hit;
        if (Physics.Raycast(spawnPosition, direction, out hit, turretRangeRadius, obstacleLayerMask))
        {
            if (hit.transform.gameObject != _targetEnemy)
            {
                Debug.DrawRay(spawnPosition, direction * hit.distance, Color.red, 0.1f);
                return true;
            }
        }
        
        Debug.DrawRay(spawnPosition, direction * turretRangeRadius, Color.green, 0.1f);
        return false;
    }

    void Shoot(Transform spawnPoint)
    {
        if (!CanShoot(spawnPoint))
        {
            return;
        }

        Vector3 spawnPosition = spawnPoint.position + spawnPoint.TransformDirection(spawnPointOffset);
        Quaternion spawnRotation = turretHead.transform.rotation * Quaternion.Euler(spawnAngleOffset);
        Vector3 direction = spawnRotation * Vector3.forward;

        // Muzzle effects
        if (muzzleDecalPrefab != null)
        {
            Vector3 decalPosition = spawnPosition + spawnPoint.TransformDirection(muzzleDecalOffset);
            GameObject decal = Instantiate(muzzleDecalPrefab, decalPosition, spawnRotation);
            if (decal != null)
            {
                Vector3 appliedScale = muzzleDecalScale == Vector3.zero ? muzzleDecalPrefab.transform.localScale : muzzleDecalScale;
                Vector3 parentScale = spawnPoint.localScale;
                Vector3 correctedScale = new Vector3(
                    appliedScale.x / (parentScale.x != 0 ? parentScale.x : 1f),
                    appliedScale.y / (parentScale.y != 0 ? parentScale.y : 1f),
                    appliedScale.z / (parentScale.z != 0 ? parentScale.z : 1f)
                );
                decal.transform.localScale = correctedScale;
            }
            Destroy(decal, muzzleDecalLifetime);
        }

        if (enableLightFlash && muzzleLight != null)
        {
            muzzleLight.intensity = lightIntensity;
            StartCoroutine(DisableLightAfterDelay(0.2f));
        }

        // Shooting logic
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, spawnRotation);
        TurretProjectile proj = projectile.GetComponent<TurretProjectile>();

        if (shootMode == ShootMode.Raycast)
        {
            if (proj != null)
            {
                Vector3 endPosition;
                RaycastHit hit;
                LayerMask mask = LayerMask.GetMask("Enemy", "Obstacles", "Ground");
                if (Physics.Raycast(spawnPosition, direction, out hit, turretRangeRadius, mask))
                {
                    endPosition = hit.point;
                }
                else
                {
                    endPosition = spawnPosition + direction * turretRangeRadius;
                }

                if (raycastTrailPrefab != null)
                {
                    GameObject trail = Instantiate(raycastTrailPrefab, spawnPosition, Quaternion.identity);
                    TrailRenderer trailRenderer = trail.GetComponent<TrailRenderer>();
                    if (trailRenderer != null)
                    {
                        trailRenderer.Clear();
                        trailRenderer.AddPosition(spawnPosition);
                        trailRenderer.AddPosition(endPosition);
                        Destroy(trail, trailLifetime);
                    }
                }

                proj.Initialize(this, null, spawnPosition, direction);
            }
        }
        else
        {
            if (proj != null)
            {
                proj.Initialize(this, _targetEnemy != null ? _targetEnemy.transform : null);
            }
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null && proj.mode == TurretProjectile.ProjectileMode.Directional)
            {
                rb.velocity = direction * proj.speed;
            }
        }

        OnShoot?.Invoke();
    }

    void RotateTurretPredictive()
    {
        if (_targetEnemy == null || projectileSpawnPositions.Count == 0) return;

        Vector3 enemyPosition = _targetEnemy.transform.position;
        Vector3 enemyVelocity = Vector3.zero;

        Rigidbody enemyRb = _targetEnemy.GetComponent<Rigidbody>();
        if (enemyRb != null)
        {
            enemyVelocity = enemyRb.velocity;
        }

        float projectileSpeed = projectilePrefab.GetComponent<TurretProjectile>().speed;
        Vector3 directionToEnemy = enemyPosition - turretHead.transform.position;
        float distanceToEnemy = directionToEnemy.magnitude;

        float timeToHit = shootMode == ShootMode.Raycast ? 0f : distanceToEnemy / projectileSpeed;
        Vector3 predictedPosition = enemyPosition + enemyVelocity * timeToHit;

        Vector3 direction = predictedPosition - turretHead.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Quaternion finalRotation = lookRotation * Quaternion.Euler(headRotationOffset);

        if (lockRotationOnObstacle && Time.time >= nextObstacleCheckTime)
        {
            isRotationLocked = IsPathBlocked();
            nextObstacleCheckTime = Time.time + OBSTACLE_CHECK_INTERVAL;
        }

        if (!lockRotationOnObstacle || !isRotationLocked)
        {
            if (lockPitchRotation)
            {
                // Preserve current pitch (X-axis) and only update yaw (Y-axis)
                Vector3 currentEuler = turretHead.transform.rotation.eulerAngles;
                Vector3 targetEuler = finalRotation.eulerAngles;
                Quaternion lockedPitchRotation = Quaternion.Euler(currentEuler.x, targetEuler.y, currentEuler.z);
                turretHead.transform.rotation = Quaternion.Slerp(turretHead.transform.rotation, lockedPitchRotation, Time.deltaTime * turretRotationSpeed);
            }
            else
            {
                // Full rotation (both pitch and yaw)
                turretHead.transform.rotation = Quaternion.Slerp(turretHead.transform.rotation, finalRotation, Time.deltaTime * turretRotationSpeed);
            }
            IsRotating?.Invoke();
        }

        float angleDifference = Quaternion.Angle(turretHead.transform.rotation, finalRotation);
        if (Time.time >= nextFireTime && angleDifference <= maxAngleDeviation)
        {
            if (firingMode == FiringMode.Sequential)
            {
                if (CanShoot(projectileSpawnPositions[currentSpawnIndex]))
                {
                    Shoot(projectileSpawnPositions[currentSpawnIndex]);
                    currentSpawnIndex = (currentSpawnIndex + 1) % projectileSpawnPositions.Count;
                    nextFireTime = Time.time + 1f / turretFireRate;
                }
            }
            else // Simultaneous
            {
                bool shotFired = false;
                foreach (Transform spawnPoint in projectileSpawnPositions)
                {
                    if (CanShoot(spawnPoint))
                    {
                        Shoot(spawnPoint);
                        shotFired = true;
                    }
                }
                if (shotFired)
                {
                    nextFireTime = Time.time + 1f / turretFireRate;
                }
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(enemyTag))
        {
            enemiesInRange.Add(other.gameObject);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(enemyTag))
        {
            enemiesInRange.Remove(other.gameObject);
            if (other.gameObject == _targetEnemy)
            {
                _targetEnemy = null;
                Debug.Log("Target exited range");
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (showRange)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, turretRangeRadius);
        }

        if (useConeDetection)
        {
#if UNITY_EDITOR
            Vector3 coneOrigin = coneMode == ConeMode.HeadAttached && turretHead != null ? turretHead.transform.position : transform.position;
            Vector3 coneUp = coneMode == ConeMode.HeadAttached && turretHead != null ? turretHead.transform.up : transform.up;
            Quaternion offsetRotation = Quaternion.Euler(coneOffset);
            Vector3 coneForward = offsetRotation * (coneMode == ConeMode.HeadAttached && turretHead != null ? turretHead.transform.forward : transform.forward);
            Handles.color = coneGizmoColor;
            Handles.DrawSolidArc(
                coneOrigin,
                coneUp,
                Quaternion.Euler(0, -coneAngle / 2f, 0) * coneForward,
                coneAngle,
                turretRangeRadius
            );
#endif
        }

        foreach (Transform spawnPoint in projectileSpawnPositions)
        {
            if (spawnPoint != null)
            {
                Vector3 spawnPosition = spawnPoint.position + spawnPoint.TransformDirection(spawnPointOffset);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(spawnPosition, 0.1f);

                Quaternion spawnRotation = Quaternion.Euler(spawnAngleOffset) * (turretHead != null ? turretHead.transform.rotation : transform.rotation);
                Gizmos.color = Color.green;
                Vector3 direction = spawnRotation * Vector3.forward;
                Gizmos.DrawRay(spawnPosition, direction * 1f);
            }
        }

        if (enableObstacleCheck && projectileSpawnPositions.Count > 0)
        {
            foreach (Transform spawnPoint in projectileSpawnPositions)
            {
                if (spawnPoint != null && turretHead != null)
                {
                    Vector3 spawnPosition = spawnPoint.position + spawnPoint.TransformDirection(spawnPointOffset);
                    Quaternion spawnRotation = turretHead.transform.rotation * Quaternion.Euler(spawnAngleOffset);
                    Vector3 raycastDirection = spawnRotation * Vector3.forward;
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawRay(spawnPosition, raycastDirection * turretRangeRadius);
                }
            }
        }
    }

    private IEnumerator DisableLightAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (muzzleLight != null)
        {
            muzzleLight.intensity = 0f;
        }
    }
}
}