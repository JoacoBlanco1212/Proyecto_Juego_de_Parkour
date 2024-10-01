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
    public AudioClip damageSFX;

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

    public void PlayJumpSFX()
    {
        {
            sourceSFX.clip = jumpSFX;
            sourceSFX.PlayOneShot(jumpSFX);
        }
    }
    public void PlayDamageSFX()
    {
        if (!sourceSFX.isPlaying)
        {
            sourceSFX.clip = damageSFX;
            sourceSFX.PlayOneShot(damageSFX);
        }

    }
    public void PlaylandSFX()
    {
        if (!sourceSFX.isPlaying)
        {
            sourceSFX.clip = landSFX;
            sourceSFX.PlayOneShot(landSFX);
        }
    }
}
