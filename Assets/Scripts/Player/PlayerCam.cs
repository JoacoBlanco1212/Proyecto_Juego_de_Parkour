using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public float targetPOV;
    private float startPOV;
    public float fovSpeed;

    public float tiltAngle;

    public Transform orientation;
    public Camera firstPersonCamera;
    public Transform playerModel;

    float xRotation;
    float yRotation;

    // Start is called before the first frame update
    void Start()
    {
        //Lock mouse and hide it 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        startPOV = firstPersonCamera.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        //Get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        
        xRotation += mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); //Limit camara movement

        transform.rotation = Quaternion.Euler(-xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        playerModel.rotation = Quaternion.Euler(0, yRotation, 0);

    }

    public void DoFov()
    {
        firstPersonCamera.fieldOfView = Mathf.Lerp(firstPersonCamera.fieldOfView, targetPOV, Time.deltaTime * fovSpeed);
    }
    public void EndFov()
    {
        firstPersonCamera.fieldOfView = Mathf.Lerp(firstPersonCamera.fieldOfView, startPOV, Time.deltaTime * fovSpeed);
    }

    public void DoTilt()
    {
        firstPersonCamera.transform.rotation = Quaternion.Euler(0, 0, tiltAngle);
    }

    public void EndTilt()
    {
        firstPersonCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void StartWallRunEffects()
    {
        DoFov();
        DoTilt();
    }

    public void EndWallRunEffects()
    {
        EndFov();
        EndTilt();
    }
}
