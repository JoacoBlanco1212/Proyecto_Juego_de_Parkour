using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    public float velocidad= 5f;
    public Animator animator;

    void start()
    {

    }
    void update()
    {
        float velocidadX = Input.GetAxis("Horizontal")*Time.deltaTime*velocidad;
        
        animator.SetFloat("speed", velocidadX*velocidad);

        if (velocidadX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        if (velocidadX < 0)
        {
            transform.localScale = new Vector3 (-1, 1, 1);
        }
        Vector3 posicion = transform.position;
        
        transform.position = new Vector3(velocidadX + posicion.x,posicion.y,posicion.z);

    }
}
