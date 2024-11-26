using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraControllerScript : MonoBehaviour
{
    public Transform playerOrientation;
    public Transform playerModel;
    public Vector3 offset;
    public float height;

    private Quaternion targetRotation;

    private float yRotation;
    private float xRotation;

    public float xRotationMin;
    public float xRotationMax;
    private float xRotationClamped;

    public float xSensitivity;
    public float ySensitivity;

    public Vector3 desiredPos;

    public LayerMask collisionLayers;
    public float collisionRadius = 0.2f;
    public float cameraCollisionOffset = 0.2f; 


    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        yRotation += Input.GetAxis("Mouse X") * ySensitivity;
        xRotation -= Input.GetAxis("Mouse Y") * xSensitivity;
    }

    void LateUpdate()
    {
        xRotationClamped = Mathf.Clamp(xRotation, xRotationMin, xRotationMax);
        targetRotation = Quaternion.Euler(xRotationClamped, yRotation, 0f);

        desiredPos = playerOrientation.position - targetRotation * offset + Vector3.up * height;

        RaycastHit hit;
        bool cameraCollision = Physics.SphereCast(playerOrientation.position, collisionRadius, desiredPos - playerOrientation.position, out hit, offset.magnitude);
        // Debug.DrawRay(playerOrientation.position, (desiredPos - playerOrientation.position) * offset.magnitude, cameraCollision ? Color.green : Color.red);
        if (cameraCollision)
        {
            // If a collision is detected, adjust the camera position to avoid clipping
            desiredPos = hit.point + hit.normal * cameraCollisionOffset;
        }

        transform.SetPositionAndRotation(desiredPos, targetRotation);

        Quaternion playerTargetRotation = Quaternion.Euler(0f, yRotation, 0f);
        playerOrientation.rotation = Quaternion.Slerp(playerOrientation.rotation, playerTargetRotation, Time.deltaTime * 10f);
        playerModel.rotation = Quaternion.Slerp(playerModel.rotation, playerTargetRotation, Time.deltaTime * 10f);
}
}
