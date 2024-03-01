using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float sensetivity = 100f;
    [SerializeField] private float maxAngleX = 80; //Max angle up/down rotation by X axis
    [SerializeField] private Transform player; //Player transform for rotate
    private float xRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensetivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensetivity;

        xRotation = Mathf.Clamp(xRotation - mouseY, -maxAngleX, maxAngleX);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        player.Rotate(Vector3.up * mouseX);
    }
}
