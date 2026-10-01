using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedPowerDoor : MonoBehaviour
{
    public bool NeedPowerSwitch1 = false;
    public bool NeedPowerSwitch2 = false;
    public bool NeedPowerSwitch3 = false;

    public GameObject balll1, balll2, balll3;
    public Material greenMat;

    public bool DoorisOpen = false;

    public static NeedPowerDoor Instance;

    private void Awake()
    {
        Instance = this;
    }

   
    public void getPowerSwitch1()
    {
        NeedPowerSwitch1 = true;
        balll1.GetComponent<Renderer>().material = greenMat;
    }

    public void getPowerSwitch2()
    {
        NeedPowerSwitch2 = true;
        balll2.GetComponent<Renderer>().material = greenMat;
    }

    public void getPowerSwitch3()
    {
        NeedPowerSwitch3 = true;
        balll3.GetComponent<Renderer>().material = greenMat;
    }

    private void Update()
    {
        if (NeedPowerSwitch1 && NeedPowerSwitch2 && NeedPowerSwitch3)
        {
            DoorisOpen = true;

        }
    }
}
