using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraControllerScript : MonoBehaviour
{
    public Transform player;
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

        desiredPos = player.position - targetRotation * offset + Vector3.up * height;

        transform.SetPositionAndRotation(desiredPos, targetRotation);

        Quaternion playerTargetRotation = Quaternion.Euler(0f, yRotation, 0f);
        player.rotation = Quaternion.Slerp(player.rotation, playerTargetRotation, Time.deltaTime * 10f);
    }
}
