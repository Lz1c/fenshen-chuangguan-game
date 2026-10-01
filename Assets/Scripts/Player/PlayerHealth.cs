using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("生命值设置")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("回血设置")]
    public float healDelay = 5f; // 受伤后开始回血的延迟时间
    public float healInterval = 1f; // 回血间隔（秒）
    public int healAmount = 5; // 每次回血量

    [Header("UI引用")]
    public Slider healthSlider;
    public Image healEffectImage; // 可选：回血时的视觉效果

    // 私有变量
    private float lastDamageTime; // 最后受伤时间
    private bool isHealing = false; // 是否正在回血
    private Coroutine healingCoroutine; // 回血协程

    public static PlayerHealth instance;
    public Animator dieanimator; 
     Transform respawnPoint;
    void Start()
    {
        currentHealth = maxHealth;

        // 如果没有手动赋值，尝试自动查找
        if (healthSlider == null)
            healthSlider = GameObject.Find("Canvas/HpSlider")?.GetComponent<Slider>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        lastDamageTime = -healDelay; // 初始设置为可以立即回血
    }

    void Update()
    {
        CheckAndStartHealing();
    }

    public void TakeDamage(int damage,Transform respawn)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        // 更新UI
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        // 记录受伤时间
        lastDamageTime = Time.time;

        // 停止正在进行的回血
        if (healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine);
            isHealing = false;
        }

    
        if (healEffectImage != null)
        {
            healEffectImage.color = new Color(1, 0, 0, 0.3f); // 红色闪动
            StartCoroutine(FadeOutDamageEffect());
        }

        Debug.Log($"玩家受到 {damage} 点伤害，剩余生命值: {currentHealth}");

        if (currentHealth <= 0)
        {
            respawnPoint = respawn;
            Die();
        }
    }

    void CheckAndStartHealing()
    {
        // 如果生命值已满或正在回血，不需要处理
        if (currentHealth >= maxHealth || isHealing)
            return;

        // 检查是否过了回血延迟时间
        if (Time.time - lastDamageTime >= healDelay)
        {
            StartHealing();
        }
    }

    void StartHealing()
    {
        if (healingCoroutine != null)
            StopCoroutine(healingCoroutine);

        healingCoroutine = StartCoroutine(HealingRoutine());
    }

    IEnumerator HealingRoutine()
    {
        isHealing = true;

        while (currentHealth < maxHealth)
        {
            // 再次检查是否在回血期间受伤
            if (Time.time - lastDamageTime < healDelay)
            {
                isHealing = false;
                yield break;
            }

            // 执行回血
            Heal(healAmount);

            // 等待回血间隔
            yield return new WaitForSeconds(healInterval);
        }

        isHealing = false;
    }

    void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        // 更新UI
        if (healthSlider != null)
            healthSlider.value = currentHealth;


       // Debug.Log($"回复 {amount} 点生命值，当前生命值: {currentHealth}");
    }

    IEnumerator FadeOutDamageEffect()
    {
        Image effect = healEffectImage;
        float duration = 0.5f;
        float elapsed = 0f;
        Color startColor = effect.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / duration);
            effect.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        effect.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
    }

    void Die()
    {
        Debug.Log("玩家死亡！");

        // 停止所有协程
        if (healingCoroutine != null)
            StopCoroutine(healingCoroutine);
        dieanimator.SetTrigger("die");
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;
        currentHealth = maxHealth;
        healthSlider.value = currentHealth;

        darkness2.instance.closeLight();

    }

}