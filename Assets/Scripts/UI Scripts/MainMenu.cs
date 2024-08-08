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

    // Parte de Volumen
    public Slider masterVol, musicVol, sfxVol;
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
        mainAudioMixer.SetFloat("MusicVol", musicVol.value);
    }

    // Cambio de Volumen de los "efectos especiales" (Sfx)
    public void ChangeSfxVolume()
    {
        mainAudioMixer.SetFloat("SfxVol", sfxVol.value);
    }

    // Cambio de escena a Game (escena)
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    // Funcionamiento de cerrar juego
    public void QuitGame()
    {
        Application.Quit();
    }
}
