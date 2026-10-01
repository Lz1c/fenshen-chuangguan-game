using PrototypeFPC;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class pointControl : MonoBehaviour
{

    public static pointControl instance;
    [Header("检测设置")]

    public float maxDistance = 5f;
    public LayerMask detectionLayer = -1;

    private Camera mainCamera;
    private GameObject currentTarget; // 当前对准的物体

    [Header("道具设置")]
    Sprite key1Sprite;
    Sprite key2Sprite;
    Sprite key3Sprite;



    private Text DoortripText;
    private Text PowerSwitchText;
    private Text keytripText;
    private Text NeedkeyDoorText;
    private Text PowerDoortripText;
    private Text BooktripText;
    private Text chemistrytripText;

    //关卡5
    GameObject lights;//光
    GameObject turrets; //炮台
    bool istriggered = false;//是否触发
    GameObject box;//碰撞体

    [Header("监测日志本")]
    GameObject log1;
    GameObject log2;
    GameObject log3;
    GameObject log4;
    bool bookisChecked = false;

    //日志开的门
    Animator endDoorAnimator;

    //瓶子序列
     GameObject MedicalrecordUi;


    void Start()
    {
        instance = this;
        mainCamera = Camera.main;
        DoortripText = GameObject.Find("Canvas/DoortripText").GetComponent<Text>();
        PowerSwitchText = GameObject.Find("Canvas/PowerSwitchText").GetComponent<Text>();
        keytripText = GameObject.Find("Canvas/keytripText").GetComponent<Text>();
        NeedkeyDoorText = GameObject.Find("Canvas/NeedkeyDoorText").GetComponent<Text>();
        PowerDoortripText = GameObject.Find("Canvas/PowerDoortripText").GetComponent<Text>();
        BooktripText = GameObject.Find("Canvas/BooktripText").GetComponent<Text>();
        chemistrytripText = GameObject.Find("Canvas/chemistrytripText").GetComponent<Text>();
        key1Sprite = Resources.Load<Sprite>("key1");//加载钥匙图片,在Resources文件夹下
        key2Sprite = Resources.Load<Sprite>("key2");
        key3Sprite = Resources.Load<Sprite>("key3");
        lights = GameObject.Find("lights");
        turrets = GameObject.Find("turrets");
        log1 = GameObject.Find("Canvas/log1");
        log2 = GameObject.Find("Canvas/log2");
        log3 = GameObject.Find("Canvas/log3");
        log4 = GameObject.Find("Canvas/log4");
        MedicalrecordUi = GameObject.Find("Canvas/MedicalrecordUi");

        endDoorAnimator = GameObject.Find("enddoor").GetComponent<Animator>();

    }

    void Update()
    {
        CheckCenterTarget();//检测屏幕中心物体
        checkBook(); //检测书本排序是否完成
    }

    // 检测屏幕中心是否有物体
    void CheckCenterTarget()
    {
        // 从屏幕中心发射射线
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        Physics.Raycast(ray, out hit, maxDistance, detectionLayer);
        GameObject newTarget = null;

        /// <summary>
        /// 钥匙高亮
        /// </summary>
        if (hit.collider == null) return;
        if (hit.collider.name == "key1")
        {
            newTarget = hit.collider.gameObject;
            keytripText.text = "点击拾取钥匙";
            if (Input.GetMouseButtonDown(0))
            {
                // 点击钥匙
                newTarget.gameObject.SetActive(false);
                BackpackManager.instance.Additem(key1Sprite);
                NeedKeyDoor.instance.getkey1();
            }
        }
        else if (hit.collider.name == "key2")
        {
            newTarget = hit.collider.gameObject;
            keytripText.text = "点击拾取钥匙";
            if (Input.GetMouseButtonDown(0))
            {
                // 点击钥匙
                newTarget.gameObject.SetActive(false);
                BackpackManager.instance.Additem(key2Sprite);
                NeedKeyDoor.instance.getkey2();
            }
        }
        else if (hit.collider.name == "key3")
        {
            newTarget = hit.collider.gameObject;
            keytripText.text = "点击拾取钥匙";
            if (Input.GetMouseButtonDown(0))
            {
                // 点击钥匙
                newTarget.gameObject.SetActive(false);
                BackpackManager.instance.Additem(key3Sprite);
                NeedKeyDoor.instance.getkey3();
            }
        }
        else
        {
            keytripText.text = "";
        }

        if (newTarget != currentTarget)
        {
            // 隐藏之前的目标Outline
            if (currentTarget != null)
            {
                Outline outline = currentTarget.GetComponent<Outline>();

                outline.enabled = false;
            }

            // 显示新目标的Outline
            if (newTarget != null)
            {

                Outline outline = newTarget.GetComponent<Outline>();
                outline.enabled = true;
            }

            currentTarget = newTarget;
        }

        if (hit.collider.CompareTag("nokeydoor"))
        {
            DoortripText.text = "按F开门";
            DoortripText.enabled = true;

            if (Input.GetKeyDown(KeyCode.F))
            {
                hit.collider.GetComponent<Animator>().SetTrigger("open");
                print(hit.collider);
                hit.collider.tag = "Untagged";
                DoortripText.enabled = false;
            }
        }
        else
        {
            DoortripText.enabled = false;
        }
        // 三个电源门检测
        if (hit.collider.gameObject.name == "NeedpowerDoor")
        {
            if (!NeedPowerDoor.Instance.DoorisOpen)
            {
                PowerDoortripText.text = "需要打开三个电源才能打开这个门";
            }
            else
            {
                PowerDoortripText.text = "电源全部开启，按F开门";
                if (Input.GetKeyDown(KeyCode.F))
                {
                    hit.collider.GetComponent<Animator>().SetTrigger("open");

                    PowerDoortripText.text = "";
                    hit.collider.name = "NeedpowerDoor_opened";
                }
            }

        }
        else
        {
            PowerDoortripText.text = "";
        }


        // 三个电源开关检测
        if (hit.collider.name == "PowerSwitch1")
        {
            PowerSwitchText.text = "按F打开电源开关1";
            if (Input.GetKeyDown(KeyCode.F))
            {
                hit.collider.transform.parent.GetComponent<Animator>().SetTrigger("open");
                NeedPowerDoor.Instance.getPowerSwitch1();
                PowerSwitchText.text = "";
                foreach (Transform child in lights.transform)
                {
                    child.gameObject.SetActive(true);
                }
                foreach (Transform turret in turrets.transform)
                {
                    turret.gameObject.GetComponent<TurretController>().enabled = false;
                }
                if (istriggered)
                {
                    box.name = "Box";
                }
            }
        }
        else if (hit.collider.name == "PowerSwitch2")
        {
            PowerSwitchText.text = "按F打开电源开关2";
            if (Input.GetKeyDown(KeyCode.F))
            {
                hit.collider.transform.parent.GetComponent<Animator>().SetTrigger("open");
                NeedPowerDoor.Instance.getPowerSwitch2();
                PowerSwitchText.text = "";
            }
        }
        else if (hit.collider.name == "PowerSwitch3")
        {
            PowerSwitchText.text = "按F打开电源开关3";
            if (Input.GetKeyDown(KeyCode.F))
            {
                hit.collider.transform.parent.GetComponent<Animator>().SetTrigger("open");
                NeedPowerDoor.Instance.getPowerSwitch3();
                PowerSwitchText.text = "";
            }


        }
        else
        {
            PowerSwitchText.text = "";

        }

        //需要钥匙门检测
        if (hit.collider.tag == "NeedkeyDoor")
        {
            if (hit.collider.name == "NeedkeyDoor1")
            {
                if (NeedKeyDoor.instance.key1)
                {
                    NeedkeyDoorText.text = "有钥匙，按F开门";
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        hit.collider.GetComponent<Animator>().SetTrigger("open");
                        NeedkeyDoorText.text = "";
                        hit.collider.tag = "Untagged";
                        BackpackManager.instance.usekey1();
                    }
                }
                else
                {
                    NeedkeyDoorText.text = "没有钥匙";
                }
            }
            else if (hit.collider.name == "NeedkeyDoor2")
            {
                if (NeedKeyDoor.instance.key2)
                {
                    NeedkeyDoorText.text = "有钥匙，按F开门";
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        hit.collider.GetComponent<Animator>().SetTrigger("open");
                        NeedkeyDoorText.text = "";
                        hit.collider.tag = "Untagged";
                        BackpackManager.instance.usekey2();
                    }
                }
                else
                {
                    NeedkeyDoorText.text = "没有钥匙";
                }
            }
            else if (hit.collider.name == "NeedkeyDoor3")
            {
                if (NeedKeyDoor.instance.key3)
                {
                    NeedkeyDoorText.text = "有钥匙，按F开门";
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        hit.collider.GetComponent<Animator>().SetTrigger("open");
                        NeedkeyDoorText.text = "";
                        hit.collider.tag = "Untagged";
                        BackpackManager.instance.usekey3();
                    }
                }
                else
                {
                    NeedkeyDoorText.text = "没有钥匙";
                }
            }

        }
        else
        {
            NeedkeyDoorText.text = "";
        }

        //第七关书本检测
        if (hit.collider.name == "book1")
        {
            BooktripText.text = "按 R 查看内容";
            if (Input.GetKeyDown(KeyCode.R))
            {
                //显示鼠标
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                log1.transform.GetChild(0).gameObject.SetActive(true);
                log1.GetComponent<Image>().enabled = true;
                stopcam();
                BooktripText.text = "";
            }
            if (!bookisChecked)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    hit.collider.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                    bookisChecked = true;

                }
                else
                {

                }
            }
                

        }
        else if (hit.collider.name == "book2")
        {
            BooktripText.text = "按R查看内容";
            if (Input.GetKeyDown(KeyCode.R))
            {
                //显示鼠标
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                log2.transform.GetChild(0).gameObject.SetActive(true);
                log2.GetComponent<Image>().enabled = true;
                stopcam();
                BooktripText.text = "";
            }
        }
        else if (hit.collider.name == "book3")
        {
            BooktripText.text = "按R查看内容";
            if (Input.GetKeyDown(KeyCode.R))
            {
                //显示鼠标
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                log3.transform.GetChild(0).gameObject.SetActive(true);
                log3.GetComponent<Image>().enabled = true;
                BooktripText.text = "";
                stopcam();
            }
        }
        else if (hit.collider.name == "book4")
        {
            BooktripText.text = "按R查看内容";
            if (Input.GetKeyDown(KeyCode.R))
            {
                //显示鼠标
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                log4.transform.GetChild(0).gameObject.SetActive(true);
                log4.GetComponent<Image>().enabled = true;
                stopcam();
                BooktripText.text = "";
            }
        }
        else
        {
            BooktripText.text = "";
        }

        if (hit.collider.CompareTag("chemistry"))
        {
            chemistrytripText.text = "按R查看";
            if (Input.GetKeyDown(KeyCode.R))
            {
                stopcam();
                //显示鼠标
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                MedicalrecordUi.transform.GetChild(0).gameObject.SetActive(true);
              
            }
        }else
        {
            chemistrytripText.text = "";
        }

    }
    void HideTripText()
    {
        DoortripText.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "darkness")
        {
            istriggered = true;
            box = other.gameObject;
        }
     
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "darkness")
        {
            istriggered = false;
        }

    }

    void checkBook()
    {
        bool enddoor=false;
        if (GameObject.Find("BookPuzzleManager").GetComponent<BookPuzzleManager>().iswin)
        {
            if (!enddoor)
            {
                enddoor = true;
                endDoorAnimator.SetTrigger("open");
            }
        }
    }

    public void stopcam()
    {
        transform.parent.GetComponent<Perspective>().enabled = false;
    }

    public void startcam()
    {
        transform.parent.GetComponent<Perspective>().enabled = true;
    }

}
