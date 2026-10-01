using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BottleColor
{
    Blue,
    Yellow,
    Green,
    Purple
}

public class chemistryGameManager : MonoBehaviour
{
    public GameObject lock_;
    public Animator lockAnimator;

    [Header("正确的颜色顺序")]
    public List<BottleColor> correctOrder = new List<BottleColor>
    {
        BottleColor.Green,
        BottleColor.Purple,
        BottleColor.Blue,
        BottleColor.Yellow
    };

    private List<BottleColor> currentOrder = new List<BottleColor>();
    private BottleController currentlySelectedBottle;

    public static chemistryGameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSelectedBottle(BottleController bottle)
    {
        // 取消之前选中的瓶子
        if (currentlySelectedBottle != null && currentlySelectedBottle != bottle)
        {
            currentlySelectedBottle.SetSelected(false);
        }

        currentlySelectedBottle = bottle;
    }

    public void AddColorToEmptyBottle(BottleColor color, EmptyBottleController emptyBottle)
    {
       for (int i = 0; i < currentOrder.Count; i++)
        {
            if (currentOrder[i] == color)
            {
                // 已经有颜色了，不再加入
                return;
            }
        }
        // 加入当前顺序   
        currentOrder.Add(color);
        // 检查顺序是否正确
        if (currentOrder.Count == 4)
        {
            if (CheckOrder())
            {
                if (currentOrder.Count == correctOrder.Count)
                {
                    // 全部正确，播放动画
                    emptyBottle.PlaySuccessAnimation();
                    Invoke("openbox", 1.5f);
                    //   Debug.Log("恭喜！顺序完全正确！");
                }
            }
            else
            {
                // 顺序错误，重置
                ResetGame();
            }
        }
       

    }


    void openbox()
    {
        lock_.gameObject.SetActive(false);
        Invoke("openbox1", 0.5f);
    }
    void openbox1()
    {
        lockAnimator.SetTrigger("open");

    }
    private bool CheckOrder()
    {
        for (int i = 0; i < currentOrder.Count; i++)
        {
            if (currentOrder[i] != correctOrder[i])
            {
              //  Debug.Log($"顺序错误！第{i + 1}个应该是{correctOrder[i]}，但加入了{currentOrder[i]}");
                return false;
            }
        }
        return true;
    }

    private void ResetGame()
    {
        Debug.Log("顺序错误，重新开始！");
        currentOrder.Clear();

        // 重置所有瓶子状态
        BottleController[] allBottles = FindObjectsOfType<BottleController>();
        foreach (BottleController bottle in allBottles)
        {
            bottle.ResetBottle();
        }

        EmptyBottleController emptyBottle = FindObjectOfType<EmptyBottleController>();
        if (emptyBottle != null)
        {
            emptyBottle.ResetBottle();
        }
    }

    public BottleController GetSelectedBottle()
    {
        return currentlySelectedBottle;
    }
    public void hideMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

}