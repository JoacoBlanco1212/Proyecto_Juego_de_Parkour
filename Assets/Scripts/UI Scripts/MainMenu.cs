using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;
public class MainMenu : MonoBehaviour
{
    // Parte de Graphics Quality
    public TMP_Dropdown graphicsDropDown;
    public Slider masterVol, musicVol, sfxVol;
    public SoundManager soundManagerScp;
    public GameManager gameManagerScp;
    public AudioMixer mainAudioMixer;

    // Cambio de Graficos
    public void ChangeGraphicsQuality()
    {
        QualitySettings.SetQualityLevel(graphicsDropDown.value);
    }

    // Cambio de Volumen General
    public void ChangeMasterVolume()
    {
        mainAudioMixer.SetFloat("MasterVol", masterVol.value);
    }

    // Cambio de Volumen de música
    public void ChangeMusicVolume()

    {
        float volume = musicVol.value;
        mainAudioMixer.SetFloat("MusicVol", Mathf.Log10(volume) * 20);
      
    }

    // Cambio de Volumen de los "efectos especiales" (Sfx)
    public void ChangeSfxVolume()
    {
        mainAudioMixer.SetFloat("SfxVol", sfxVol.value);
    }


    // Cambio de escena a Game (escena)
    public void PlayGame()
    {
        gameManagerScp.ChangeScene("Game");
    }

    // Funcionamiento de cerrar juego
    public void QuitGame()
    {
        Application.Quit();
    }
}
