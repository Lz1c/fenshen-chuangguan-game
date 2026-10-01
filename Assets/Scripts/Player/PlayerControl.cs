using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{

    public GameObject ClonePlayer; //克隆的预制体
     GameObject countdownText; //倒计时文本
    public GameObject lights;
    public Vector3 oldpoint;
    public static PlayerControl instance;
   public bool isClone = false;
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        countdownText = GameObject.Find("CountdownText");
       
    }
    private void OnEnable()
    {
        Invoke("checkCountdown", 2);
    }
    void Update()
    {
        if (isClone == false)
        {
            // 监听键盘输入
            if (Input.GetKeyDown(KeyCode.E))
            {
                GetComponent<ClonePlayer>().enabled = true;
                gameObject.tag = "Untagged";


              
                    isClone = true;
                    oldpoint = transform.localPosition;
                print(oldpoint + "1111111");
      
                

            }
        }
    }
    //检测倒计时是否为空
    void checkCountdown()
    {
        if (countdownText.GetComponent<Text>().text != null)
        {
            countdownText.GetComponent<Text>().text = null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "darkness")
        {
            foreach (Transform child in lights.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "darkness")
        {
          foreach (Transform child in lights.transform)
          {
              child.gameObject.SetActive(true);
          } 
        }
    }

 


}
