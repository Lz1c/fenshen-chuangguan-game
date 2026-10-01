using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedKeyDoor : MonoBehaviour
{
   public static NeedKeyDoor instance;

 public bool key1, key2, key3;

   private void Awake()
    {
        instance = this;
    }

   public void getkey1()
    {
        key1 = true;
    }

   public void getkey2()
    {
        key2 = true;
    }

   public void getkey3()
    {
        key3 = true;
    }




}
