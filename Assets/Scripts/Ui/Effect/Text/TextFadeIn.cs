using UnityEngine;
using TMPro;
using System.Collections;

public class TextFadeIn : MonoBehaviour
{
    public TextMeshProUGUI[] textsToFade;
    public float fadeDuration = 2f;

    private void Start()
    {
        foreach (var text in textsToFade)
        {
            if (text != null)
            {
                StartCoroutine(FadeInText(text));
            }
        }
    }

    private IEnumerator FadeInText(TextMeshProUGUI tmpText)
    {
        Color originalColor = tmpText.color;
        Color tempColor = originalColor;
        tempColor.a = 0f;
        tmpText.color = tempColor;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            tempColor.a = alpha;
            tmpText.color = tempColor;
            yield return null;
        }

        tmpText.color = originalColor;
    }
}