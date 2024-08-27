using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioSource sourceSFX;

    public AudioClip mainMenuMusic;
    public AudioClip[] worldMusic;

    public AudioClip currentSong;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(this);
    }

    public void PlayMenuMusic()
    {
        sourceSFX.clip = mainMenuMusic;
        sourceSFX.loop = true;
        sourceSFX.PlayOneShot(mainMenuMusic);
    }

    private void Start()
    {
        PlayMenuMusic();
    }

    void Update()
    {
        whenToChangeSong();
    }
    public void whenToChangeSong()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (!sourceSFX.isPlaying)
            {
                SetRandomOnPlaylist();
            }
        }
    }
    public void SetRandomOnPlaylist()
    {
        int rng = Random.Range(0, worldMusic.Length - 1);
        currentSong = worldMusic[rng];
        PlayWorldMusic();
    }
    public void PlayWorldMusic()
    {
        sourceSFX.clip = currentSong;
        sourceSFX.loop = false;
        sourceSFX.PlayOneShot(currentSong);
    }
}