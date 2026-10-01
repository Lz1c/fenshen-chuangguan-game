using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class log_ : MonoBehaviour
{
   public void hideMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GetComponent<Image>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        pointControl.instance.startcam();
    }
    

}
