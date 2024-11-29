using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{


    public class PlayerMovement : MonoBehaviour
    {
        public Animator animator; // Referencia al Animator
        public float speed = 5f; // Velocidad de movimiento
        public float jumpForce = 5f; // Fuerza de salto
        private Rigidbody rb; // Referencia al Rigidbody
        private bool isGrounded = true; // Para detectar si está en el suelo
        private bool isWallRunning = false; // Para detectar si está corriendo en la pared
        private bool isClimbing = false; // Para detectar si está escalando

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        void Update()
        {
            // Movimiento del personaje
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;

            // Movimiento básico
            if (moveDirection.magnitude >= 0.1f)
            {
                // Mover al personaje en el mundo 3D
                transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

                // Controlar las animaciones de caminar o correr
                animator.SetFloat("Speed", moveDirection.magnitude);
            }
            else
            {
                // Si no se está moviendo, animación de "Idle"
                animator.SetFloat("Speed", 0);
            }

            // Detectar giros a la izquierda o derecha
            if (Input.GetKeyDown(KeyCode.A))  // Giro hacia la izquierda
            {
                animator.SetBool("TurnLeft", true);
            }
            else if (Input.GetKeyDown(KeyCode.D))  // Giro hacia la derecha
            {
                animator.SetBool("TurnRight", true);
            }

            // Detectar cuando se termina el giro
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            {
                animator.SetBool("TurnLeft", false);
                animator.SetBool("TurnRight", false);
            }

            // Salto del personaje
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;

                // Activar animación de salto
                animator.SetBool("IsJumping", true);
            }

            // Transición a "Idle" después del salto
            if (isGrounded)
            {
                animator.SetBool("IsJumping", false);
            }

            // Wall Run (corriendo por la pared)
            if (isWallRunning)
            {
                animator.SetBool("WallRun", true);
            }
            else
            {
                animator.SetBool("WallRun", false);
            }

            // Escalar paredes
            if (isClimbing)
            {
                animator.SetBool("ClimbWall", true);
            }
            else
            {
                animator.SetBool("ClimbWall", false);
            }
        }

        // Detectar si el personaje está en el suelo
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
            }
        }

        // Detectar si está corriendo por la pared (esto puede ser activado por alguna lógica personalizada)
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Wall"))
            {
                isWallRunning = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Wall"))
            {
                isWallRunning = false;
            }
        }

        // Detectar si está escalando
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Climbable"))
            {
                isClimbing = true;
            }
        }

        private void OnTriggerExitClimb(Collider other)
        {
            if (other.CompareTag("Climbable"))
            {
                isClimbing = false;
            }
        }
    }
}
