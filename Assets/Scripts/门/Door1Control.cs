using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door1Control : MonoBehaviour
{
    public static Door1Control instance;
    public Animator Door1Animator;
    //开关1
    public bool switch1 = false;
    public Animator Switch1Animator;
    //开关2
    public bool switch2 = false;
    public Animator Switch2Animator;

    public float closeTime = 10f;

    bool door1IsOpen = false;
    private Coroutine switch1Coroutine;
    private Coroutine switch2Coroutine;
    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckDoor1();//检测门1
    }
    void CheckDoor1()
    {
        if (switch1 == true && switch2 == true)
        {
            if (door1IsOpen == false)
            {
                door1IsOpen = true;
                Door1Animator.SetTrigger("open");

            }
        }

    }
    public void Switch1()
    {
        Switch1Animator.SetTrigger("open");
        switch1 = true;
      switch1Coroutine= StartCoroutine(CloseSwitch1());
    }
    public void Switch2()
    {
        Switch2Animator.SetTrigger("open");
        switch2 = true;
      switch2Coroutine =  StartCoroutine(CloseSwitch2());
    }


    IEnumerator CloseSwitch1()
    {
        
        yield return new WaitForSeconds(closeTime);
        if (door1IsOpen == true)
        {
         
            StopCoroutine(switch1Coroutine);
            yield return 0;
        }
        switch1 = false;
        Switch1Animator.SetTrigger("close");
        print("switch1 is closed");
    }
    IEnumerator CloseSwitch2()
    {
      
        yield return new WaitForSeconds(closeTime);
        if (door1IsOpen == true)
        {
            StopCoroutine(switch2Coroutine);
            yield return 0;
        }

        switch2 = false;
        Switch2Animator.SetTrigger("close");
        print("switch2 is closed");
    }
}
