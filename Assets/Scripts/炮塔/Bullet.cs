using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed;
    private int damage;
    private Rigidbody rb;
  public      Transform pos;

    public void Initialize(float bulletSpeed, int bulletDamage)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }

        // 3秒后自动销毁
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter(Collider other)
    {
        // 检查是否击中玩家
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage,pos);
            }

            // 击中后销毁子弹
            Destroy(gameObject);
        }
        // 击中墙壁或其他障碍物
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
