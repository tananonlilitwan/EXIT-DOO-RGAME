using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject winPanel;
    public GameObject losePanel;
    
    [Header("UI NoteDialogPanel")]
    public GameObject noteDialogPanel;
    public GameObject pressEText;
    public GameObject noteText;
    
    [Header("UI Next level Panel")]
    public GameObject nextLevelPanel; 
    
    [Header("UI Pause game Panel ")]
    public GameObject pausePanel;
    
    [Header("UI ItemDialogPanel ")]
    public GameObject ItemDialogPanel;
    private bool cursorHidden = false;
    
    [Header("UI End credit Panel ")]
    public GameObject endCreditPanel;

    [Header("UI Get ItemFlower Panel")] 
    [SerializeField] public GameObject ItemFlowerPanel;
    [SerializeField] private TextMeshProUGUI itemFlowerText;
    
    //[SerializeField] private int totalFlowers = 5;
    private int collectedFlowers = 0;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    private void Update()
    {
        HandlePauseToggle();
    }
    
    public int GetCollectedFlowerCount()
    {
        return collectedFlowers;
    }
    
    public void UpdateItemFlowerUI(int count, int required, int level) //public void UpdateItemFlowerUI(int count)
    {
        collectedFlowers = count;
        //itemFlowerText.text = collectedFlowers + " / " + totalFlowers;
        
        /*int level = MapGenerator.Instance.GetCurrentLevel();
        int required = GetRequiredFlowerCount(level);*/

        itemFlowerText.text = $"Collected: {collectedFlowers} / Required: {required} (Level {level})";
    }
    public int GetRequiredFlowerCount(int level)
    {
        switch (level)
        {
            case 2: return 5;
            case 3: return 8;
            case 4: return 10;
            case 5: return 12;
            default: return 0;
        }
    }
    public int GetRequiredFlowerForCurrentLevel()
    {
        int level = MapGenerator.Instance.GetCurrentLevel();
        return GameManager.Instance.GetRequiredFlowerCount(level);
    }
    public void AddCollectedFlower()
    {
        collectedFlowers++;
        int currentLevel = MapGenerator.Instance.GetCurrentLevel();
        int required = GetRequiredFlowerCount(currentLevel);
        UpdateItemFlowerUI(collectedFlowers, required, currentLevel);
    }

    public void ShowWinPanel()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            SoundManager.Instance.PlayWinSound();
        }
        
        SoundManager.Instance.StopBGM();
        
        StartCoroutine(DelayThenShowCredit());

    }
    private IEnumerator DelayThenShowCredit()
    {
        yield return new WaitForSecondsRealtime(3f);

        if (winPanel != null)
            winPanel.SetActive(false);

        if (endCreditPanel != null)
            endCreditPanel.SetActive(true);

        Time.timeScale = 1f;
    }
    
    public void ShowLosePanel()
    {

        if (Time.timeScale != 0f)
        {
            Time.timeScale = 0f;
        }
        
        if (losePanel != null)
        {
            losePanel.SetActive(true);
            Time.timeScale = 0f;
        }
        
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlayLoseSound();
    }
    
    public void ShowNoteDialog()
    {
        if (noteDialogPanel != null && pressEText != null)
        {
            noteDialogPanel.SetActive(true);
            pressEText.SetActive(true);
            noteText.SetActive(false);
        }
    }

    public void ShowNoteText()
    {
        if (noteText != null)
        {
            pressEText?.SetActive(false);
            noteText.SetActive(true);
            SoundManager.Instance.StopBGM();
        }
    }

    public void HideNoteDialog()
    {
        if (noteDialogPanel != null)
        {
            noteDialogPanel.SetActive(false);
            pressEText?.SetActive(false);
            noteText?.SetActive(false);
        }
    }
    
    public void ShowDialogPanel()
    {
        ItemDialogPanel.SetActive(true);
    }

    public void HitDialogPanel()
    {
        ItemDialogPanel.SetActive(false);
    }
    
    public void ShowNextLevelPanel()
    {
        if (nextLevelPanel != null)
        {
            nextLevelPanel.SetActive(true);
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
        GameManager.Instance.CloseItemFlowerPanel();
        SoundManager.Instance.StopBGM();
    }

    public void ShowItemFlowerPanel()
    {
        ItemFlowerPanel.SetActive(true);
    }

    public void CloseItemFlowerPanel()
    {
        ItemFlowerPanel.SetActive(false);
    }
    
    public void ClickNextDoorButton()
    {
        if (MapGenerator.Instance != null)
        {
            MapGenerator.Instance.NextLevel();
        }

        if (!SoundManager.Instance.MusicGameSource.isPlaying)
        {
            SoundManager.Instance.PlayBGM();
        }

        nextLevelPanel.SetActive(false);
       
        // ✅ รีเซตดอกไม้ที่เก็บมาแล้ว
        collectedFlowers = 0;

        // ✅ อัปเดตข้อความให้เป็นของด่านใหม่
        int currentLevel = MapGenerator.Instance.GetCurrentLevel(); // ถ้ามี method นี้
        int required = GameManager.Instance.GetRequiredFlowerForCurrentLevel();
        itemFlowerText.text = $"Collected: {collectedFlowers} / Required: {required} (Level {currentLevel})";
        
        ItemFlowerPanel.SetActive(true);
        Time.timeScale = 1f;
    }
    
    public void ResetGame()
    {
        pausePanel.SetActive(false);     
        Time.timeScale = 1f;
        Cursor.visible = false;          

        foreach (AudioSource audio in FindObjectsOfType<AudioSource>())
        {
            if (audio != null)
            {
                audio.UnPause();
            }

            if (SoundManager.Instance != null && SoundManager.Instance.MusicGameSource != null)
            {
                if (!SoundManager.Instance.MusicGameSource.isPlaying)
                {
                    SoundManager.Instance.PlayBGM();
                }
            }

        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ResumeGame()
    {
        if (pausePanel != null && pausePanel.activeSelf)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;           
            Cursor.visible = true;

            // ✅ เล่นเสียงที่เคย Pause ไว้
            foreach (AudioSource audio in FindObjectsOfType<AudioSource>())
            {
                if (audio != null)
                {
                    audio.UnPause();
                }
            }

            Debug.Log("เล่นเกมต่อแล้ว (Resume)");
        }
    }

    private void HandlePauseToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //แสดง Cursor เสมอเมื่อกด ESC
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            cursorHidden = false;

            //ถ้ามี Panel เหล่านี้เปิดอยู่ ห้าม Pause เกม
            if ((nextLevelPanel != null && nextLevelPanel.activeSelf) ||
                (winPanel != null && winPanel.activeSelf) ||
                (losePanel != null && losePanel.activeSelf) ||
                GameObject.Find("GamecoverPanel")?.activeSelf == true) 
            {
                return;
            }
            
            // ✅ NEW: ถ้า Time หยุดแล้วจาก Win หรือ Lose ไม่ให้ ESC มา Resume
            if (Time.timeScale == 0f && (winPanel.activeSelf || losePanel.activeSelf))
            {
                return;
            }
            
            // ✅ ปิด Note Dialog Panel ถ้ามี
            if (noteDialogPanel != null && noteDialogPanel.activeSelf)
            {
                HideNoteDialog();
            }

            // ✅ Toggle Pause Panel
            bool isPaused = pausePanel.activeSelf;
            //pausePanel.SetActive(!isPaused);
            
            if (!isPaused)
            {
                pausePanel.SetActive(true);
                Time.timeScale = 0f;

                if (ItemFlowerPanel != null && ItemFlowerPanel.activeSelf)
                {
                    ItemFlowerPanel.SetActive(false);
                }
            }
            else
            {
                // ถ้าอยู่ในสถานะ pause → resume เกม และเปิด ItemFlowerPanel
                pausePanel.SetActive(false);
                Time.timeScale = 1f;

                if (ItemFlowerPanel != null)
                {
                    ItemFlowerPanel.SetActive(true);
                }
            }

            // ✅ หยุดหรือเล่นเกมตามสถานะ Pause
            //Time.timeScale = isPaused ? 1f : 0f;

            // ✅ หยุดเสียงทั้งหมด
            foreach (AudioSource audio in FindObjectsOfType<AudioSource>())
            {
                if (!isPaused && audio.isPlaying)
                {
                    audio.Pause();
                }
                else if (isPaused)
                {
                    audio.UnPause();
                }
            }
        }
    }
    
    public void ExitGame()
    {
        Debug.Log("Exit Game");
        Application.Quit();
    }
}
