using UnityEngine;

public class BottleController : MonoBehaviour
{
    [Header("瓶子设置")]
    public BottleColor bottleColor;
    public GameObject liquidObject; // 液体的子对象

    private Vector3 originalScale;
    private bool isSelected = false;

    void Start()
    {
        originalScale = transform.localScale;

        if (liquidObject == null && transform.childCount > 0)
        {
            liquidObject = transform.GetChild(0).gameObject;
        }
    }

    void OnMouseDown()
    {
        if (liquidObject != null && liquidObject.activeInHierarchy)
        {
            ToggleSelection();
        }
    }

    private void ToggleSelection()
    {
        isSelected = !isSelected;
        SetSelected(isSelected);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (isSelected)
        {
            transform.localScale = originalScale * 1.2f;
            chemistryGameManager.Instance.SetSelectedBottle(this);
        }
        else
        {
            transform.localScale = originalScale;
        }
    }

    public void HideLiquid()
    {
        if (liquidObject != null)
        {
            liquidObject.SetActive(false);
            SetSelected(false);
        }
    }

    public void ShowLiquid()
    {
        if (liquidObject != null)
        {
            liquidObject.SetActive(true);
        }
    }

    public void ResetBottle()
    {
        ShowLiquid();
        SetSelected(false);
    }
}