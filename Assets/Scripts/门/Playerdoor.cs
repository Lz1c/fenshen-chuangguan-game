using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Playerdoor : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject tripText;

    private void Start()
    {
        mainCamera = Camera.main;
        tripText = GameObject.Find("Canvas/TripText");
        tripText.GetComponent<Text>().enabled = false;


    }
    private void Update()
    {
        CheckCenterTarget();
    }
    // 检测屏幕中心是否有物体
    void CheckCenterTarget()
    {
        // 从屏幕中心发射射线
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        Physics.Raycast(ray, out hit);
        if (hit.collider == null) return;
        /// <summary>
        /// 主角找到开关1
        /// </summary>
        if (hit.collider.gameObject.name == "Door1Switch")
        {
            tripText.GetComponent<Text>().enabled = true;

            if (Input.GetKeyDown(KeyCode.F))
            {
                Door1Control.instance.Switch1();
            }

        }
        else if (hit.collider.gameObject.name == "Door1Switch2")
        {
            tripText.GetComponent<Text>().enabled = true;

            if (Input.GetKeyDown(KeyCode.F))
            {
                Door1Control.instance.Switch2();
            }
        }
        else
        {
            tripText.GetComponent<Text>().enabled = false;
        }


    }
}