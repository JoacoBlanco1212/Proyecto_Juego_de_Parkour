using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator anim;

    // Update is called once per frame
    void Start()
    {
     
    }
    void Update()
    {
       if (Input.GetKeyDown(KeyCode.W))
        {
           
        }
       if (Input.GetKeyDown(KeyCode.A))
        {

        }
       if (Input.GetKeyDown(KeyCode.S))
        {

        }
       if (Input.GetKeyDown(KeyCode.D))
        {

        }
       if (Input.GetKeyDown(KeyCode.C))
        {

        }
       if (Input.GetKeyDown(KeyCode.Escape))
        {

        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("Trigger");
        }
       
    }
}
