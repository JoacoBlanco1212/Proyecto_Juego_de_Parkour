using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;

    public float walkSpeedThreshold = 0.3f; // Velocidad mínima para caminar
    public float runSpeedThreshold = 0.6f;  // Velocidad mínima para correr
    public float turnSensitivity = 0.5f;    // Sensibilidad para giros

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Obtener input del jugador
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 velocity = new Vector3(horizontal, 0, vertical);

        // Calcular magnitud de velocidad (moverse en Walk o Run)
        float speed = Mathf.Clamp01(velocity.magnitude);

        // Actualizar parámetro Speed
        animator.SetFloat("Speed", speed);

        // Cambiar a caminar o correr según el umbral
        if (speed > walkSpeedThreshold && speed <= runSpeedThreshold)
        {
            // Estado Walk
            animator.SetFloat("Speed", 0.5f);
        }
        else if (speed > runSpeedThreshold)
        {
            // Estado Run
            animator.SetFloat("Speed", 1.0f);
        }

        // Giros izquierda/derecha
        if (horizontal < -turnSensitivity)
        {
            animator.SetFloat("TurnDirection", -1f); // Girar a la izquierda
        }
        else if (horizontal > turnSensitivity)
        {
            animator.SetFloat("TurnDirection", 1f); // Girar a la derecha
        }
        else
        {
            animator.SetFloat("TurnDirection", 0f); // Sin giro
        }

        // Detectar salto
        if (Input.GetButtonDown("Jump"))
        {
            animator.SetBool("IsJumping", true);
        }
        else
        {
            animator.SetBool("IsJumping", false);
        }

    }
}
