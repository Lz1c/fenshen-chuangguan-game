using UnityEngine;

public class EmptyBottleController : MonoBehaviour
{
    [Header("空瓶设置")]
    public GameObject fillObject; // 填充物的子对象

    private Animator animator;
    private Vector3 originalScale;




    void Start()
    {
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;

        if (fillObject == null && transform.childCount > 0)
        {
            fillObject = transform.GetChild(0).gameObject;
        }

        // 开始时隐藏填充物
        if (fillObject != null)
        {
            fillObject.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        BottleController selectedBottle = chemistryGameManager.Instance.GetSelectedBottle();

        if (selectedBottle != null)
        {
            // 隐藏选中瓶子的液体
            selectedBottle.HideLiquid();

            // 显示空瓶的填充物
            if (fillObject != null)
            {
                fillObject.SetActive(true);
            }

            chemistryGameManager.Instance.AddColorToEmptyBottle(selectedBottle.bottleColor, this);

        }
    }

    public void PlaySuccessAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Success");
        }

       
    }


    public void ResetBottle()
    {
        if (fillObject != null)
        {
            fillObject.SetActive(false);
        }

        transform.localScale = originalScale;


        // 停止所有动画
        if (animator != null)
        {
            animator.Rebind();
        }

        StopAllCoroutines();
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }
}