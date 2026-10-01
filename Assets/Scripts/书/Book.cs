using UnityEngine;
using System.Collections;

public class Book : MonoBehaviour
{
    [Header("Book Settings")]
    public int bookId; // 书的唯一标识 (1,2,3,4)
    public Vector3 originalPosition;
    public Vector3 originalScale;

    [Header("Animation Settings")]
    public float moveDuration = 0.8f;
    public float scaleDuration = 0.3f;
    public float selectedScale = 1.2f;

    private bool isSelected = false;
    private BookPuzzleManager manager;
    private Coroutine moveCoroutine;
    private Coroutine scaleCoroutine;


    void Start()
    {
        originalPosition = transform.position;
        originalScale = transform.localScale;
        manager = FindObjectOfType<BookPuzzleManager>();

        // 确保有Collider用于点击检测
        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();
    }

    void OnMouseDown()
    {
        if (manager != null)
            manager.OnBookClicked(this);
    }

    public void SelectBook()
    {
        if (isSelected) return;

        isSelected = true;
        ScaleTo(originalScale * selectedScale);
    }

    public void DeselectBook()
    {
        if (!isSelected) return;

        isSelected = false;
        ScaleTo(originalScale);
    }

    public void MoveToPosition(Vector3 targetPosition)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveRoutine(targetPosition));
    }

    public void ResetToOriginal()
    {
        transform.position = originalPosition;
        transform.localScale = originalScale;
        isSelected = false;
    }

    private IEnumerator MoveRoutine(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        // 添加一个轻微的弧线移动，让动画更自然
        Vector3 controlPoint = startPosition + (targetPosition - startPosition) * 0.5f + Vector3.up * 0.5f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;

            // 贝塞尔曲线移动
            transform.position = CalculateBezierPoint(t, startPosition, controlPoint, targetPosition);

            yield return null;
        }

        transform.position = targetPosition;
        originalPosition = targetPosition; // 更新原始位置
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        return uu * p0 + 2 * u * t * p1 + tt * p2;
    }

    public void ScaleTo(Vector3 targetScale)
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);

        scaleCoroutine = StartCoroutine(ScaleRoutine(targetScale));
    }

    private IEnumerator ScaleRoutine(Vector3 targetScale)
    {
        Vector3 startScale = transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < scaleDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / scaleDuration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;
    }

    public bool IsSelected => isSelected;
}