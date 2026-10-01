using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class darkness2 : MonoBehaviour
{
    public Animator lights;
    public static darkness2 instance;
    private void Awake()
    {
        instance = this;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            lights.SetBool("Open", true);
         //   print("open");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            lights.SetBool("Open", false);
          //  print("close");
        }
    }
    public void closeLight()
    {
        lights.SetBool("Open", false);
    
    }
}
