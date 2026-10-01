using UnityEngine;
using UnityEngine.Events;

namespace IronSamiStudio.AdvancedTurretAI {
public class HealthSystem : MonoBehaviour
{

    public float maxHealth = 100f;
    public float currentHealth = 0f;
    public bool fillHealthOnStartUp = true;
    public UnityEvent<GameObject> onDeath;

    // Start is called before the first frame update
    void Start()
    {
        if(fillHealthOnStartUp == true)
        { 
            currentHealth = maxHealth; // Fill health on start of scene 
        }
        
    }

    void Update()
    {
        if(currentHealth < 0){
            onDeath?.Invoke(gameObject);
        }
    }

    public void RemoveHP(GameObject enemy, float amount)
    {
        HealthSystem _enemy = enemy.GetComponent<HealthSystem>();
        
        _enemy.currentHealth -= amount;
    }

    public void DestroyThis(GameObject enemy)
    {
        Destroy(enemy);
    }


} 

}