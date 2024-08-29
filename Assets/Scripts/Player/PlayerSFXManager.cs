using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFXManager : MonoBehaviour
{
    [Header("SFX")]
    public AudioClip sprintSFX;
    public AudioClip walkSFX;
    public AudioClip jumpSFX;
    public AudioClip landSFX;
    // Se crea una variable y funcion para cada audio

    [Header("References")]
    public AudioSource sourceSFX;

    // Start is called before the first frame update
    void Start()
    {
        sourceSFX.clip = null; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySprintSFX()
    {
        if (!sourceSFX.isPlaying)
        {
            sourceSFX.clip = sprintSFX;
            sourceSFX.PlayOneShot(sprintSFX);
        }
    }
    
    public void PlayWalkSFX()
    {
        if (!sourceSFX.isPlaying)
        {
            sourceSFX.clip = walkSFX;
            sourceSFX.PlayOneShot(walkSFX);
        }
    }
}
