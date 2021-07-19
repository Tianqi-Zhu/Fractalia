using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float horizontalSensitivity = 100f;
    public float verticalSensitivity = 10f;
    public Transform player;

    // private float xRotation = 0f;

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal") * horizontalSensitivity * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * verticalSensitivity * Time.deltaTime;

        /*
        // Rotate the camera around the x axis
        xRotation -= vertical;        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamp the rotation of camera so it cannot look behind the player
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        */

        // Rotate the player
        player.Rotate(Vector3.up * horizontal);
        player.Rotate(Vector3.right * vertical);
    }
}
