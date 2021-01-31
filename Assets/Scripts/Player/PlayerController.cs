using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//player's basic movement, camera, etc.
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float rotSpeed;
    [SerializeField] private Transform cam;
    [SerializeField] private float camRotLimit;
    [SerializeField] private float rollSpeed;

    private Transform model;
    private Rigidbody rb;
    private float mouseX, mouseY, xRot;
    private Vector3 x, z, vel;

    // Start is called before the first frame update
    void Start()
    {
        model = transform.GetChild(0);
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerUI.pauseOn)
        {
            //unlock mouse
            Cursor.lockState = CursorLockMode.None;

            //player cannot move on pause
            return;
        }

        //lock mouse
        Cursor.lockState = CursorLockMode.Locked;

        //get movement inputs
        x = transform.right * Input.GetAxisRaw("Horizontal");
        z = transform.forward * Input.GetAxisRaw("Vertical");

        //combine for velocity
        vel = (x + z).normalized * speed;

        //get rotation inputs
        mouseX = Input.GetAxisRaw("Mouse X") * rotSpeed * Time.deltaTime;
        mouseY = Input.GetAxisRaw("Mouse Y") * rotSpeed * Time.deltaTime;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -camRotLimit, camRotLimit);
    }

    void FixedUpdate()
    {
        if (PlayerUI.pauseOn)
        {
            //player cannot move on pause
            return;
        }

        //add v * dt to current pos
        rb.MovePosition(rb.position + vel * Time.fixedDeltaTime);

        //roll ball
        model.Rotate(new Vector3(vel.z, 0f, -vel.x) * rollSpeed * Time.fixedDeltaTime, Space.World);

        //rotate camera
        cam.localRotation = Quaternion.Euler(xRot, 0f, 0f);

        //rotate player
        transform.Rotate(Vector3.up * mouseX);
    }
}
