using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI buttonText;
    public Image buttonBackground;

    public Color normalTextColor = Color.white;
    public Color hoverTextColor = Color.red;

    public Color normalBGColor = new Color(0, 0, 0, 0.5f); // ดำโปร่งใส
    public Color hoverBGColor = new Color(0, 0, 0, 0.8f);  // ดำเข้มขึ้น

    private void Start()
    {
        if (buttonText != null)
        {
            buttonText.color = normalTextColor;
        }

        if (buttonBackground != null)
        {
            buttonBackground.color = normalBGColor;   
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = hoverTextColor;
        }
        
        if (buttonBackground != null)
        {
            buttonBackground.color = hoverBGColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = normalTextColor;
        }

        if (buttonBackground != null)
        {
            buttonBackground.color = normalBGColor;
        }
    }
}