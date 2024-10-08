﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiDebugModeHandler : MonoBehaviour
{
    public Text speedText;


    public GameObject player;
    CharacterControllerScript playerScript;

    Rigidbody playerRB;
    

    // Start is called before the first frame update
    void Start()
    {
        playerRB = player.GetComponent<Rigidbody>();
        playerScript = player.GetComponent<CharacterControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 flatVel = new Vector2 (playerRB.velocity.x, playerRB.velocity.z);
        speedText.text = "Speed: " + playerRB.velocity.ToString() + " [" + flatVel.magnitude + "]";
    }
}
