using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterscript : MonoBehaviour
{
    private float movementspeed;
    private float mousesensitivity;
    private float jumpforce;
    private BoxCollider ground; // detecta el piso
    private LayerMask groundmask;
    private Rigidbody rb;
    private Vector2 movement = Vector2.zero; //almacena el movimiento WASD
    private bool grounded = false;
    private Animator ertyg;
    private string currentanimation;
    void Start()
    {
        rb = GetComponent<Rigidbody>();//llama al rigidbody
        ertyg = GetComponent<Animator>(); //llama al animator
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // bloquea y oculta el cursor
        changeAnimations("jump");
    }

    // Update is called once per frame
    void Update()
    {
        //tiene el movimiento wasd
        movement = new Vector2(Input.GetAxisRaw("horizontal"), Input.GetAxisRaw("vertical"));
        //tiene el movimiento del mouse
        Vector2 mouse = mousesensitivity * Time.deltaTime * new Vector2(Input.GetAxisRaw("mouseX"), (Input.GetAxisRaw("mousey")));
        //hace que el personaje gire
        transform.Rotate(mouse.x * Vector3.up);
       //
       if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(jumpforce * Vector3.up, ForceMode.Impulse); //le agrega fuerza/salto vertical
        }
    }
    private void changeAnimations (string animation, float crossfade = 0.2f)
    {
        if(currentanimation != animation)
        {
            currentanimation = animation;
            object p = Animator.Crossfade(animation, crossfade); //transiciona las animaciones 
        }
    }
    private void FixedUpdate()
    {
        Vector3 velocity = movementspeed * (movement.x * transform.right + movement.y * transform.forward); //consigue la direccion de la velocidad
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z); //aplica velocidad
        grounded = Physics.CheckBox(ground.transform.position + ground.center, 0.5f * ground.size, ground.transform.rotation, groundmask); //actualiza el grounded
    }
}
