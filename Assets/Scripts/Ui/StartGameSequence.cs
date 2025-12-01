using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartGameSequence : MonoBehaviour
{
    public static StartGameSequence instance;
    public static bool isGameStarted = false;

    [Header("UI")]
    public Image uiFlash;
    [Tooltip("Script of GamecoverPanel")]
    public GameObject gamecoverPanel;
    public Image fadeImage;

    [Header("Timing")]
    public float flashDuration = 0.2f;
    public float fadeDuration = 1.0f;

    public float waitAfterFade = 1.1f;
    private bool isStarting = false;
    
    
    public void OnStartButtonPressed()
    {
        StartCoroutine(PlayStartSequence());
        if (isStarting) return;
        isStarting = true;  
    }

    private IEnumerator PlayStartSequence()
    {
        if (uiFlash != null)
        {
            uiFlash.color = new Color(1, 1, 1, 1);
            yield return new WaitForSecondsRealtime(flashDuration); //yield return new WaitForSeconds(flashDuration);
            uiFlash.color = new Color(1, 1, 1, 0);
        }

        SoundManager.Instance.PlaySoundEffect();
        SoundManager.Instance.PlayExitDoor();
        
        if (fadeImage != null)
        {
            float elapsed = 0f;
            Color originalColor = fadeImage.color;
            while (elapsed < fadeDuration)
            {
                float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                fadeImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }
            fadeImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        }
        
        yield return new WaitForSeconds(waitAfterFade);

        if (gamecoverPanel != null)
        {
            gamecoverPanel.SetActive(false);
            GameManager.Instance.ShowItemFlowerPanel();
        }
        
        SoundManager.Instance.PlayBGM();
        
        isGameStarted = true; 
        
    }
}
