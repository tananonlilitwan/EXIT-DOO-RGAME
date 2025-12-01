using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndCreditController : MonoBehaviour
{
    public TextMeshProUGUI[] creditTexts; 
    [SerializeField] public float scrollSpeed;
    public Button continueButton;

    private bool isScrolling = true;
    private RectTransform canvasRect;

    void Start()
    {
        SoundManager.Instance.EndCreditSound();
        canvasRect = creditTexts[0].canvas.GetComponent<RectTransform>();
        continueButton.gameObject.SetActive(false); 
    }

    void Update()
    {
        if (!isScrolling) return;

        // เลื่อน Text ทุกตัวขึ้น
        foreach (var text in creditTexts)
        {
            RectTransform rect = text.GetComponent<RectTransform>();
            rect.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
        }

        // Text ตัวสุดท้ายพ้นจอด้านบนหรือยัง
        RectTransform lastRect = creditTexts[creditTexts.Length - 1].GetComponent<RectTransform>();
        if (lastRect.anchoredPosition.y > canvasRect.rect.height)
        {
            isScrolling = false;
            continueButton.gameObject.SetActive(true);
            SoundManager.Instance.StopEndCreditSound();
        }
    }
}