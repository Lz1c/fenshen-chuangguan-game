using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackManager : MonoBehaviour
{
    public GameObject propPrefab;
    public static BackpackManager instance;
    public GameObject itemPrefab;
    private void Awake()
    {
        instance = this;
    }
   public void Additem(Sprite item)
    {
        foreach(Transform child in transform)
        {
            if (child.childCount==0)
            {
               GameObject prop = Instantiate(propPrefab);
               prop.transform.SetParent(child);
                prop.transform.localPosition = Vector3.zero;
                prop.GetComponent<Image>().sprite = item;
                break;
            }
        }
    }
    public void usekey1()
    {
        foreach (Transform child in transform)
        {
            if (child.childCount > 0)
            {
                if (child.GetChild(0).GetComponent<Image>().sprite.name == "key1")
                {
                    Destroy(child.gameObject);
                    Instantiate(itemPrefab,transform);
                    break;
                }
            }
        }
    }
    public void usekey2()
    {
        foreach (Transform child in transform)
        {
            if (child.childCount > 0)
            {
                if (child.GetChild(0).GetComponent<Image>().sprite.name == "key2")
                {
                    Destroy(child.gameObject);
                    Instantiate(itemPrefab, transform);
                    break;
                }
            }
        }
    }
    public void usekey3()
    {
        foreach (Transform child in transform)
        {
            if (child.childCount > 0)
            {
                if (child.GetChild(0).GetComponent<Image>().sprite.name == "key3")
                {
                    Destroy(child.gameObject);
                    Instantiate(itemPrefab, transform);
                    break;
                }
            }
        }
    }


}
