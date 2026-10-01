using UnityEngine;
using System.Collections;

public class TurretController : MonoBehaviour
{
    [Header("扫描设置")]
    public float scanAngle = 90f; // 扫描扇形角度
    public float scanSpeed = 30f; // 扫描速度(度/秒)

    [Header("攻击设置")]
    public float detectionRange = 10f; // 检测范围
    public float attackPreparationTime = 0.25f; // 攻击间隔
    //第一次攻击间隔时间
    public float firstAttackInterval = 1.2f;
                                            

    [Header("子弹设置")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 15f;
    public int bulletDamage = 10;
    [Header("复活点")]
    public Transform respawnPoint;
    public bool isStart = true;

    [Header("可视化")]
    public bool showGizmos = true;
    public Color scanColor = new Color(0, 1, 0, 0.2f);
    public Color attackColor = new Color(1, 0, 0, 0.3f);

    // 状态变量
    private enum TurretState { Scanning, Preparing, Attacking }
    private TurretState currentState = TurretState.Scanning;

    private Transform player;
    private float currentScanAngle = 0f;
    private bool scanningRight = true;
    private float preparationTimer = 0f;// 攻击准备计时器

    private float attackCooldown = 0f;

 
  


    // 初始旋转
    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.rotation;
        // 查找玩家
      //  player = GameObject.FindGameObjectWithTag("Player").transform;

    }

    void Update()
    {
        if (!isStart)return;
        switch (currentState)
        {
            case TurretState.Scanning:
                UpdateScanning();
                break;
            case TurretState.Preparing:
                UpdatePreparing();
                break;
            case TurretState.Attacking:
                UpdateAttacking();
                break;
        }
    }

    void UpdateScanning()
    {
        // 扫描移动
        float scanDirection = scanningRight ? 1f : -1f;
        currentScanAngle += scanDirection * scanSpeed * Time.deltaTime;

        // 限制扫描角度
        if (Mathf.Abs(currentScanAngle) >= scanAngle / 2f)
        {
            currentScanAngle = Mathf.Clamp(currentScanAngle, -scanAngle / 2f, scanAngle / 2f);
            scanningRight = !scanningRight;
        }

        // 应用旋转
        transform.rotation = initialRotation * Quaternion.Euler(0, currentScanAngle, 0);

        // 检测玩家
        if (IsPlayerInSight())
        {
            currentState = TurretState.Preparing;
        
            Invoke("startAtk", firstAttackInterval);
        }
    }

    void UpdatePreparing()
    {


        // 检查玩家是否离开
        if (player == null  || !IsPlayerInSight())
        {
            currentState = TurretState.Scanning;
          
            return;
        }
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) return;

        // 转向玩家
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);

    }
    void startAtk()
    {
        currentState = TurretState.Attacking;
        attackCooldown = 0f;
    }

    void UpdateAttacking()
    {

        attackCooldown -= Time.deltaTime;

        // 检查停止条件

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null )return;

        // 持续转向玩家
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 8f * Time.deltaTime);

        // 射击逻辑
        if (attackCooldown <= 0f && IsPlayerInSight())
        {
            Shoot();
            attackCooldown = attackPreparationTime;

        }

    }

    void Shoot()
    {
      
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bulletComponent = bullet.GetComponent<Bullet>();

        if (bulletComponent != null)
        {
            bulletComponent.pos = respawnPoint;
            bulletComponent.Initialize(bulletSpeed, bulletDamage);
        }
    }

    bool IsPlayerInSight()//是否在视野范围内
    {
        
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) return false;
        

        Vector3 directionToPlayer = player.position - transform.position;

        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        return angle <= scanAngle / 2f &&
               IsPlayerInRange(detectionRange)
              ;
    }

    bool IsPlayerInRange(float range)
    {

      //  print(Vector3.Distance(transform.position, player.position) <= range);
        return Vector3.Distance(transform.position, player.position) <= range;
    }



    // 可视化调试
    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        // 绘制扫描扇形
        DrawSector(scanColor, detectionRange);

        // 绘制攻击范围
        Gizmos.color = attackColor;

    }

    void DrawSector(Color color, float range)
    {
        int segments = 30;
        float angleStep = scanAngle / segments;

        Gizmos.color = color;

        Vector3 forward = transform.forward;
        Vector3 startPoint = transform.position;

        for (int i = 0; i <= segments; i++)
        {
            float angle = -scanAngle / 2 + angleStep * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * forward;
            Vector3 endPoint = startPoint + dir * range;

            if (i > 0)
            {
                Gizmos.DrawLine(startPoint, endPoint);
            }

            if (i == 0 || i == segments)
            {
                Gizmos.DrawLine(startPoint, endPoint);
            }
        }

        // 绘制弧线
        Vector3 lastDir = Quaternion.Euler(0, -scanAngle / 2, 0) * forward;
        Vector3 lastPoint = startPoint + lastDir * range;

        for (int i = 1; i <= segments; i++)
        {
            float angle = -scanAngle / 2 + angleStep * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * forward;
            Vector3 currentPoint = startPoint + dir * range;

            Gizmos.DrawLine(lastPoint, currentPoint);
            lastPoint = currentPoint;
        }
    }
}