using PrototypeFPC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClonePlayer : MonoBehaviour
{
    public GameObject cubePrefab; // 立方体预制体
    public float spawnInterval = 3f; // 生成间隔
    public float destroyDelay = 10f; // 销毁延迟


    [Header("UI设置")]
    public Text countdownText; // 倒计时文本UI
  
    private float currentTime; // 当前剩余时间
    private bool isCounting = true; // 是否正在计时
    private GameObject SpawnAll; // 存储所有生成的立方体
    private Coroutine spawnCoroutine; // 生成协程的引用
    private GameObject lights; // 灯光

    void Start()
    {
       
       
       
    }
    void OnEnable()
    {
        isCounting = true;
        countdownText = GameObject.Find("Canvas/CountdownText").GetComponent<Text>();
        // 灯光
        lights = GameObject.Find("lights");
        countdownText.text = "分身剩余: 10秒";
        countdownText.gameObject.SetActive(true);
       

        // 初始化计时器
        currentTime = destroyDelay;

        // 创建存储所有立方体的空对象
        SpawnAll = new GameObject("SpawnAll");

        // 启动生成虚影协程
        spawnCoroutine = StartCoroutine(SpawnCubesRoutine());

        // 更新UI显示
        UpdateCountdownUI();

        GameObject cube = Instantiate(cubePrefab);
        cube.transform.position = transform.position;
        cube.transform.rotation = transform.rotation;
        cube.transform.parent = SpawnAll.transform;
    }

    void Update()
    {
        // 更新计时器
        if (isCounting)
        {
            currentTime -= Time.deltaTime;
            UpdateCountdownUI();

            // 检查是否到达0秒
            if (currentTime <= 0f)
            {
                currentTime = 0f;
                AutoDestroy();
            }
        }

        // 监听键盘输入
        if (Input.GetKeyDown(KeyCode.E) && isCounting)
        {
            ManualDestroy();
        }


    }

    IEnumerator SpawnCubesRoutine()
    {
        while (true)
        {
            // 等待生成间隔
            yield return new WaitForSeconds(spawnInterval);

            if (SpawnAll != null && isCounting)
            {
                // 生成立方体
                GameObject cube = Instantiate(cubePrefab);
                cube.transform.position = transform.position;
                cube.transform.rotation = transform.rotation;
                cube.transform.parent = SpawnAll.transform;
            }
        }
    }

    // 更新倒计时UI
    void UpdateCountdownUI()
    {
        if (countdownText != null)
        {
            countdownText.text = $"分身剩余: {currentTime:F0}秒";
        }
    }

    // 手动销毁（按E键）
    void ManualDestroy()
    {
        if (!isCounting) return;

        isCounting = false;
        StopAllCoroutines();

        // 销毁所有生成的立方体
        if (SpawnAll != null)
            Destroy(SpawnAll);




        print(transform.localPosition);
        print(PlayerControl.instance.oldpoint);
        Invoke("stopClone", 0.2f);

        // 显示主角

        // player.SetActive(true);
        GetComponent<ClonePlayer>().enabled = false;
        gameObject.tag = "Player";

     

        // 更新UI显示
        if (countdownText != null)
        {
            countdownText.text = "分身结束";
            Invoke("hideTimeText", 2f);
        }
        transform.parent.gameObject.SetActive(false);

        transform.localPosition = PlayerControl.instance.oldpoint;
        transform.parent.gameObject.SetActive(true);
        // 销毁自己
        // Destroy(transform.parent.parent.gameObject);
    }

    // 自动销毁（10秒后）
    void AutoDestroy()
    {
        if (!isCounting) return;

       /* player.transform.position = transform.position;
        player.transform.GetChild(0).rotation = transform.GetChild(0).rotation;

        player.SetActive(true);*/

        isCounting = false;
        StopAllCoroutines();
      

        // 销毁所有生成的立方体
        if (SpawnAll != null)
            Destroy(SpawnAll);

/*        // 显示主角
        if (player != null)
            player.SetActive(true);*/

        // 更新UI显示
        if (countdownText != null)
        {
            countdownText.text = "分身结束";
            Invoke("hideTimeText", 2f);
        }

        GetComponent<ClonePlayer>().enabled = false;
        gameObject.tag = "Player";
        Invoke("stopClone", 0.2f);
        // 销毁自己
        //  Destroy(transform.parent.parent.gameObject);

    }
    void stopClone()
    {
        PlayerControl.instance.isClone = false;

    }
    void hideTimeText()
    {
        countdownText.text = "";
    }
    // 在销毁时清理
    void OnDestroy()
    {
        // 确保停止所有协程
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.name== "darkness")
        {
            foreach (Transform child in lights.transform)
            {
                child.gameObject.SetActive(false);
            }
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "darkness")
        {
            foreach (Transform child in lights.transform)
            {
                child.gameObject.SetActive(true);
            }
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }


}