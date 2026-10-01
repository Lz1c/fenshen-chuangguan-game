using UnityEngine;

public class RewindSkill : MonoBehaviour
{
    [Header("设置")]
    public KeyCode teleportKey = KeyCode.E; // 触发按键
    public float duration = 10f;            // 保存位置的持续时间（秒）

    private Vector3 savedPosition;          // 保存的世界坐标
    private bool hasSaved = false;          // 是否有保存的位置
    private float timer = 0f;               // 倒计时
    private Rigidbody rb;                   // 玩家刚体

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("未找到 Rigidbody，请确认玩家物体上有 Rigidbody 组件！");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(teleportKey))
        {
            if (!hasSaved)
            {
                // 保存当前位置（世界坐标）
                savedPosition = transform.position;
                hasSaved = true;
                timer = duration;
                Debug.Log("位置已保存: " + savedPosition);
            }
            else
            {
                // 传送回保存的位置
                TeleportToSavedPosition();
            }
        }

        // 如果有保存位置，就倒计时
        if (hasSaved)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                // 超时清空
                hasSaved = false;
                Debug.Log("保存位置已失效");
            }
        }
    }

    private void TeleportToSavedPosition()
    {
        if (rb != null)
        {
            // 1. 停止一切移动
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero; // 如果有旋转也清掉

            // 2. 传送
            rb.position = savedPosition;
        }
        else
        {
            transform.position = savedPosition;
        }

        hasSaved = false; // 用完就清空
        Debug.Log("已传送回保存位置: " + savedPosition);
    }
}
