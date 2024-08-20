using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    public PauseMenu pauseMenu;

    void Start()
    {
        
    }

    void Update()
    {
        // It detects if the key Escape is pressed
         if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.Pause();
        }
    }
}
