using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clonedoor : MonoBehaviour
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
    void CheckCenterTarget()
    {
        // 从屏幕中心发射射线
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        Physics.Raycast(ray, out hit);
        if (hit.collider == null) return;

        /// <summary>
        /// 分身找到开关2
        /// </summary>
        if (hit.collider.gameObject.name == "Door1Switch2")
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
