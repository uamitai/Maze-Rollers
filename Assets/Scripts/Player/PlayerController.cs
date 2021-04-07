﻿using UnityEngine;

//player's basic movement, camera, etc.
public class PlayerController : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float camLimit;
    [SerializeField] private float rollSpeed;
    [SerializeField] private Transform model;

    //stamina
    [SerializeField] public float staminaSpeed;
    [SerializeField] private float speedMultiplier;
    private float stamina;

    private Rigidbody rb;
    private float mouseX, rotation;
    private Vector3 x, z, vel;
    private const float maxStamina = 100;

    // Start is called before the first frame update
    void Start()
    {
        stamina = maxStamina;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Player.UI.pauseOn || !GetComponent<Player>().isAlive)
        {
            //player cannot move on pause
            return;
        }

        GetInput();

        //combine axes for velocity
        vel = (x + z).normalized * speed; 

        //sprint
        if(Input.GetButton("Jump") && vel != Vector3.zero && stamina >= 5)
        {
            vel *= speedMultiplier;
            stamina -= 2 * staminaSpeed * Time.deltaTime;
        }
        else
        {
            stamina += staminaSpeed * Time.deltaTime;
        }

        rotation = Mathf.Clamp(rotation, -camLimit, camLimit); //clamp rotation
        stamina = Mathf.Clamp(stamina, 0f, maxStamina);        //clamp stamina
    }

    void FixedUpdate()
    {
        if(Player.UI.pauseOn || !GetComponent<Player>().isAlive)
        {
            //player cannot move on pause
            return;
        }

        Move();

        //updare stamina meter
        Player.UI.SetStamina(stamina / maxStamina);
    }

    void GetInput()
    {
        //movement inputs
        x = transform.right * Input.GetAxisRaw("Horizontal");
        z = transform.forward * Input.GetAxisRaw("Vertical");

        //rotation inputs
        mouseX = Input.GetAxisRaw("Mouse X") * rotationSpeed * Time.deltaTime;
        rotation -= Input.GetAxisRaw("Mouse Y") * rotationSpeed * Time.deltaTime;
    }

    void Move()
    {
        //move
        rb.MovePosition(rb.position + vel * Time.fixedDeltaTime);

        //roll ball
        model.Rotate(new Vector3(vel.z, 0f, -vel.x) * rollSpeed * vel.magnitude * Time.fixedDeltaTime, Space.World);

        //rotate camera
        Player.localPlayer.cam.localRotation = Quaternion.Euler(rotation, 0f, 0f);

        //rotate player
        transform.Rotate(Vector3.up * mouseX);
    }
}
