using UnityEngine;
using UnityEngine.UI;

using UnityEngine;

public class NoteInteraction : MonoBehaviour
{
    private bool playerInRange = false;
    private bool noteRead = false;

    void Update()
    {
        // ปิด UI ถ้าอยู่ใน win/lose panel
        if (GameManager.Instance.winPanel?.activeSelf == true || GameManager.Instance.losePanel?.activeSelf == true)
        {
            GameManager.Instance.HideNoteDialog();
            Time.timeScale = 1f;
            return;
        }

        if (playerInRange && !noteRead)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                GameManager.Instance.ShowNoteText();
                Time.timeScale = 0f;
            }
        }

        if (Input.GetKeyDown(KeyCode.X) && GameManager.Instance.noteText?.activeSelf == true)
        {
            GameManager.Instance.HideNoteDialog();
            Time.timeScale = 1f;
            SoundManager.Instance.PlayBGM();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !noteRead)
        {
            if (GameManager.Instance.winPanel?.activeSelf == true || GameManager.Instance.losePanel?.activeSelf == true)
                return;

            playerInRange = true;
            GameManager.Instance.ShowNoteDialog();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !noteRead)
        {
            playerInRange = false;
            GameManager.Instance.HideNoteDialog();
        }
    }
}
