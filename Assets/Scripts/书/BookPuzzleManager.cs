using UnityEngine;
using System.Collections.Generic;

public class BookPuzzleManager : MonoBehaviour
{
    [Header("Book References")]
    public List<Book> books = new List<Book>();

    [Header("Target Order")]
    public int[] correctOrder = { 2, 4, 1, 3 }; // 正确顺序: 位置1=书2, 位置2=书4, 位置3=书1, 位置4=书3

    [Header("Positions")]
    public Transform[] bookPositions; // 4个位置点

    private Book firstSelectedBook = null;
    private Dictionary<int, int> currentOrder = new Dictionary<int, int>(); // 位置ID -> 书ID

    public static BookPuzzleManager instance;
    public bool iswin;
    public AudioSource winAudio;

    void Awake()
    {
        instance = this;
    }   
    void Start()
    {
        InitializeBooks();
        UpdateCurrentOrder();
        Debug.Log("初始顺序: " + GetCurrentOrderString());
    }

    void InitializeBooks()
    {
        // 确保书本按照ID顺序排列
        books.Sort((a, b) => a.bookId.CompareTo(b.bookId));

        // 为每本书分配初始位置
        for (int i = 0; i < books.Count && i < bookPositions.Length; i++)
        {
            books[i].originalPosition = bookPositions[i].position;
            books[i].transform.position = bookPositions[i].position;
            books[i].ResetToOriginal(); // 确保重置状态
        }
    }

    public void OnBookClicked(Book clickedBook)
    {
        if (firstSelectedBook == null)
        {
            // 第一次点击 - 选中书本
            firstSelectedBook = clickedBook;
            firstSelectedBook.SelectBook();
            Debug.Log($"选中书本: {clickedBook.bookId}");
        }
        else
        {
            // 第二次点击
            if (firstSelectedBook == clickedBook)
            {
                // 点击同一本书 - 取消选中
                firstSelectedBook.DeselectBook();
                firstSelectedBook = null;
                Debug.Log("取消选中");
            }
            else
            {
                // 交换两本书
                StartCoroutine(SwapBooks(firstSelectedBook, clickedBook));
            }
        }
    }

    private System.Collections.IEnumerator SwapBooks(Book book1, Book book2)
    {
        Debug.Log($"交换书本: {book1.bookId} <-> {book2.bookId}");

        // 获取目标位置
        Vector3 targetPos1 = book2.transform.position; // 使用当前位置而不是originalPosition
        Vector3 targetPos2 = book1.transform.position;

        // 交换originalPosition引用
        Vector3 tempPos = book1.originalPosition;
        book1.originalPosition = book2.originalPosition;
        book2.originalPosition = tempPos;

        // 同时移动两本书
        book1.MoveToPosition(targetPos1);
        book2.MoveToPosition(targetPos2);

        // 等待移动完成
        yield return new WaitForSeconds(book1.moveDuration);

        // 取消第一本书的选中状态
        book1.DeselectBook();
        firstSelectedBook = null;

        // 更新顺序并检查
        UpdateCurrentOrder();
        CheckPuzzleCompletion();
    }

    void UpdateCurrentOrder()
    {
        currentOrder.Clear();

        // 重新构建位置到书本ID的映射
        for (int i = 0; i < bookPositions.Length; i++)
        {
            Book bookAtPosition = FindBookAtPosition(bookPositions[i].position);
            if (bookAtPosition != null)
            {
                currentOrder[i + 1] = bookAtPosition.bookId; // 位置从1开始编号
            }
        }
    }

    Book FindBookAtPosition(Vector3 position)
    {
        float distanceThreshold = 0.5f; // 增加阈值以更好地检测

        Book closestBook = null;
        float closestDistance = float.MaxValue;

        foreach (Book book in books)
        {
            float distance = Vector3.Distance(book.transform.position, position);
            if (distance < distanceThreshold && distance < closestDistance)
            {
                closestBook = book;
                closestDistance = distance;
            }
        }
        return closestBook;
    }

    void CheckPuzzleCompletion()
    {
        bool isCorrect = true;

        // 检查每个位置的书本ID是否正确
        for (int i = 0; i < correctOrder.Length; i++)
        {
            int positionId = i + 1; // 位置ID从1开始
            if (!currentOrder.ContainsKey(positionId) || currentOrder[positionId] != correctOrder[i])
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            OnPuzzleCompleted();
        }
        else
        {
            Debug.Log("当前顺序: " + GetCurrentOrderString() + " | 目标: 2 4 1 3");
        }
    }

    void OnPuzzleCompleted()
    {
        // 拼图完成后的效果
        foreach (Book book in books)
        {
            book.ScaleTo(book.originalScale * 1.1f);
        }
        iswin = true;
        winAudio.Play();
        Debug.Log("恭喜！书本顺序正确！");
    }

    public string GetCurrentOrderString()
    {
        string order = "";
        for (int i = 1; i <= 4; i++)
        {
            if (currentOrder.ContainsKey(i))
                order += currentOrder[i] + " ";
            else
                order += "? ";
        }
        return order.Trim();
    }

/*    // 调试用：显示当前顺序
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("书本拼图游戏");
        GUILayout.Label("目标顺序: 2 4 1 3");
        GUILayout.Label("当前顺序: " + GetCurrentOrderString());

        if (firstSelectedBook != null)
            GUILayout.Label($"已选中: 书本{firstSelectedBook.bookId}");
        else
            GUILayout.Label("点击选择第一本书");

        GUILayout.EndArea();
    }*/
}