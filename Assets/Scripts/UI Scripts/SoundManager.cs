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
    public AudioSource sourceMusic;

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

    private void Start()
    {
        sourceMusic.loop = false;
    }

    void Update()
    {
        whenToChangeSong();
    }
    public void whenToChangeSong()
    { 
        if (!sourceMusic.isPlaying)
        {
            SetRandomOnPlaylist();
        }
    }
    public void SetRandomOnPlaylist()
    {
        int rng = Random.Range(0, worldMusic.Length);
        currentSong = worldMusic[rng];
        PlayWorldMusic(currentSong);
    }
    public void PlayWorldMusic(AudioClip clip)
    {
        sourceMusic.clip = clip;
        sourceMusic.Play();
    }
}