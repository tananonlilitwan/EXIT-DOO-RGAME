using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource MusicGameSource;
    public AudioSource soundEffectSource;
    public AudioSource exitDoorSource;
    public AudioSource enemySource;
    public AudioSource winSource;
    public AudioSource loseSource;
    public AudioSource EndCreditSounrce;

    [Header("Audio Clips")]
    public AudioClip MusicGameClip;
    public AudioClip soundEffectClip;
    public AudioClip exitDoorClip;
    public AudioClip enemyClip;
    public AudioClip winClip;
    public AudioClip loseClip;
    public AudioClip EndCreditClip;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float MusicGameVolume = 1f;
    [Range(0f, 1f)] public float soundEffectVolume = 1f;
    [Range(0f, 1f)] public float exitDoorVolume = 1f;
    [Range(0f, 1f)] public float enemyVolume = 1f;
    [Range(0f, 1f)] public float winVolume = 1f;
    [Range(0f, 1f)] public float loseVolume = 1f;
    [Range(0f, 1f)] public float EndCreditVolume = 1f;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        ApplyVolumes();
    }

    private void Update()
    {
        ApplyVolumes();
    }

    public void ApplyVolumes()
    {
        if (MusicGameSource) MusicGameSource.volume = MusicGameVolume * masterVolume;
        if (soundEffectSource) soundEffectSource.volume = soundEffectVolume * masterVolume;
        if (exitDoorSource) exitDoorSource.volume = exitDoorVolume * masterVolume;
        if (enemySource) enemySource.volume = enemyVolume * masterVolume;
        if (winSource) winSource.volume = winVolume * masterVolume;
        if (loseSource) loseSource.volume = loseVolume * masterVolume;
        if (EndCreditSounrce) EndCreditSounrce.volume = EndCreditVolume * masterVolume;
    }

    public void EndCreditSound()
    {
        if (EndCreditClip != null &&  EndCreditSounrce != null)
        {
            EndCreditSounrce.clip = EndCreditClip;
            EndCreditSounrce.loop = true;
            EndCreditSounrce.Play();
            
        }
    }
    public void StopEndCreditSound()
    {
        if (EndCreditSounrce != null && EndCreditSounrce.isPlaying)
        {
            EndCreditSounrce.Stop();
        }
    }

    public void PlayBGM()
    {
        if (MusicGameClip != null && MusicGameSource != null)
        {
            MusicGameSource.clip = MusicGameClip;
            MusicGameSource.loop = true;
            MusicGameSource.Play();
        }
    }
    public void StopBGM()
    {
        if (MusicGameSource != null && MusicGameSource.isPlaying)
        {
            MusicGameSource.Stop();
        }
    }


    public void PlaySoundEffect()
    {
        PlaySound(soundEffectSource, soundEffectClip);
    }

    public void PlayExitDoor()
    {
        PlaySound(exitDoorSource, exitDoorClip);
    }

    public void PlayEnemySound()
    {
        PlaySound(enemySource, enemyClip);
    }

    public void PlayWinSound()
    {
        PlaySound(winSource, winClip);
    }

    public void PlayLoseSound()
    {
        PlaySound(loseSource, loseClip);
    }

    private void PlaySound(AudioSource source, AudioClip clip)
    {
        if (source != null && clip != null)
        {
            source.clip = clip;
            source.Play();
        }
    }
}