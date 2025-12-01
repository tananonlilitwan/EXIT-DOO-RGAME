using UnityEngine;

public class PanelSoundTrigger : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject losePanel;

    [Tooltip("Sound Exit Door Game")]
    public AudioSource winSound;  

    [Tooltip("Sound YOU DIE")]
    public AudioSource loseSound;  

    private bool winPlayed = false;
    private bool losePlayed = false;

    void Update()
    {
        if (winPanel.activeSelf && !winPlayed && winSound != null)
        {
            winSound.Play();
            winPlayed = true;
        }

        if (losePanel.activeSelf && !losePlayed && loseSound != null)
        {
            loseSound.Play();
            losePlayed = true;
        }
    }
}